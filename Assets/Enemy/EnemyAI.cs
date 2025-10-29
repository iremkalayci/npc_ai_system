using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Fight Distance")]
public float desiredShootDistance = 10f;   
public float keepDistanceBuffer   = 2f;    
public float strafeSpeed          = 1.5f;  
public float turnSpeed            = 6f;    

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

        GoToNextPatrolPoint();
         Invoke(nameof(GoToNextPatrolPoint), 0.2f);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
            AttackPlayerAtRange(distance);
        else if (distance <= viewDistance)
            ChasePlayer();
        else
            Patrol();

        UpdateAnimation();
    }

    
 void GoToNextPatrolPoint()
{
    if (patrolPoints.Length == 0) return;

    
    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    Debug.Log($"Yeni devriye hedefi: {patrolPoints[currentPatrolIndex].name}");

    
    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
}

   

    void Patrol()
{
    void Patrol()
{
    if (patrolPoints == null || patrolPoints.Length == 0) return;

    agent.isStopped = false;
    agent.updateRotation = true;
    agent.speed = 2.5f;
    agent.stoppingDistance = 0.1f;

    animator.SetBool("Aiming", false);
    animator.SetBool("Shooting", false);

    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        GoToNextPatrolPoint();
}

}


    // 🟡 TAKIP
    void ChasePlayer()
    {
        agent.isStopped = false;
    agent.updateRotation = true;
    agent.speed = 3.5f;
    Vector3 dir = (player.position - transform.position).normalized;
    Vector3 targetPos = player.position - dir * desiredShootDistance;
    agent.stoppingDistance = keepDistanceBuffer;
    agent.SetDestination(targetPos);
    animator.SetBool("Aiming", false);
    animator.SetBool("Shooting", false);
    }

    // 🔴 SALDIRI
    void AttackPlayer()
    {
        agent.isStopped = true;

        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 6f);
        }

        animator.SetBool("Aiming", true);
        animator.SetBool("Shooting", true);

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireBullet();
        }
    }
    void AttackPlayerAtRange(float dist)
{
    // NavMeshAgent ile hafif pozisyon düzeltmeleri yapalım ama dönmeyi biz kontrol edelim
    agent.isStopped = false;
    agent.updateRotation = false; // dönüşü biz yapacağız
    agent.speed = 2.0f;

    // Mesafeyi koru: çok yaklaştıysa hafif geri/yan hareket
    if (dist < desiredShootDistance - keepDistanceBuffer)
    {
        Vector3 away = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + away * 1.0f);
    }
    else
    {
        // küçük strafing (opsiyonel)
        Vector3 right = transform.right * Mathf.Sin(Time.time * 1.2f) * strafeSpeed;
        agent.SetDestination(transform.position + right * Time.deltaTime);
    }

    // oyuncuya doğru yumuşak dönüş
    Vector3 look = player.position - transform.position; look.y = 0;
    if (look.sqrMagnitude > 0.01f)
    {
        Quaternion rot = Quaternion.LookRotation(look.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
    }

    // nişan anim durumları
    animator.SetBool("Aiming", true);
    animator.SetBool("Shooting", true);

    // ateş aralığı
    if (Time.time >= nextFireTime)
    {
        nextFireTime = Time.time + fireRate;

        // firePoint’i hedefe çevir (hafif sapmaları toparlar)
        if (firePoint != null)
        {
            Vector3 aimPos = player.position + Vector3.up * 1.2f; // göğüs hizası
            firePoint.LookAt(aimPos);
        }
        FireBullet();
    }
}


    // 💥 MERMI OLUŞTURMA
    void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null)
    {
        Debug.LogWarning("⚠️ BulletPrefab veya FirePoint eksik.");
        return;
    }

    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    if (rb != null)
        rb.velocity = firePoint.forward * bulletSpeed;

    Destroy(bullet, 3f);
    }

    // 🔄 ANIMASYON
    void UpdateAnimation()
    {
        Vector3 localVel = transform.InverseTransformDirection(agent.velocity);
        animator.SetFloat("vInput", localVel.z);
        animator.SetFloat("hzInput", localVel.x);
    }

    // 🔍 GIZMO
    void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
        }
    }
}
