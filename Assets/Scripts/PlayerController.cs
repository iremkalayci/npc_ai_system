using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float jumpForce = 3.8f;
    public float airControl = 0.5f;
    public float fallMultiplier = 8f;

    [Header("Yer Kontrolü")]
    [SerializeField] private Transform groundCheck;
    public float groundDistance = 0.25f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Animator animator;
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

    // 🔹 Eklenen değişkenler
    private float jumpCooldown = 0.25f;
    private float lastJumpTime = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cam = Camera.main != null ? Camera.main.transform : null;

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
                Debug.LogWarning("⚠️ GroundCheck bulunamadı! Ayağın altına bir Empty (GroundCheck) ekleyip atayın.");
        }
    }

    void Update()
    {
        // 🔹 Zemin kontrolü
        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 🔹 Girişler
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isGunPlay = Input.GetMouseButton(1);
        isCrouched = Input.GetKey(KeyCode.LeftControl);

        // 🔹 Zıplama tuş kontrolü (tek seferlik)
        if (Input.GetKeyDown(KeyCode.Space))
            jumpHeld = true;
        if (Input.GetKeyUp(KeyCode.Space))
            jumpHeld = false;

        // 🔹 Yalnızca yere temaslıyken zıplama hakkı ver
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

        MovePlayer();
        HandleJump();
        ApplyExtraGravity();
        AlignWithCamera(); // 🎯 Kamera hizalama eklendi
    }

    private void MovePlayer()
    {
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        if (inputDir.magnitude == 0f) return;

        float targetSpeed = isRunning ? runSpeed : moveSpeed;
        float controlFactor = isGrounded ? 1f : airControl;

        // 🔹 Kamera yönüne göre hareket
        Vector3 camForward = cam != null ? cam.forward : Vector3.forward;
        Vector3 camRight = cam != null ? cam.right : Vector3.right;
        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * 100f * Time.fixedDeltaTime);
        }

        // 🔹 Hareket
        rb.MovePosition(rb.position + moveDir * (targetSpeed * controlFactor * Time.fixedDeltaTime));
    }

    private void HandleJump()
    {
        if (!jumpQueued) return;
        jumpQueued = false;

        if (isGrounded && !isJumping)
        {
            isJumping = true;
            lastJumpTime = Time.time;

            // Dikey hız sıfırlanır (daha kontrollü zıplama)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (animator)
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Jump");
            }
        }
    }

    private void ApplyExtraGravity()
    {
        if (rb.linearVelocity.y < 0)
            rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Acceleration);
    }

    // 🎯 Yeni: Kamera yönüne göre hizalama (sağ tık nişan alınca)
    private void AlignWithCamera()
    {
        if (isGunPlay && cam != null)
        {
            Vector3 cameraForward = cam.forward;
            cameraForward.y = 0f; // sadece yatay yön
            if (cameraForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(cameraForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void UpdateAnimator()
    {
        if (!animator) return;

        float moveMag = new Vector2(horizontal, vertical).magnitude;
        float speedParam = moveMag * (isRunning ? 1f : 0.5f);

        animator.SetFloat("Speed", speedParam);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouched", isCrouched);
        animator.SetBool("IsGunPlay", isGunPlay);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
