using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class EnemyMeleeHitbox : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 20f;
    public float hitCooldown = 0.3f;   

    [Header("Filtreler")]
    public string playerTag = "Player"; 
    public LayerMask playerLayers;      

    [Header("Aktiflik")]
    public bool activeOnStart = false;  
    private bool _active = false;
    private float _lastHitTime = -999f;

    void Awake()
    {
        _active = activeOnStart;

        
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    
    public void ActivateHitbox()  { _active = true;  }
    public void DeactivateHitbox(){ _active = false; }

    void OnTriggerEnter(Collider other)
    {
        if (!_active) return;
        if (Time.time - _lastHitTime < hitCooldown) return;

        
        if (playerLayers.value != 0)
        {
            if (((1 << other.gameObject.layer) & playerLayers.value) == 0)
                return;
        }

        
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag))
            return;

        
        var dmg = other.GetComponentInParent<DamageablePlayer>();
        if (dmg != null)
        {
            dmg.ApplyDamage(damage);
            _lastHitTime = Time.time;
        }
    }
}
