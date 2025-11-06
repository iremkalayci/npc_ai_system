using UnityEngine;

public class BulletDamageTrigger : MonoBehaviour
{
    public float damage = 25f;

    
    [SerializeField] private string[] ignoreTags = { "Player" };

    void OnTriggerEnter(Collider other)
    {
        
        foreach (var t in ignoreTags)
        {
            if (other.CompareTag(t)) return;
        }

        
        var hp = other.GetComponentInParent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        
        Destroy(gameObject);
    }
}
