using UnityEngine;

public class GunControllerNew : MonoBehaviour
{
    [Header("Ateş Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public float fireRate = 0.25f; 
    [Header("Referanslar")]
    public Transform worldCrossPlus; 
    public WeaponAmmoReload ammo;    

    [Header("Ses ve Efekt Ayarları")]
    public AudioSource fireSound;     
    public ParticleSystem muzzleFlash;

    private float nextFireTime = 0f;

    void Awake()
    {
        
        if (ammo == null)
        {
            ammo = GetComponent<WeaponAmmoReload>();
            if (ammo == null) ammo = GetComponentInParent<WeaponAmmoReload>();
        }
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            
            if (ammo != null && !ammo.TryConsume(1))
                return;

            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }

       
        if (fireSound != null) fireSound.Play();

        Transform cross = worldCrossPlus;
        if (cross == null)
        {
            GameObject go = GameObject.Find("WorldCrossPlus");
            if (go != null) cross = go.transform;
        }

        Vector3 targetPoint = (cross != null)
            ? cross.position
            : firePoint.position + firePoint.forward * 100f; 

       
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction, Vector3.up)
        );

      
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = direction * bulletSpeed;

        
        Destroy(bullet, 3f);
    }
}
