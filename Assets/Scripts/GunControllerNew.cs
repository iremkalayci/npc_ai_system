using UnityEngine;

public class GunControllerNew : MonoBehaviour
{
    [Header("Ateş Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public float fireRate = 0.25f; // saniyede 4 atış

    [Header("Ses ve Efekt Ayarları")]
    public AudioSource fireSound; // 🔊 Silah sesi
    public ParticleSystem muzzleFlash; // 💥 Namlu flaşı

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 💥 Flaş efekti
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }

        // 🔊 Ses
        if (fireSound != null) fireSound.Play();

        // 🎯 Cross'un pozisyonunu hedef al
        GameObject cross = GameObject.Find("WorldCrossPlus");
        Vector3 targetPoint;

        if (cross != null)
            targetPoint = cross.transform.position;
        else
            targetPoint = firePoint.position + firePoint.forward * 100f; // yedek yön

        // 📍 Mermiyi cross yönüne hizala
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = direction * bulletSpeed;

        Destroy(bullet, 3f);
    }
}
