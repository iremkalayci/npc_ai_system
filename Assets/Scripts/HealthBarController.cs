using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
    [Header("UI Bağlantıları")]
    public Image healthFill;                     // Yeşil bar (fill)
    public TextMeshProUGUI healthText;           // 100 yazısı
    public TextMeshProUGUI gameOverText;         // Game Over yazısı

    [Header("Karakter Referansı")]
    public GameObject playerCharacter;           // Karakter objesi (Animasyon ya da model)

    private float targetHealth = 100f;
    private float currentHealth = 100f;
    private float lerpSpeed = 2f;

    private Color fullColor = Color.green;
    private Color lowColor = Color.red;
    private bool isDead = false;

    void Start()
    {
        // Game Over yazısını gizle
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Yumuşak geçiş
        currentHealth = Mathf.Lerp(currentHealth, targetHealth, Time.deltaTime * lerpSpeed);
        healthFill.fillAmount = currentHealth / 100f;
        healthText.text = Mathf.RoundToInt(currentHealth).ToString();

        // Renk geçişi
        if (currentHealth < 60f)
            healthFill.color = Color.Lerp(healthFill.color, lowColor, Time.deltaTime * 5f);
        else
            healthFill.color = Color.Lerp(healthFill.color, fullColor, Time.deltaTime * 5f);

        // Test için: H tuşuna basınca 10 damage al
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }

        // 0 olduğunda karakter yere düşsün (targetHealth'e göre kontrol)
        if (!isDead && targetHealth <= 0f)
        {
            Debug.Log("💀 HandleDeath() çağrıldı!");
            StartCoroutine(HandleDeath());
        }
    }

    public void TakeDamage(float amount)
    {
        targetHealth = Mathf.Clamp(targetHealth - amount, 0, 100);
    }

    private IEnumerator HandleDeath()
    {
        isDead = true;

        if (playerCharacter != null)
        {
            // Rigidbody kontrolü
            Rigidbody rb = playerCharacter.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                Debug.Log("⚙️ Rigidbody aktif edildi (karakter düşecek).");
            }
            else
            {
                Debug.LogWarning("⚠️ Karakterde Rigidbody bulunamadı!");
            }

            // Animator kontrolü
            Animator anim = playerCharacter.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Die");
                Debug.Log("💀 Death animasyonu tetiklendi!");
            }
            else
            {
                Debug.LogWarning("⚠️ Animator bulunamadı, ölüm animasyonu oynatılmadı!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ playerCharacter atanmadı!");
        }

        // 3 saniye bekle (karakter yerde kalıyor)
        yield return new WaitForSeconds(3f);

        // Game Over yazısını göster
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER";
        }

        // 1 saniye sonra karakteri devre dışı bırak
        yield return new WaitForSeconds(1f);

        if (playerCharacter != null)
        {
            playerCharacter.SetActive(false); // Destroy yerine devre dışı
            Debug.Log("☠️ Karakter devre dışı bırakıldı (Game Over).");
        }
    }
}
