using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    private ActionBaseState currentState;

    public ReloadState  Reload  = new ReloadState();
    public DefaultState Default = new DefaultState();

    [Header("Silah / Bileşenler")]
    public GameObject currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    [HideInInspector] public Animator anim;
    private CharacterController cc;

    [Header("Rig/IK")]
    public MultiAimConstraint   RHandAim;          
    public TwoBoneIKConstraint  LHandIK;          
    [Tooltip("IK ağırlığının açılıp kapanma hızı")]
    public float ikBlendSpeed = 10f;
    [Tooltip("IK sadece yerdeyken aktif olsun mu?")]
    public bool requireGroundedForIK = true;
    [Tooltip("Animator bool parametresi (nişan alma)")]
    public string aimingParam = "IsAiming";

    [Header("Aim Layer (Animator)")]
    [Tooltip("Animator'daki Aim layer index'i (0=Base). Aim genelde 1 olur).")]
    public int aimLayerIndex = 1;
    [Tooltip("Aim layer ağırlığını yumuşatmak için hız")]
    public float layerBlendSpeed = 10f;

    // dahili
    private float rAimW, lIkW; 

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        cc   = GetComponent<CharacterController>();

        if (currentWeapon != null)
        {
            ammo = currentWeapon.GetComponent<WeaponAmmo>();
            if (RHandAim == null) RHandAim = currentWeapon.GetComponentInChildren<MultiAimConstraint>();
            if (LHandIK  == null) LHandIK  = currentWeapon.GetComponentInChildren<TwoBoneIKConstraint>();
        }

     
        if (RHandAim) { rAimW = RHandAim.weight = 0f; }
        if (LHandIK)  { lIkW  = LHandIK.weight  = 0f; }

        
        if (anim && aimLayerIndex >= 0 && aimLayerIndex < anim.layerCount)
            anim.SetLayerWeight(aimLayerIndex, 0f);

        SwitchState(Default);
    }

    void Update()
    {
        
        currentState?.UpdateState(this);

        
        bool aiming   = anim != null && anim.GetBool(aimingParam);
        bool grounded = cc   != null ? cc.isGrounded : true;

        
        bool enableIK = aiming && (!requireGroundedForIK || grounded);
        float target  = enableIK ? 1f : 0f;

        float ikStep    = ikBlendSpeed     * Time.deltaTime;
        float layerStep = layerBlendSpeed  * Time.deltaTime;

        
        if (RHandAim)
        {
            rAimW = Mathf.MoveTowards(rAimW, target, ikStep);
            RHandAim.weight = rAimW;
        }
        if (LHandIK)
        {
            lIkW = Mathf.MoveTowards(lIkW, target, ikStep);
            LHandIK.weight = lIkW;
        }

        
        if (anim && aimLayerIndex >= 0 && aimLayerIndex < anim.layerCount)
        {
            float cur = anim.GetLayerWeight(aimLayerIndex);
            float nxt = Mathf.MoveTowards(cur, target, layerStep);
            anim.SetLayerWeight(aimLayerIndex, nxt);
        }
    }

    public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        currentState?.EnterState(this);
    }

    public void ReloadWeapon()
    {
        if (ammo != null) ammo.Reload();
    }
}
