using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MovementStateManager : MonoBehaviour
{
    
    public MovementBaseState currentState;
    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public RunState Run = new RunState();
    public CrouchState Crouch = new CrouchState();

    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController controller;

    [Header("Movement Settings")]
    public float currentMoveSpeed = 3f;
    public float walkSpeed = 3f, walkBackSpeed = 2f;
    public float runSpeed  = 7f, runBackSpeed  = 5f;
    public float crouchSpeed = 2f, crouchBackSpeed = 1f;

    [Header("Gravity / Ground")]
    public float gravity = -9.81f;
    public float groundYOffset = 0.1f;
    public LayerMask groundMask;

    [Header("Rotation")]
    public float turnSpeed = 12f; 
    
   
    [HideInInspector] public bool rotationLockedByAim = false; 

    [Header("Landing Kick (opsiyonel)")]
    public float landGraceTime = 0.15f; 	
    public float minWalkKick01 = 0.18f; 	

    // inputs / runtime
    [HideInInspector] public float hzInput;
    [HideInInspector] public float vInput;
    [HideInInspector] public Vector3 velocity; 	
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public bool isGrounded;

    private float landTimer; 	
    private bool wasGrounded; 	

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (controller == null) Debug.LogError("CharacterController yok!");
        if (anim == null) 	     Debug.LogError("Animator yok!");

        SwitchState(Idle);
    }

    void Update()
    {
       
        hzInput = Input.GetAxisRaw("Horizontal");
        vInput 	= Input.GetAxisRaw("Vertical");

        
        Vector3 forward = transform.forward;
        Vector3 right 	= transform.right;
        dir = (forward * vInput + right * hzInput);
        Vector3 moveXZ = dir.normalized * currentMoveSpeed; 

        
        Vector3 spherePos = transform.position - new Vector3(0f, groundYOffset, 0f);
        isGrounded = Physics.CheckSphere(
            spherePos,
            Mathf.Max(0.01f, controller.radius - 0.05f),
            groundMask,
            QueryTriggerInteraction.Ignore
        );

       
        if (isGrounded && !wasGrounded) landTimer = landGraceTime;
        else if (landTimer > 0f) 	 	landTimer -= Time.deltaTime;

        
        if (isGrounded && velocity.y < 0f) velocity.y = -2f; // yere yapıştırma
        else 	 	                       velocity.y += gravity * Time.deltaTime;

        
        
        if (!rotationLockedByAim)
        {
            Vector3 lookDir = new Vector3(moveXZ.x, 0f, moveXZ.z);
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(lookDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
            }
        }

        
        Vector3 motion = moveXZ; 
        motion.y = velocity.y; 	 
        controller.Move(motion * Time.deltaTime);

        
        SetFloatIfExists("hzInput", hzInput, 0.1f);
        SetFloatIfExists("vInput", 	vInput, 	0.1f);

       
        float speed01 = Mathf.Clamp01(new Vector2(hzInput, vInput).magnitude);

       
        if (landTimer > 0f && speed01 > 0f && speed01 < minWalkKick01)
            speed01 = minWalkKick01;

        
        float damp = (landTimer > 0f) ? 0f : 0.1f;
        SetFloatIfExists("Speed", speed01, damp);

        SetBoolIfExists("IsGrounded", isGrounded);

        
        currentState.UpdateState(this);

        
        wasGrounded = isGrounded;
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    private void OnDrawGizmos()
    {
        if (controller != null)
        {
            Gizmos.color = Color.red;
            Vector3 spherePos = transform.position - new Vector3(0f, groundYOffset, 0f);
            Gizmos.DrawWireSphere(spherePos, Mathf.Max(0.01f, controller.radius - 0.05f));
        }
    }

   
    void SetFloatIfExists(string name, float value, float damp = 0f)
    {
        foreach (var p in anim.parameters)
            if (p.name == name && p.type == AnimatorControllerParameterType.Float)
            {
                if (damp > 0f) anim.SetFloat(name, value, damp, Time.deltaTime);
                else 	 	       anim.SetFloat(name, value);
                return;
            }
    }
    void SetBoolIfExists(string name, bool value)
    {
        foreach (var p in anim.parameters)
            if (p.name == name && p.type == AnimatorControllerParameterType.Bool)
            {
                anim.SetBool(name, value);
                return;
            }
    }
}