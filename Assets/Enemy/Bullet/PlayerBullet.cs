using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 25f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy == null)
                enemy = other.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"enemy vuruldu -{damage} HP");
            }

            Destroy(gameObject);
            return;
        }

        
        if (!other.CompareTag("Player") && !other.CompareTag("PlayerBullet"))
        {
            Destroy(gameObject);
        }
    }
}
