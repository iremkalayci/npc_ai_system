using UnityEngine;
using UnityEngine.AI;

public class NPCTwoAI : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    private Transform player;

    [Header("Detection & Combat Settings")]
    public float viewDistance = 18f;      
    public float attackDistance = 3f;     
    public float attackRate = 1.8f;       
    public float damageAmount = 10f;      
    private float nextAttackTime = 0f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Speeds")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        animator.applyRootMotion = false;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // === hız hesaplama ===
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float currentSpeed = agent.velocity.magnitude;
        float vInput = Mathf.InverseLerp(0, runSpeed, currentSpeed);
        float hzInput = localVelocity.x / runSpeed;  // 🔥 EKLENDİ — yönelme input'u

        // === animator’a ver ===
        if (agent.remainingDistance > 0.2f && agent.velocity.magnitude > 0.05f)
        {
            animator.SetFloat("vInput", vInput, 0.1f, Time.deltaTime);
            animator.SetFloat("hzInput", hzInput, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("vInput", 0);
            animator.SetFloat("hzInput", 0);
        }

        if (distance > viewDistance)
            Patrol();
        else if (distance > attackDistance)
            ChasePlayer();
        else
            AttackPlayer();
    }

    // === PATROL ===
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.speed = walkSpeed;

        if (!agent.hasPath)
            GoToNextPatrolPoint();

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPatrolPoint();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    
    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
    }


    void AttackPlayer()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0f;
        if (lookPos != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), 5f * Time.deltaTime);

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            animator.SetTrigger("Attack");

            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damageAmount);
        }
    }

    
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        isDead = true;
        agent.isStopped = true;

        animator.SetTrigger("FallingBackDeath");

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(gameObject, 4f);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlayerBullet"))
        {
            TakeDamage(50f);
            Destroy(other.gameObject);
        }
    }
}
