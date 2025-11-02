using UnityEngine;
using TMPro;

public class PlayerAmmo : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmoInMag = 50;       // Şarjör kapasitesi
    public int currentAmmoInMag;        // Şu anki şarjör
    public int totalReserveAmmo = 150;  // Toplam yedek mermi
    public float reloadTime = 2f;       // Reload süresi
    public bool isReloading = false;    // Şu anda reload yapıyor mu?

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Animator")]
    public Animator playerAnimator;     // Reload animasyonu için

    void Start()
    {
        currentAmmoInMag = maxAmmoInMag;
        UpdateAmmoUI();
    }

    void Update()
    {
        // 🔹 Eğer reload ediyorsa bile hareket/ateş kodları donmasın
        // sadece ateş kısmını kontrol et
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            if (currentAmmoInMag > 0)
            {
                Shoot();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }

        // 🔹 Manuel reload (R tuşu)
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        currentAmmoInMag--;
        UpdateAmmoUI();
    }

    System.Collections.IEnumerator Reload()
    {
        if (totalReserveAmmo <= 0 || currentAmmoInMag == maxAmmoInMag)
            yield break; // Mermi yoksa veya zaten doluysa reload yapma

        isReloading = true;
        Debug.Log("Reloading...");

        // 🔹 Animasyonu tetikle
        if (playerAnimator != null)
        {
            // Her durumda (Idle, Run, Crouch) çalışır
            playerAnimator.ResetTrigger("Reload");
            playerAnimator.SetTrigger("Reload");
        }

        // 🔹 Animasyon süresi kadar bekle
        yield return new WaitForSeconds(reloadTime);

        // 🔹 Eksik mermileri doldur
        int neededAmmo = maxAmmoInMag - currentAmmoInMag;
        int ammoToLoad = Mathf.Min(neededAmmo, totalReserveAmmo);

        currentAmmoInMag += ammoToLoad;
        totalReserveAmmo -= ammoToLoad;

        UpdateAmmoUI();

        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        ammoText.text = $"Ammo: {currentAmmoInMag} / {totalReserveAmmo}";
    }

    void OnEnable()
    {
        // 🔹 Oyun yeniden başlayınca değerleri sıfırla
        currentAmmoInMag = maxAmmoInMag;
        totalReserveAmmo = 150;
        UpdateAmmoUI();
        isReloading = false;
    }
}
