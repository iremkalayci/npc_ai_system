using UnityEngine;
using System;

public class WeaponAmmoReload : MonoBehaviour
{
    [Header("Ammo")]
    public int magSize = 30;
    public int currentMag = 30;
    public int reserveAmmo = 150;
    public int maxReserve = 300;

    [Header("Reload")]
    public float reloadTime = 1.8f;   
    public KeyCode reloadKey = KeyCode.R;
    [Tooltip("Şarjör 0 olduğunda otomatik reload yap")]
    public bool autoReloadOnEmpty = true;

    [Header("Animator (opsiyonel)")]
    public Animator animator;
    public string reloadTrigger = "Reload";
    public string isReloadingBool = "IsReloading";

    [Header("Ses (opsiyonel)")]
    public AudioSource reloadSFX;
    public AudioSource emptyClickSFX;

    [Header("Tanı / Debug")]
    public bool debugLogs = true;     
    public bool logStackTraces = true;

    public bool IsReloading { get; private set; }

    public event Action<int,int,int> OnAmmoChanged;

    
    bool manualReloadArmed = false;

    void Awake()
    {
        currentMag  = Mathf.Clamp(currentMag, 0, magSize);
        reserveAmmo = Mathf.Clamp(reserveAmmo, 0, maxReserve);
        Notify();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(reloadKey))
        {
            manualReloadArmed = true;          
            if (debugLogs) Debug.Log($"[Ammo] R pressed (cur={currentMag}, res={reserveAmmo})");
            TryReloadInternal(manual: true, caller: "R-Key");
        }

        
        manualReloadArmed = false;
    }

    
    public bool TryConsume(int amount)
    {
        if (IsReloading) return false;
        if (amount <= 0) return false;

        if (currentMag >= amount)
        {
            currentMag -= amount;
            Notify();

            if (autoReloadOnEmpty && currentMag == 0 && reserveAmmo > 0)
            {
                if (debugLogs) Debug.Log("[Ammo] Auto-reload (cur=0).");
                TryReloadInternal(manual: false, caller: "Auto(0)");
            }
            return true;
        }

        
        if (reserveAmmo > 0)
        {
            if (debugLogs) Debug.Log("[Ammo] Mag empty while firing; try auto.");
            TryReloadInternal(manual: false, caller: "Auto(FireEmpty)");
        }
        else
        {
            if (emptyClickSFX) emptyClickSFX.Play();
            if (debugLogs) Debug.Log("[Ammo] Empty click (no reserve).");
        }

        return false;
    }

    
    
    public bool TryReload(bool force)
    {
        bool manualIntent = force && manualReloadArmed;
        string who = "ExternalTryReload" + (force ? "(force)" : "");
        return TryReloadInternal(manualIntent, who);
    }

    bool TryReloadInternal(bool manual, string caller)
    {
        if (IsReloading) return false;
        if (reserveAmmo <= 0) return false;
        if (currentMag >= magSize) return false;

        
        if (!manual && currentMag > 0)
        {
            if (debugLogs)
            {
                string msg = $"[Ammo] BLOCK auto-reload at cur={currentMag} (caller={caller}). Only at 0.";
                if (logStackTraces) msg += "\n" + Environment.StackTrace;
                Debug.LogWarning(msg);
            }
            return false;
        }

        if (manual && !manualReloadArmed)
        {
            
            if (debugLogs)
            {
                string msg = $"[Ammo] BLOCK manual (not R-frame). cur={currentMag} caller={caller}";
                if (logStackTraces) msg += "\n" + Environment.StackTrace;
                Debug.LogWarning(msg);
            }
            return false;
        }

        StartCoroutine(ReloadRoutine(caller));
        return true;
    }

    System.Collections.IEnumerator ReloadRoutine(string caller)
    {
        IsReloading = true;

        if (debugLogs) Debug.Log($"[Ammo] >>> RELOAD START by {caller} (cur={currentMag}, res={reserveAmmo})");

        if (animator)
        {
            if (!string.IsNullOrEmpty(isReloadingBool)) animator.SetBool(isReloadingBool, true);
            if (!string.IsNullOrEmpty(reloadTrigger))    animator.SetTrigger(reloadTrigger);
        }
        if (reloadSFX) reloadSFX.Play();

        yield return new WaitForSeconds(reloadTime);

        int need   = magSize - currentMag;
        int toLoad = Mathf.Clamp(need, 0, reserveAmmo);

        currentMag  += toLoad;
        reserveAmmo -= toLoad;
        Notify();

        if (animator && !string.IsNullOrEmpty(isReloadingBool)) animator.SetBool(isReloadingBool, false);
        IsReloading = false;

        if (debugLogs) Debug.Log($"[Ammo] <<< RELOAD DONE (cur={currentMag}, res={reserveAmmo})");
    }

    void Notify()
    {
        OnAmmoChanged?.Invoke(currentMag, magSize, reserveAmmo);
    }
}
