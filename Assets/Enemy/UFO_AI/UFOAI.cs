using UnityEngine;
using System.Collections;

public class UFOAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    private int currentIndex = 0;

    [Header("Attack Settings")]
    public GameObject lightProjectilePrefab;
    public Transform firePoint;
    public float fireRate = 3f;
    public float projectileSpeed = 15f;
    public float damage = 5f; // düşük hasar
    private float nextFireTime = 0f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        Patrol();

        if (player != null && Vector3.Distance(transform.position, player.position) < 20f)
        {
            AttackPlayer();
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
            currentIndex = (currentIndex + 1) % waypoints.Length;
    }

    void AttackPlayer()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (lightProjectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(lightProjectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * projectileSpeed;

        Destroy(proj, 4f);
    }
}
