using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CCJumpGravity : MonoBehaviour
{
    [Header("Fizik")]
    public float gravity = -19.62f;     
    public float jumpHeight = 1.2f;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Animator (opsiyonel ama önerilir)")]
    public Animator animator;                  
    public string isGroundedParam = "IsGrounded"; 
    public string jumpTriggerParam = "Jump";      
    public string verticalSpeedParam = "VertSpeed"; 

    [Header("State Fallback (inişte Idle'a dönüş)")]
    public bool  forceLocomotionOnLand = true;
    public string locomotionStateName   = "Base Layer.Idle";
    public float locomotionCrossFade    = 0.05f;

    private CharacterController cc;
    private float yVel;            
    private bool  wasGrounded;     

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        bool grounded = cc.isGrounded;

        
        if (grounded && yVel < 0f)
            yVel = -2f;

        
        if (grounded && Input.GetKeyDown(jumpKey))
        {
            yVel = Mathf.Sqrt(jumpHeight * -2f * gravity);

            
            if (animator != null && !string.IsNullOrEmpty(jumpTriggerParam))
                animator.SetTrigger(jumpTriggerParam);
        }

        
        yVel += gravity * Time.deltaTime;

        
        Vector3 verticalMove = new Vector3(0f, yVel, 0f) * Time.deltaTime;
        cc.Move(verticalMove);

        
        if (animator != null)
        {
            if (!string.IsNullOrEmpty(isGroundedParam))
                animator.SetBool(isGroundedParam, grounded);

            if (!string.IsNullOrEmpty(verticalSpeedParam))
                animator.SetFloat(verticalSpeedParam, yVel);

            
            if (grounded && !wasGrounded && !string.IsNullOrEmpty(jumpTriggerParam))
                animator.ResetTrigger(jumpTriggerParam);

            
            if (grounded && !wasGrounded && forceLocomotionOnLand && !string.IsNullOrEmpty(locomotionStateName))
                animator.CrossFadeInFixedTime(locomotionStateName, locomotionCrossFade);
        }

        wasGrounded = grounded;
    }
}
