using UnityEngine;

public class BulletDamageTrigger : MonoBehaviour
{
    public float damage = 25f;

    // Oyuncuya çarpmayı veya kendi silahına çarpmayı önlemek istersen
    [SerializeField] private string[] ignoreTags = { "Player" };

    void OnTriggerEnter(Collider other)
    {
        // İstenmeyen çarpmaları ele
        foreach (var t in ignoreTags)
        {
            if (other.CompareTag(t)) return;
        }

        // NPC sağlığı var mı?
        var hp = other.GetComponentInParent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        // Mermiyi yok et
        Destroy(gameObject);
    }
}
