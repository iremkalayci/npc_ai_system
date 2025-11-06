using UnityEngine;

public class WeaponAttach : MonoBehaviour
{
    [Header("Silah Ayarları")]
    [Tooltip("Karakterin eline takılacak silah prefab'ı")]
    public GameObject weaponPrefab;

    [Tooltip("Karakterin elinde silahın bağlanacağı nokta (örnek: RightHand_Grip)")]
    public Transform weaponGrip;

    private GameObject currentWeapon;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        
        if (weaponGrip != null && weaponGrip.childCount > 0)
        {
            currentWeapon = weaponGrip.GetChild(0).gameObject;
            Debug.Log("⚙️ Mevcut silah bulundu: " + currentWeapon.name);
        }
        else
        {
            AttachWeapon(); 
        }

        
        if (animator != null)
            animator.SetBool("IsGunPlay", true);
        else
            Debug.LogWarning("⚠️ Animator bulunamadı! Silahlı animasyon tetiklenemedi.");
    }

    public void AttachWeapon()
    {
        if (weaponPrefab == null || weaponGrip == null)
        {
            Debug.LogWarning("⚠️ WeaponPrefab veya WeaponGrip atanmadı!");
            return;
        }

       
        if (currentWeapon != null)
        {
            Debug.Log("🟡 Silah zaten takılı: " + currentWeapon.name);
            return;
        }

        
        currentWeapon = Instantiate(weaponPrefab);
        currentWeapon.transform.SetParent(weaponGrip, false);

        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one * 0.05f;

        
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        
        Collider col = currentWeapon.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        Debug.Log("🔫 Silah başarıyla eklendi: " + currentWeapon.name);
    }

    public void RemoveWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;

            if (animator != null)
                animator.SetBool("IsGunPlay", false);

            Debug.Log("❌ Silah kaldırıldı.");
        }
    }
}
