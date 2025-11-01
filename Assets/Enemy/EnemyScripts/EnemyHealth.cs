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
    private Collider col;
    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        aiScript = GetComponent<EnemyAI>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(float amount)
    {
        // ✅ Eğer zaten ölmüşse hiçbir şey yapma
        if (isDead) return;

        currentHealth -= amount;

        // ✅ Sıfırın altına düşmeyi engelle (görsel bug olmasın)
        currentHealth = Mathf.Max(currentHealth, 0f);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // ✅ Ölüm bir kere çalışsın
        if (isDead) return;
        isDead = true;

        Debug.Log("Enemy died 🧟‍♂️");

        // 🔹 Animasyon tetikle
        animator.SetBool("isDead", true);

        // 🔹 Hareketi durdur
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // 🔹 AI scriptini kapat
        if (aiScript != null)
            aiScript.enabled = false;

        // 🔹 Fizik ve çarpışmayı kapat (mermiler artık etkilemesin)
        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        // 🔹 3 saniye sonra yok et
        Destroy(gameObject, 3f);
    }
}
