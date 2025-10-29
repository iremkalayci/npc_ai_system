using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    ActionBaseState currentState;

    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();

    public GameObject currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    [HideInInspector] public Animator anim;

    public MultiAimConstraint RHandAim;
    public TwoBoneIKConstraint LHandIK;

    void Start()
    {
        ammo = currentWeapon.GetComponent<WeaponAmmo>();
        anim = GetComponentInChildren<Animator>();

        // IK otomatik atama (opsiyonel)
        if(RHandAim == null) RHandAim = currentWeapon.GetComponentInChildren<MultiAimConstraint>();
        if(LHandIK == null) LHandIK = currentWeapon.GetComponentInChildren<TwoBoneIKConstraint>();

        // IK başlangıç weight
        RHandAim.weight = 1f;
        LHandIK.weight = 1f;

        SwitchState(Default);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void ReloadWeapon()
    {
        ammo.Reload();
    }
}
