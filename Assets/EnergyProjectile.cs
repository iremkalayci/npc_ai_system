using UnityEngine;

public class EnergyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 5f;
    public GameObject explosionEffect;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other)
    {
        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("💥 Player hit by energy blast!");
            // PlayerHealth varsa buraya damage ekle
        }

        Destroy(gameObject);
    }
}
