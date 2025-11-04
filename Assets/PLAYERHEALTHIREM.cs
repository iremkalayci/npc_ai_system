using UnityEngine;
using UnityEngine.SceneManagement;

public class PLAYERHEALTHIREM: MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Death Settings")]
    public bool isDead = false;
    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Ölüm sonrası yeniden doğma (test için)
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ✅ Hasar alma
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"[PlayerHealth] Oyuncu hasar aldı: -{amount} HP (Kalan: {currentHealth})");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"[PlayerHealth] Can yenilendi: +{amount} HP (Toplam: {currentHealth})");
    }

    
    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PlayerHealth] Oyuncu öldü!");
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        // Hareket ve atışı kapat
        var controller = GetComponent<CharacterController>();
        if (controller) controller.enabled = false;

        var gun = GetComponentInChildren<WeaponAmmo>();
        if (gun) gun.enabled = false;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pumpkin"))
        {
            Heal(25f);
            Destroy(other.gameObject);
        }
    }
}
