using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Zıplama Ayarları")]
    public float jumpHeight = 1.8f;    
    public float airControl = 0.5f;     
    public float fallMultiplier = 8f;   

    [Header("Yer Kontrolü")]
    [SerializeField] private Transform groundCheck;
    public float groundDistance = 0.25f;
    public LayerMask groundMask;

    [Header("Health / Damage")]
    public PlayerHealth playerHealth;           
    [Tooltip("NPC mermisi isabetinde toplam kaç HP gitsin?")]
    public float bulletHitDamage = 20f;
    [Tooltip("Bu hasarı kaç saniyeye yayalım?")]
    public float bulletDamageDuration = 0.50f;  
    [Tooltip("Ölünce input kilitlensin mi?")]
    public bool lockControlsOnDeath = true;

    // Dahili
    private Rigidbody rb;
    [SerializeField] private Animator animator; 
    private Transform cam;

    private float horizontal;
    private float vertical;
    private bool isGrounded;
    private bool isRunning;
    private bool isCrouched;
    private bool isGunPlay;
    private bool isJumping;
    private bool jumpQueued;
    private bool jumpHeld;

   
    private float jumpCooldown = 0.25f;
    private float lastJumpTime = -1f;

   
    private bool isDeadLock;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        cam = Camera.main ? Camera.main.transform : null;

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
                Debug.LogWarning("⚠️ GroundCheck yok. Ayağın altına bir Empty (GroundCheck) ekleyip ata.");
        }

        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
       
        if (lockControlsOnDeath && !isDeadLock && playerHealth && playerHealth.CurrentHealth <= 0f)
        {
            isDeadLock = true;
            rb.linearVelocity = Vector3.zero;
        }
        if (isDeadLock) return;

       
        if (groundCheck)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);

       
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical   = Input.GetAxisRaw("Vertical");
        isRunning  = Input.GetKey(KeyCode.LeftShift);
        isGunPlay  = Input.GetMouseButton(1);
        isCrouched = Input.GetKey(KeyCode.LeftControl);

       
        if (Input.GetKeyDown(KeyCode.Space)) jumpHeld = true;
        if (Input.GetKeyUp(KeyCode.Space))   jumpHeld = false;

        if (jumpHeld && isGrounded && !isJumping && Time.time - lastJumpTime > jumpCooldown)
        {
            jumpQueued = true;
            jumpHeld = false;
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isGrounded && rb.linearVelocity.y <= 0.01f)
            isJumping = false;

        if (isDeadLock) return;

        MovePlayer();
        HandleJump();
        ApplyExtraGravity();
        AlignWithCamera();
    }

    private void MovePlayer()
    {
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        if (inputDir.sqrMagnitude < 0.0001f) return;

        float targetSpeed = isRunning ? runSpeed : moveSpeed;
        float control = isGrounded ? 1f : airControl;

        Vector3 camForward = cam ? cam.forward : Vector3.forward;
        Vector3 camRight   = cam ? cam.right   : Vector3.right;
        camForward.y = 0f; camRight.y = 0f;

        Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

        
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * 100f * Time.fixedDeltaTime);
        }

        
        rb.MovePosition(rb.position + moveDir * (targetSpeed * control * Time.fixedDeltaTime));
    }

    private void HandleJump()
    {
        if (!jumpQueued) return;
        jumpQueued = false;

        if (isGrounded && !isJumping)
        {
            isJumping = true;
            lastJumpTime = Time.time;

            
            float g = Mathf.Abs(Physics.gravity.y);
            float v0 = Mathf.Sqrt(2f * g * Mathf.Max(0.01f, jumpHeight));

            Vector3 v = rb.linearVelocity;
            v.y = v0;
            rb.linearVelocity = v;

            if (animator)
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Jump");
            }
        }
    }

    private void ApplyExtraGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            Vector3 extra = Physics.gravity * (fallMultiplier - 1f);
            rb.AddForce(extra, ForceMode.Acceleration);
        }
    }

    private void AlignWithCamera()
    {
        if (isGunPlay && cam)
        {
            Vector3 forward = cam.forward; forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(forward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void UpdateAnimator()
    {
        if (!animator) return;

        Vector3 planar = rb.linearVelocity; planar.y = 0f;
        float normalized = runSpeed > 0.01f ? Mathf.Clamp01(planar.magnitude / runSpeed) : 0f;
        float speedParam = isRunning ? normalized : normalized * 0.5f;

        animator.SetFloat("Speed", speedParam);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouched", isCrouched);
        animator.SetBool("IsGunPlay", isGunPlay);
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            if (playerHealth && bulletHitDamage > 0f)
                StartCoroutine(DamageOverTime(bulletHitDamage, bulletDamageDuration));
        }
    }

    IEnumerator DamageOverTime(float amount, float duration)
    {
        if (!playerHealth || amount <= 0f || duration <= 0f) yield break;

        float t = 0f;
        float start = playerHealth.CurrentHealth;
        float target = Mathf.Max(0f, start - amount);

        while (t < duration && playerHealth.CurrentHealth > target)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float h = Mathf.Lerp(start, target, k);
            playerHealth.SetHealth(h);
            yield return null;
        }

        playerHealth.SetHealth(target);

        if (lockControlsOnDeath && playerHealth.CurrentHealth <= 0f)
        {
            isDeadLock = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
