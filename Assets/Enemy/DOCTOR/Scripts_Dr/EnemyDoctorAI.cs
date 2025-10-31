using UnityEngine;
using UnityEngine.AI;

public class EnemyDoctorAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public Transform firePoint;
    public GameObject virusPrefab;

    [Header("Detection Settings")]
    public float viewDistance = 20f;
    public float attackDistance = 10f;
    public float fireRate = 1.5f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private float nextFireTime = 0f;
    private bool playerDetected = false;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= viewDistance)
        {
            playerDetected = true;
        }

        if (playerDetected)
        {
            if (distance > attackDistance)
            {
                ChasePlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            Patrol();
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Patrol()
    {
        agent.isStopped = false;
        agent.speed = 2.5f;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = 4.5f;
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        transform.LookAt(player);

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireVirus();
        }
    }

    void FireVirus()
    {
        if (virusPrefab == null || firePoint == null) return;

        GameObject virus = Instantiate(virusPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = virus.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * 15f;

        Destroy(virus, 5f);
    }
}
