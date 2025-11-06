using UnityEngine;
using TMPro;

public class PlayerAmmo : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmoInMag = 50;      
    public int currentAmmoInMag;       
    public int totalReserveAmmo = 150;  
    public float reloadTime = 2f;      
    public bool isReloading = false;   

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Animator")]
    public Animator playerAnimator;     

    void Start()
    {
        currentAmmoInMag = maxAmmoInMag;
        UpdateAmmoUI();
    }

    void Update()
    {
        
       
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
            yield break; 

        isReloading = true;
        Debug.Log("Reloading...");

       
        if (playerAnimator != null)
        {
           
            playerAnimator.ResetTrigger("Reload");
            playerAnimator.SetTrigger("Reload");
        }

        
        yield return new WaitForSeconds(reloadTime);

      
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
        
        currentAmmoInMag = maxAmmoInMag;
        totalReserveAmmo = 150;
        UpdateAmmoUI();
        isReloading = false;
    }
}
