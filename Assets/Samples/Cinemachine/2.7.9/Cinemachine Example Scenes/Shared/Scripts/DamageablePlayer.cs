using UnityEngine;

public class DamageablePlayer : MonoBehaviour
{
    [Header("UI Health")]
    public HealthBarController uiHealth;    

    [Header("Hasar Ayarı")]
    public float defaultDamage = 10f;
    public float hitCooldown   = 0.30f;     

    [Header("Filtreler (tag/layer)")]
    
    public string[] enemyTags = { "EnemyWeapon", "EnemyProjectile" };

    
    public LayerMask damageLayers; 

    private float _lastHitTime = -999f;

    
    public void ApplyDamage(float amount)
    {
        if (uiHealth == null) return;
        if (Time.time - _lastHitTime < hitCooldown) return;

        _lastHitTime = Time.time;
        uiHealth.TakeDamage(amount);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        TryHit(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHit(collision.collider.gameObject);
    }

   
    private void TryHit(GameObject source)
    {
        
        if (Time.time - _lastHitTime < hitCooldown) return;

        
        if (damageLayers.value != 0)
        {
            if (((1 << source.layer) & damageLayers.value) == 0)
                return;
        }

       
        for (int i = 0; i < enemyTags.Length; i++)
        {
            if (!string.IsNullOrEmpty(enemyTags[i]) && source.CompareTag(enemyTags[i]))
            {
                ApplyDamage(defaultDamage);
                return;
            }
        }

        
        if (damageLayers.value != 0)
            ApplyDamage(defaultDamage);
    }
}
