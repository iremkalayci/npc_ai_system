using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
    [Header("UI Bağlantıları")]
    public Image healthFill;                    
    public TextMeshProUGUI healthText;           
    public TextMeshProUGUI gameOverText;        

    [Header("Karakter Referansı")]
    public GameObject playerCharacter;           
    public PlayerHealth playerHealth;           

    [Header("Görsel Ayarlar")]
    [Min(0.1f)] public float lerpSpeed = 2f;
    public Color fullColor = Color.green;
    public Color lowColor = Color.red;

    private float displayHealth;
    private float maxHealth = 100f;
    private bool isDeathSequenceStarted = false;

    void Awake()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void Start()
    {
        
        if (playerHealth == null)
        {
            if (playerCharacter != null)
                playerHealth = playerCharacter.GetComponentInChildren<PlayerHealth>();

            
#if UNITY_2023_1_OR_NEWER
            if (playerHealth == null)
                playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
#else
            if (playerHealth == null)
                playerHealth = FindObjectOfType<PlayerHealth>();
#endif
        }

        if (playerHealth != null)
        {
            
            playerHealth.OnHealthChanged += HandleHealthChanged;
            playerHealth.OnDied += HandleDied;

           
            maxHealth = Mathf.Max(1f, playerHealth.maxHealth);
            displayHealth = playerHealth.CurrentHealth;

            
            ApplyUI(displayHealth, maxHealth, instant: true);
        }
        else
        {
            Debug.LogWarning("⚠️ HealthBarController: PlayerHealth bulunamadı!");
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
            playerHealth.OnDied -= HandleDied;
        }
    }

    void Update()
    {
        if (playerHealth == null || healthFill == null) return;

       
        displayHealth = Mathf.Lerp(displayHealth, playerHealth.CurrentHealth, Time.deltaTime * lerpSpeed);
        ApplyUI(displayHealth, maxHealth, instant: false);

       
        if (Input.GetKeyDown(KeyCode.H) && !isDeathSequenceStarted)
        {
            playerHealth.TakeDamage(10f);
        }
    }

    public void TakeDamage(float amount)
    {
        if (playerHealth != null) playerHealth.TakeDamage(amount);
    }

    
    private void HandleHealthChanged(float current, float max)
    {
        maxHealth = Mathf.Max(1f, max);
       
    }

    private void HandleDied()
    {
        if (isDeathSequenceStarted) return;
        StartCoroutine(HandleDeathSequence());
    }

    
    private void ApplyUI(float current, float max, bool instant)
    {
        if (healthFill != null)
        {
            float pct = Mathf.Clamp01(max <= 0f ? 0f : current / max);
            healthFill.fillAmount = pct;
          
            healthFill.color = Color.Lerp(lowColor, fullColor, pct);
        }

        if (healthText != null)
        {
            healthText.text = Mathf.RoundToInt(current).ToString();
        }
    }

    
    private IEnumerator HandleDeathSequence()
    {
        isDeathSequenceStarted = true;

        if (playerCharacter != null)
        {
           
            Rigidbody rb = playerCharacter.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; 
            }

            
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

        
        yield return new WaitForSeconds(3f);

        
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER";
        }

        
        yield return new WaitForSeconds(1f);

        if (playerCharacter != null)
        {
            playerCharacter.SetActive(false);
            Debug.Log("☠️ Karakter devre dışı bırakıldı (Game Over).");
        }
    }
}
