using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class JumpHandler : MonoBehaviour
{
    [Header("Refs")]
    public Rigidbody rb;                  
    public Transform groundCheck;        
    public LayerMask groundMask;       

    [Header("Jump Ayarları")]
    [Min(0f)] public float jumpForce = 5.2f;           
    [Range(0.05f, 0.4f)] public float groundCheckRadius = 0.15f;
    public KeyCode jumpKey = KeyCode.Space;
    [Min(0f)] public float jumpCooldown = 0.15f;       

    [Header("Düşüş Hissi (opsiyonel)")]
    [Min(0f)] public float extraFallGravity = 14f;    
    [Min(0f)] public float lowJumpGravity = 6f;        

    [Header("Animator (opsiyonel)")]
    public Animator animator;                           
    public string jumpTriggerName = "Jump";             

    float lastJumpTime = -999f;
    bool isGroundedCached;

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
        var gc = transform.Find("GroundCheck");
        if (gc) groundCheck = gc;

        
        int ground = LayerMask.NameToLayer("Ground");
        if (ground >= 0) groundMask = 1 << ground;

        
        rb.freezeRotation = true;
    }

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (!rb || !groundCheck) return;

        
        isGroundedCached = Physics.CheckSphere(
            groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        
        if (Input.GetKeyDown(jumpKey) && isGroundedCached && Time.time >= lastJumpTime + jumpCooldown)
        {
            
            var v = rb.linearVelocity; v.y = 0f; rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;

           
            if (animator && !string.IsNullOrWhiteSpace(jumpTriggerName))
                animator.SetTrigger(jumpTriggerName);
        }

        
        if (rb.linearVelocity.y < -0.01f)
        {
            rb.AddForce(Vector3.down * (extraFallGravity * Time.deltaTime), ForceMode.VelocityChange);
        }
        else if (rb.linearVelocity.y > 0.01f && !Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.down * (lowJumpGravity * Time.deltaTime), ForceMode.VelocityChange);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}
