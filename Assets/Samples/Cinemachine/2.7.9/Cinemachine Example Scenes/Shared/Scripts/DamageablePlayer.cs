using UnityEngine;

public class DamageablePlayer : MonoBehaviour
{
    [Header("UI Health")]
    public HealthBarController uiHealth;     // Canvas'taki HealthBarController

    [Header("Hasar Ayarı")]
    public float defaultDamage = 10f;
    public float hitCooldown   = 0.30f;      // aynı anda birden fazla vuruşu filtreler

    [Header("Filtreler (tag/layer)")]
    // Tag kontrolünü istiyorsan kullan; şart değil
    public string[] enemyTags = { "EnemyWeapon", "EnemyProjectile" };

    // BURAYA EnemyWeapon (+ varsa EnemyBullet) layer'larını işaretle
    public LayerMask damageLayers; 

    private float _lastHitTime = -999f;

    // ---- Dışarıdan direkt hasar vermek için (örn. mermi scripti çağırabilir) ----
    public void ApplyDamage(float amount)
    {
        if (uiHealth == null) return;
        if (Time.time - _lastHitTime < hitCooldown) return;

        _lastHitTime = Time.time;
        uiHealth.TakeDamage(amount);
    }

    // ---- Çarpışma yakalama ----
    private void OnTriggerEnter(Collider other)
    {
        TryHit(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHit(collision.collider.gameObject);
    }

    // ---- Ortak kontrol ----
    private void TryHit(GameObject source)
    {
        // Cooldown
        if (Time.time - _lastHitTime < hitCooldown) return;

        // Layer filtresi (DOĞRU kullanım: damageLayers.value)
        if (damageLayers.value != 0)
        {
            if (((1 << source.layer) & damageLayers.value) == 0)
                return;
        }

        // Tag filtresi (opsiyonel ek emniyet)
        for (int i = 0; i < enemyTags.Length; i++)
        {
            if (!string.IsNullOrEmpty(enemyTags[i]) && source.CompareTag(enemyTags[i]))
            {
                ApplyDamage(defaultDamage);
                return;
            }
        }

        // Tag kullanmıyorsan sadece layer yeterli
        if (damageLayers.value != 0)
            ApplyDamage(defaultDamage);
    }
}
