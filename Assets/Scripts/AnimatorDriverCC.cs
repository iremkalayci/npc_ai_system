using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class AnimatorDriverCC : MonoBehaviour
{
    [Header("Animator Parametreleri")]
    public string speedParam = "Speed";        
    public string isGroundedParam = "IsGrounded";
    public string isRunningParam = "IsRunning";
    public string isAimingParam = "IsAiming";

    [Header("Hız Ayarları")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float smooth = 10f;

    [Header("İniş Sonrası Yardımcı Ayarlar")]
    public float landGraceTime = 0.15f;   
    public float inputKickThreshold = 0.1f; 
    public float minWalkKick01 = 0.18f;     
    public float minRunKick01 = 0.35f;      

    [Header("Aim Layer")]
    public int aimLayerIndex = 1;
    public float aimBlendSpeed = 12f;

    private CharacterController cc;
    private Animator animator;
    private float animSpeed;
    private float landTimer;      
    private bool wasGrounded;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animator.applyRootMotion = false;
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    void Update()
    {
        
        bool grounded = cc.isGrounded;

       
        if (grounded && !wasGrounded)
            landTimer = landGraceTime;
        else if (landTimer > 0f)
            landTimer -= Time.deltaTime;

        
        Vector3 horizVel = new Vector3(cc.velocity.x, 0f, cc.velocity.z);
        float speedMps = horizVel.magnitude;

        
        float ix = Input.GetAxisRaw("Horizontal");
        float iz = Input.GetAxisRaw("Vertical");
        float inputMag = new Vector2(ix, iz).magnitude; 

        bool running = Input.GetKey(KeyCode.LeftShift);
        float maxMove = running ? runSpeed : walkSpeed;

        
        float target01 = (maxMove > 0f) ? Mathf.InverseLerp(0f, maxMove, speedMps) : 0f;

        
        if (grounded && landTimer > 0f && inputMag > inputKickThreshold)
        {
            float kick = running ? minRunKick01 : minWalkKick01;
            if (target01 < kick) target01 = kick;
        }

        
        if (grounded && speedMps < 0.02f && inputMag > inputKickThreshold)
        {
            float kick = running ? minRunKick01 : minWalkKick01;
            if (target01 < kick) target01 = kick;
        }

        
        animSpeed = Mathf.Lerp(animSpeed, target01, Time.deltaTime * smooth);

        
        if (!string.IsNullOrEmpty(speedParam))      animator.SetFloat(speedParam, animSpeed);
        if (!string.IsNullOrEmpty(isGroundedParam)) animator.SetBool(isGroundedParam, grounded);
        if (!string.IsNullOrEmpty(isRunningParam))  animator.SetBool(isRunningParam, running);

        
        if (aimLayerIndex >= 0 && aimLayerIndex < animator.layerCount)
        {
            bool aiming = !string.IsNullOrEmpty(isAimingParam) && animator.GetBool(isAimingParam);
            float current = animator.GetLayerWeight(aimLayerIndex);
            float target = aiming ? 1f : 0f; 
            float next = Mathf.MoveTowards(current, target, Time.deltaTime * aimBlendSpeed);
            animator.SetLayerWeight(aimLayerIndex, next);
        }

        wasGrounded = grounded;
    }
}
