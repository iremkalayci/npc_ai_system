using UnityEngine;
using UnityEngine.AI;

public class NPCTwoAI : MonoBehaviour
{
    [Header("References")]
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    [Header("Detection & Combat Settings")]
    [SerializeField] private float viewDistance = 18f;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float attackRate = 1.8f;
    [SerializeField] private float damageAmount = 10f;
    private float nextAttackTime = 0f;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 60f;
    private float currentHealth;
    private bool isDead = false;

    // === START ===
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        animator.applyRootMotion = false;
        GoToNextPatrolPoint();
    }

    
    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Hareket animasyonu
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float currentSpeed = agent.velocity.magnitude;
        float vInput = Mathf.InverseLerp(0, runSpeed, currentSpeed);
        float hzInput = localVelocity.x / runSpeed;

        animator.SetFloat("vInput", vInput, 0.1f, Time.deltaTime);
        animator.SetFloat("hzInput", hzInput, 0.1f, Time.deltaTime);

        if (distance > viewDistance)
            Patrol();
        else if (distance > attackDistance)
            ChasePlayer();
        else
            AttackPlayer();
    }

    
    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.speed = walkSpeed;

        if (!agent.hasPath)
            GoToNextPatrolPoint();

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPatrolPoint();
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }


    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
    }

    // === ATTACK ===
    private void AttackPlayer()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Dönüş
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0f;
        if (lookPos != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), 5f * Time.deltaTime);

        // Saldırı
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            animator.SetTrigger("Attack");

            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damageAmount);
        }
    }

    // === DAMAGE ===
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} → Damage: {amount}, Remaining HP: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        agent.isStopped = true;

        // animasyon ve collider
        animator.ResetTrigger("Attack");
        animator.SetTrigger("FallingBackDeath");

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Debug.Log($"{gameObject.name} öldü!");
        Destroy(gameObject, 3f);
    }

    // === COLLISION & TRIGGER ===
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Debug.Log($"{gameObject.name} OnTriggerEnter → {other.name}");
            TakeDamage(30f);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            Debug.Log($"{gameObject.name} OnCollisionEnter → {other.gameObject.name}");
            TakeDamage(30f);
            Destroy(other.gameObject);
        }
    }
}