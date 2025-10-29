using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Fight Distance")]
    public float desiredShootDistance = 2f;
    public float keepDistanceBuffer = 0.3f;
    public float chaseStoppingDistance = 5f; 
    public float strafeSpeed = 1.5f;
    public float turnSpeed = 6f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    [Header("Detection Settings")]
    public float viewDistance = 20f;
    public float attackDistance = 10f;
    public float fireRate = 1.2f;
    private float nextFireTime = 0f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public float damage = 20f;

    private PlayerHealth playerHealth;
    private bool isAiming = false; 

    private const float WalkSpeed = 3.0f;
    private const float RunSpeed = 6.5f;
    private float currentVInput = 0f;
    private float currentHInput = 0f;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        isAiming = true;
        animator.SetBool("Aiming", isAiming);

        GoToNextPatrolPoint();
        Invoke(nameof(GoToNextPatrolPoint), 0.2f);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        animator.SetBool("Aiming", isAiming); 

        // --- OPTİMİZE HAREKET VE ANİMASYON KONTROLÜ (vInput & hInput) ---
        // Ajanın dünya hızını, NPC'nin lokal koordinatlarına çevir.
        Vector3 worldDelta = agent.desiredVelocity;
        Vector3 localDelta = transform.InverseTransformDirection(worldDelta);

        // İstenen vInput ve hInput değerlerini ayarla
        float desiredVInput = localDelta.z;
        float desiredHInput = localDelta.x;

        // V-Input için Koşma/Yürüme Hızı Ölçeklendirmesi
        // Patrol'de yürüsün, Kovalamada koşsun
        float maxSpeedScale = (distance > viewDistance || agent.speed == WalkSpeed) ? WalkSpeed : RunSpeed;
        
        // vInput ve hInput değerlerini normalize ederek animatöre gönder
        currentVInput = Mathf.Lerp(currentVInput, desiredVInput / maxSpeedScale, Time.deltaTime * 10f);
        currentHInput = Mathf.Lerp(currentHInput, desiredHInput / maxSpeedScale, Time.deltaTime * 10f);

        animator.SetFloat("vInput", Mathf.Clamp(currentVInput, -1f, 1f));
        animator.SetFloat("hInput", Mathf.Clamp(currentHInput, -1f, 1f));
        // --------------------------------------------------------

        if (distance > viewDistance)
        {
            Patrol();
            return;
        }

       // --- Yeni mantık: Görür görmez ateşe geçsin ---
if (distance > attackDistance)
{
    // Görüyor ama çok uzakta -> yavaş koşarak yaklaşırken ateş et
    ChaseAndShoot();
}
else
{
    // Normal saldırı davranışı
    AttackPlayerAtRange(distance);
}


        // Hareket yoksa (durma mesafesine ulaşıldıysa) ajanı durdur
        if (agent.remainingDistance < 0.1f && !agent.pathPending && agent.velocity.sqrMagnitude < 0.1f)
            agent.velocity = Vector3.zero;

        // Yönlendirme (Rotasyon) kontrolü
        if (!agent.updateRotation)
        {
            Vector3 targetDir = (agent.isStopped || distance < desiredShootDistance) ? (player.position - transform.position) : agent.velocity;
            targetDir.y = 0;
            if (targetDir.sqrMagnitude > 0.01f)
            {
                Quaternion rot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
            }
        }
    }
     void ChaseAndShoot()
{
    if (player == null) return;

    agent.isStopped = false;
    agent.updateRotation = false;
    agent.speed = RunSpeed * 0.8f; // biraz daha yavaş koşsun
    agent.acceleration = 15f;
    agent.stoppingDistance = chaseStoppingDistance;

    agent.SetDestination(player.position);

    // 🔹 Animasyonlar
    animator.SetBool("Walking", false);
    animator.SetBool("Running", true);
    animator.SetBool("Aiming", true);
    animator.SetBool("Shooting", true);

    // 🔹 Hedefe bak
    Vector3 lookDir = (player.position - transform.position);
    lookDir.y = 0;
    if (lookDir.sqrMagnitude > 0.1f)
    {
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    // 🔹 Mermi gönder
    if (Time.time >= nextFireTime)
    {
        nextFireTime = Time.time + fireRate;
        if (firePoint != null)
        {
            firePoint.LookAt(player.position + Vector3.up * 1.2f);
        }
        FireBullet();
    }
}

    // --- DEVRIYE (YÜRÜME İSTEĞİ BURADA ÇÖZÜLÜYOR) ---
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        Debug.Log($"Yeni devriye hedefi: {patrolPoints[currentPatrolIndex].name}");

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.updateRotation = true; 
        agent.speed = WalkSpeed; // PATROL HIZI: YÜRÜME
        agent.acceleration = 10f;
        agent.stoppingDistance = 0.1f;
       animator.SetBool("Running", false);
        animator.SetBool("Walking", true);
        animator.SetBool("Shooting", false); 

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            GoToNextPatrolPoint();
    }

    // --- KOVALAMA (KOŞMA) ---
    void ChasePlayer()
{
    if (player == null) return;

    agent.isStopped = false;
    agent.updateRotation = false; 
    agent.speed = RunSpeed;   // 🔥 KOVALARKEN KOŞMA
    agent.acceleration = 25f;
    agent.angularSpeed = 720f;
    agent.stoppingDistance = chaseStoppingDistance;

    agent.SetDestination(player.position);

    // 🔹 Koşma animasyonu
    animator.SetBool("Walking", false);
    animator.SetBool("Running", true);
    animator.SetBool("Shooting", false);

    // 🔹 Eğer menzile girdiyse ateş etsin
    float distance = Vector3.Distance(transform.position, player.position);
    if (distance <= attackDistance)
    {
        animator.SetBool("Shooting", true);
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            if (firePoint != null)
            {
                Vector3 aimPos = player.position + Vector3.up * 1.2f; 
                firePoint.LookAt(aimPos);
            }
            FireBullet();
        }
    }

    // 🔹 Yönünü düzgün döndür (kaymadan)
    Vector3 dir = (player.position - transform.position).normalized;
    dir.y = 0;
    if (dir.sqrMagnitude > 0.1f)
    {
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
    }
}


    // --- SALDIRI (YAKIN MENZİL VE STRAFE) ---
    void AttackPlayerAtRange(float dist)
    {
        if (player == null) return;

        if (dist > desiredShootDistance + keepDistanceBuffer)
        {
            ChasePlayer();
            return;
        }

        agent.isStopped = false;
        agent.updateRotation = false;
        agent.speed = 2.0f; 
        agent.stoppingDistance = 0.05f;

        // Strafe hareketi: NavMeshAgent hedefi sürekli güncellendiği için 
        // animasyon vInput ve hInput'a güvenebiliriz.
        Vector3 strafe = transform.right * Mathf.Sin(Time.time * 1.5f) * strafeSpeed;
        agent.SetDestination(transform.position + strafe * Time.deltaTime);

        animator.SetBool("Shooting", true);

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (firePoint != null)
            {
                Vector3 aimPos = player.position + Vector3.up * 1.2f;
                firePoint.LookAt(aimPos);
            }

            FireBullet();
        }
    }

    // --- MERMİ & DEBUG METOTLARI ---
    void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("bulletPrefab veya FirePoint eksik.");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * bulletSpeed;

        Destroy(bullet, 3f);
    }

    void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
        }
    }
    // === ÖLÜM SİSTEMİ ===
private bool isDead = false;

public void TakeDamage(float amount)
{
    if (isDead) return;

    // Örnek: Enemy'nin health değeri
    float currentHealth = 100f; // istersen dışarıdan tanımlarsın
    currentHealth -= amount;

    if (currentHealth <= 0)
    {
        Die();
    }
}
  void Die()
{
    if (isDead) return;
    isDead = true;

    // 🔹 Önce bütün bool parametreleri sıfırla:
    animator.SetBool("Aiming", false);
    animator.SetBool("Walking", false);
    animator.SetBool("Running", false);
    animator.SetBool("Shooting", false);

    // 🔹 Ölüm tetikle:
    animator.SetBool("isDied", true);

    // 🔹 Agent kapat ki fizik rahat etsin
    if (agent != null)
        agent.enabled = false;

    Collider col = GetComponent<Collider>();
    if (col) col.enabled = false;

    // 🔹 Apply Root Motion çalışsın diye
    animator.applyRootMotion = true;

    Destroy(gameObject, 5f);
}
    
    void OnCollisionEnter(Collision other)
{
    if (other.gameObject.CompareTag("PlayerBullet"))
    {
        TakeDamage(50f);
        Destroy(other.gameObject); // mermiyi yok et
    }
}


}