using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private bool semiAuto = true;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject playerBullet; // 🎯 PlayerBullet prefab
    [SerializeField] private Transform firePoint;     // 🔥 FirePoint transform
    [SerializeField] private float bulletVelocity = 50f;
    [SerializeField] private int bulletsPerShot = 1;

    [Header("References")]
    [SerializeField] private AimStateManager aim;
    [SerializeField] private AudioClip gunShot;

    private AudioSource audioSource;
    private WeaponAmmo ammo;
    private float fireRateTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        aim = GetComponentInParent<AimStateManager>();
        ammo = GetComponent<WeaponAmmo>();
        fireRateTimer = fireRate;
    }

    void Update()
    {
        if (ShouldFire()) Fire();
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;

        if (fireRateTimer < fireRate)
            return false;

        if (ammo != null && ammo.currentAmmo == 0)
            return false;

        if (semiAuto && Input.GetMouseButtonDown(0))
            return true;

        if (!semiAuto && Input.GetMouseButton(0))
            return true;

        return false;
    }

    void Fire()
    {
        if (playerBullet == null || firePoint == null)
        {
            Debug.LogError("❌ PlayerBullet veya FirePoint eksik!");
            return;
        }

        fireRateTimer = 0;

        if (gunShot != null && audioSource != null)
            audioSource.PlayOneShot(gunShot);

        if (ammo != null)
            ammo.currentAmmo--;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject bullet = Instantiate(playerBullet, firePoint.position, firePoint.rotation);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 shootDir = (aim != null)
                    ? (aim.aimPos.position - firePoint.position).normalized
                    : firePoint.forward; // aim null olursa ileriye atar

                rb.AddForce(shootDir * bulletVelocity, ForceMode.Impulse);
            }
        }

        Debug.Log("🔫 Mermi ateşlendi!");
    }
}
