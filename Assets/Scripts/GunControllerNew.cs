using UnityEngine;

public class GunControllerNew : MonoBehaviour
{
    [Header("Ateş Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public float fireRate = 0.25f; // saniyede 4 atış

    [Header("Referanslar")]
    public Transform worldCrossPlus; // WorldCrossPlus objesinin Transform'u

    [Header("Ses ve Efekt Ayarları")]
    public AudioSource fireSound;      // 🔊 Silah sesi
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

        // 🎯 Hedef: WorldCrossPlus (Inspector'dan bağla; yoksa yedek Find)
        Transform cross = worldCrossPlus;
        if (cross == null)
        {
            GameObject go = GameObject.Find("WorldCrossPlus");
            if (go != null) cross = go.transform;
        }

        Vector3 targetPoint = (cross != null)
            ? cross.position
            : firePoint.position + firePoint.forward * 100f; // yedek yön

        // 📍 Mermiyi cross yönüne hizala
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction, Vector3.up)
        );

        // Hız ver
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = direction * bulletSpeed;

        // Otomatik temizleme
        Destroy(bullet, 3f);
    }
}
