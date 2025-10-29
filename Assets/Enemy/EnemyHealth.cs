using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private Animator animator;
    private NavMeshAgent agent;
    private EnemyAI aiScript; // mevcut AI scriptine referans

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        aiScript = GetComponent<EnemyAI>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Animator'da ölüm animasyonunu tetikle
        animator.SetBool("isDead", true);

        // AI hareketini durdur
        if (agent != null) agent.isStopped = true;
        if (aiScript != null) aiScript.enabled = false;

        // Rigidbody varsa devre dışı bırak (isteğe bağlı)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Collider'ı kapat (mermi vs. etkilemesin)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 5 saniye sonra yok olsun
        Destroy(gameObject, 5f);
    }
}
