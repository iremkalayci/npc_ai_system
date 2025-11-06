using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float extraStayAfterAnim = 0.25f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Animator Ayarları")]
    public string deathTriggerName = "Die";     
    public string deathBoolName    = "isDead";  
    public string deathStateFullPath = "Base Layer.Death";
    public float  fallbackDeathAnimTime = 1.0f;

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
        
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        if (aiScript != null) aiScript.enabled = false;

        if (allColliders != null) foreach (var c in allColliders) if (c) c.enabled = false;
        if (allRigidbodies != null)
        {
            foreach (var r in allRigidbodies)
            {
                if (!r) continue;
                r.isKinematic = true;
                r.linearVelocity = Vector3.zero;       
                r.angularVelocity = Vector3.zero;
            }
        }

        if (animator != null)
        {
            if (!string.IsNullOrEmpty(deathTriggerName))
                animator.SetTrigger(deathTriggerName);

            if (!string.IsNullOrEmpty(deathBoolName))
                animator.SetBool(deathBoolName, true);
        }

        StartCoroutine(WaitDeathAndCleanup());
    }

    private IEnumerator WaitDeathAndCleanup()
    {
        float waitTime = fallbackDeathAnimTime;

        if (animator != null && !string.IsNullOrEmpty(deathStateFullPath))
        {
            float t = 0f;
            int targetHash = Animator.StringToHash(deathStateFullPath);
            while (t < 1f)
            {
                var st = animator.GetCurrentAnimatorStateInfo(0);
                if (st.fullPathHash == targetHash)
                {
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

    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerBullet")) return;

        var pb = other.GetComponent<PlayerBullet>();
        float dmg = pb != null ? pb.damage : 25f;

        TakeDamage(dmg);
        Destroy(other.gameObject);
    }
}
