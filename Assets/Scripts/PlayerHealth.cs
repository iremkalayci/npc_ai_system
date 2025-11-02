using UnityEngine;
using UnityEngine.UI;

namespace Karakter2
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("UI Elemanları")]
        public Image healthFill;  // HealthFill image

        [Header("Health Ayarları")]
        public float maxHealth = 100f;

        [SerializeField] private float currentHealth;
        public float CurrentHealth => currentHealth;

        void Awake()
        {
            // İlk değer ayarı
            currentHealth = (currentHealth <= 0f) ? maxHealth : Mathf.Clamp(currentHealth, 0f, maxHealth);
            UpdateHealthUI();
        }

        // Hasar
        public void TakeDamage(float amount)
        {
            if (amount <= 0f) return;
            currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
            UpdateHealthUI();
        }

        // İyileşme (opsiyonel)
        public void Heal(float amount)
        {
            if (amount <= 0f) return;
            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            if (!healthFill) return;

            float fillAmount = (maxHealth > 0f) ? currentHealth / maxHealth : 0f;
            healthFill.fillAmount = fillAmount;

            // Renk geçişi (yeşilden kırmızıya)
            if (fillAmount > 0.5f)
                healthFill.color = Color.Lerp(Color.yellow, Color.green, (fillAmount - 0.5f) * 2f);
            else
                healthFill.color = Color.Lerp(Color.red, Color.yellow, fillAmount * 2f);
        }
    }
}
