using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Player'a çarpınca
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph == null)
                ph = other.GetComponentInParent<PlayerHealth>();

            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log($"💥 Player vuruldu! -{damage} HP");
            }

            Destroy(gameObject);
            return;
        }

        // Diğer objelere çarpınca
        if (!other.CompareTag("Enemy") && !other.CompareTag("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }
}
