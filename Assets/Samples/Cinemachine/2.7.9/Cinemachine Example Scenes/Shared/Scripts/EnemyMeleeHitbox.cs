using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class EnemyMeleeHitbox : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 20f;
    public float hitCooldown = 0.3f;   // Aynı hedefe saniyede kaç kez vurabileceği

    [Header("Filtreler")]
    public string playerTag = "Player"; // Oyuncu objesinin Tag'ı
    public LayerMask playerLayers;      // Player layer'ını işaretle (Inspector’dan)

    [Header("Aktiflik")]
    public bool activeOnStart = false;  // Test için açmak istersen
    private bool _active = false;
    private float _lastHitTime = -999f;

    void Awake()
    {
        _active = activeOnStart;

        // Güvenlik: Üzerindeki collider mutlaka trigger olsun
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    // Animasyon Event'lerinden çağrılacak
    public void ActivateHitbox()  { _active = true;  }
    public void DeactivateHitbox(){ _active = false; }

    void OnTriggerEnter(Collider other)
    {
        if (!_active) return;
        if (Time.time - _lastHitTime < hitCooldown) return;

        // Layer filtresi varsa uygula
        if (playerLayers.value != 0)
        {
            if (((1 << other.gameObject.layer) & playerLayers.value) == 0)
                return;
        }

        // Tag filtresi (opsiyonel ama ikinci emniyet)
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag))
            return;

        // Oyuncudaki DamageablePlayer'ı bul ve hasar uygula
        var dmg = other.GetComponentInParent<DamageablePlayer>();
        if (dmg != null)
        {
            dmg.ApplyDamage(damage);
            _lastHitTime = Time.time;
        }
    }
}
