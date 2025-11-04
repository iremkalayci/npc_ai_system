using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float extraStayAfterAnim = 0.25f;   // anim bitince fazladan bekleme
    private float currentHealth;
    private bool isDead = false;

    [Header("Animator Ayarları")]
    [Tooltip("Ölüm tetikleyici (Trigger) kullanıyorsan doldur. Boşsa bool kullanılacak.")]
    public string deathTriggerName = "Die";      // Animator'da Trigger varsa yaz
    [Tooltip("Ölüm bool parametresi kullanıyorsan doldur.")]
    public string deathBoolName = "isDead";      // Animator'da Bool varsa yaz
    [Tooltip("Ölüm state adı (örn: Base Layer.Death) – otomatik süre ölçümü için.")]
    public string deathStateFullPath = "Base Layer.Death";  // Animator'daki tam yol
    public float fallbackDeathAnimTime = 1.0f;   // State bulunamazsa bu kadar bekle

    // Bileşenler
    private Animator animator;
    private NavMeshAgent agent;
    private EnemyAI aiScript;
    private Collider[] allColliders;
    private Rigidbody[] allRigidbodies;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        agent     = GetComponent<NavMeshAgent>();
        aiScript  = GetComponent<EnemyAI>();
        allColliders   = GetComponentsInChildren<Collider>(true);
        allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        if (currentHealth <= 0f)
            Die();
        // burada hit reaksiyonu ekleyebilirsin
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // AI/Agent durdur
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        if (aiScript != null) aiScript.enabled = false;

        // Çarpışmaları kapat, fizik dondur
        if (allColliders != null) foreach (var c in allColliders) if (c) c.enabled = false;
        if (allRigidbodies != null)
        {
            foreach (var r in allRigidbodies)
            {
                if (!r) continue;
                r.isKinematic = true;
                r.linearVelocity = Vector3.zero;        // DÜZELTME
                r.angularVelocity = Vector3.zero;
            }
        }

        // Ölüm animasyonu tetikle
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(deathTriggerName))
                animator.SetTrigger(deathTriggerName);

            if (!string.IsNullOrEmpty(deathBoolName))
                animator.SetBool(deathBoolName, true);
        }

        // Animasyonun bitmesini bekle ve sonra temizle
        StartCoroutine(WaitDeathAndCleanup());
    }

    private IEnumerator WaitDeathAndCleanup()
    {
        float waitTime = fallbackDeathAnimTime;

        if (animator != null && !string.IsNullOrEmpty(deathStateFullPath))
        {
            // Önce state'e geçmesini bekle
            float t = 0f;
            while (t < 1f)
            {
                var st = animator.GetCurrentAnimatorStateInfo(0);
                if (st.fullPathHash == Animator.StringToHash(deathStateFullPath))
                {
                    // geçer geçmez state uzunluğunu al
                    waitTime = st.length;
                    break;
                }
                t += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(waitTime + extraStayAfterAnim);
        Destroy(gameObject);
    }
}
