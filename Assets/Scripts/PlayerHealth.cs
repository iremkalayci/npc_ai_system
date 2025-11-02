using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI Elemanları")]
    public Image healthFill;  // HealthFill (Image Type = Filled, Fill Method = Horizontal)

    [Header("Health Ayarları")]
    public float maxHealth = 100f;

    private float currentHealth;
    public float CurrentHealth => currentHealth;

    void Start()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        SetHealth(currentHealth - amount);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        SetHealth(currentHealth + amount);
    }

    public void SetHealth(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, maxHealth);
        if (!Mathf.Approximately(clamped, currentHealth))
        {
            currentHealth = clamped;
            UpdateHealthUI();
            // if (currentHealth <= 0f) OnDeath();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthFill == null) return;

        float fillAmount = currentHealth / maxHealth;
        healthFill.fillAmount = fillAmount;

        if (fillAmount > 0.5f)
            healthFill.color = Color.Lerp(Color.yellow, Color.green, (fillAmount - 0.5f) * 2f);
        else
            healthFill.color = Color.Lerp(Color.red, Color.yellow, fillAmount * 2f);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        if (healthFill != null)
        {
            if (healthFill.type != Image.Type.Filled)
                healthFill.type = Image.Type.Filled;
            if (healthFill.fillMethod != Image.FillMethod.Horizontal)
                healthFill.fillMethod = Image.FillMethod.Horizontal;

            float preview = Mathf.Clamp(currentHealth <= 0f ? maxHealth : currentHealth, 0f, maxHealth) / maxHealth;
            healthFill.fillAmount = preview;

            if (preview > 0.5f)
                healthFill.color = Color.Lerp(Color.yellow, Color.green, (preview - 0.5f) * 2f);
            else
                healthFill.color = Color.Lerp(Color.red, Color.yellow, preview * 2f);
        }
    }
#endif
}
