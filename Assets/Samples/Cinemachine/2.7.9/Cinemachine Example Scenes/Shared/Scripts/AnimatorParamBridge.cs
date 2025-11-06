using UnityEngine;

public class AnimatorParamBridge : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;                
    public Rigidbody rb;                     
    public CharacterController cc;           
    public Transform groundCheck;            
    public LayerMask groundMask = ~0;        

    [Header("Ayarlar")]
    public float speedLerp = 10f;
    public float groundCheckRadius = 0.15f;

    float smoothedSpeed;
    bool isGrounded;
    Vector3 lastPos;

    void Reset()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        var gc = transform.Find("GroundCheck");
        if (gc) groundCheck = gc;
        lastPos = transform.position;
    }

    void Update()
    {
        if (!animator) return;

        
        float planarSpeed = 0f;

        if (cc != null)
        {
            Vector3 v = cc.velocity; v.y = 0f;
            planarSpeed = v.magnitude;
        }
        else if (rb != null)
        {
            Vector3 v = rb.linearVelocity; v.y = 0f;
            planarSpeed = v.magnitude;
        }
        else
        {
            Vector3 delta = transform.position - lastPos;
            delta.y = 0f;
            planarSpeed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        }

        smoothedSpeed = Mathf.Lerp(smoothedSpeed, planarSpeed, Time.deltaTime * speedLerp);
        animator.SetFloat("Speed", smoothedSpeed);

        // 2) Koşma / Çömelme (geçici tuşlar)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsCrouching", isCrouching);

        // 3) Yerde mi?
        if (groundCheck)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        else
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.3f, groundMask);

        animator.SetBool("IsGrounded", isGrounded);

        lastPos = transform.position;
    }
}
