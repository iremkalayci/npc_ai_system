using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerBullet : MonoBehaviour
{
    [Header("Bullet")]
    public float damage = 25f;
    public float lifetime = 3f;

    [Header("Sweep (miss fix)")]
    public bool enableSweepCheck = true;
    public float sweepRadius = 0.08f;      // mermi çapına göre ayarla (capsule/sphere ise yarıçap)
    public LayerMask hitMask;              // Enemy katman(lar)ı

    private Rigidbody rb;
    private Vector3 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var col = GetComponent<Collider>();
        col.isTrigger = true; // trigger üstünden yakalıyoruz

        // Varsayılan olarak tüm maskeyi açık bırakma; Inspector'dan Enemy katmanını seç
        if (hitMask == 0) hitMask = ~0;

        lastPos = transform.position;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (!enableSweepCheck) { lastPos = transform.position; return; }

        Vector3 currentPos = transform.position;
        Vector3 delta = currentPos - lastPos;
        float dist = delta.magnitude;

        if (dist > 0f)
        {
            // Geride kalan çizgi boyunca tarama
            RaycastHit hit;
            Vector3 dir = delta / dist;

            // SphereCast: mermin inkalınlığı kadar kalın çizgi
            if (Physics.SphereCast(lastPos, sweepRadius, dir, out hit, dist, hitMask, QueryTriggerInteraction.Collide))
            {
                // EnemyHealth'i child'ta olsa bile bul
                var enemy = hit.collider.GetComponentInParent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    // Mermiyi çarpma noktasına taşı (opsiyonel görsel için)
                    transform.position = hit.point;
                    Destroy(gameObject);
                    return;
                }
            }
        }

        lastPos = currentPos;
    }

    // Trigger üzerinden yakala (önerilen yol)
    void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Player / kendi mermisi vs. değilse yok et (opsiyonel)
        if (!other.CompareTag("Player") && !other.CompareTag("PlayerBullet"))
        {
            Destroy(gameObject);
        }
    }

    // Eğer collider'ı yanlışlıkla isTrigger=OFF yaptıysan yine yakalasın
    void OnCollisionEnter(Collision other)
    {
        var enemy = other.collider.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
