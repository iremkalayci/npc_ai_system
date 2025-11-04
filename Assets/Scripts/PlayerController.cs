using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Zıplama Ayarları")]
    public float jumpHeight = 1.8f;     // ↑ Bunu büyütürsen daha yükseğe zıplar (örn. 2.2f)
    public float airControl = 0.5f;     // Havada yön verme katsayısı (0-1)
    public float fallMultiplier = 8f;   // Düşüşte ekstra yerçekimi (>=1 önerilir)

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

    // Zıplama spam koruması
    private float jumpCooldown = 0.25f;
    private float lastJumpTime = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); // child'ta ise yakalar
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
        // Zemin kontrolü
        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);

        // Girişler
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical   = Input.GetAxisRaw("Vertical");
        isRunning  = Input.GetKey(KeyCode.LeftShift);
        isGunPlay  = Input.GetMouseButton(1);
        isCrouched = Input.GetKey(KeyCode.LeftControl);

        // Zıplama tuş kontrolü (tek seferlik)
        if (Input.GetKeyDown(KeyCode.Space)) jumpHeld = true;
        if (Input.GetKeyUp(KeyCode.Space))   jumpHeld = false;

        // Yalnızca yere temaslıyken zıplama hakkı ver
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
        AlignWithCamera(); // 🎯 Kamera hizalama (sağ tık nişan)
    }

    private void MovePlayer()
    {
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        if (inputDir.sqrMagnitude < 0.0001f) return;

        float targetSpeed = isRunning ? runSpeed : moveSpeed;
        float controlFactor = isGrounded ? 1f : airControl;

        // Kamera yönüne göre hareket
        Vector3 camForward = cam ? cam.forward : Vector3.forward;
        Vector3 camRight   = cam ? cam.right   : Vector3.right;
        camForward.y = 0f; camRight.y = 0f;

        Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

        // Yumuşak dönüş
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * 100f * Time.fixedDeltaTime);
        }

        // Hareket (fiziksel konum güncelleme)
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

            // Zıplama yüksekliğinden başlangıç dikey hızını hesapla
            float g = Mathf.Abs(Physics.gravity.y);                     // 9.81…
            float v0 = Mathf.Sqrt(2f * g * Mathf.Max(0.01f, jumpHeight)); // güvenlik için min

            // Mevcut Y hızını sıfırla ve dikey hızı ver
            Vector3 v = rb.linearVelocity;
            v.y = v0;                           // doğrudan hız vermek daha net sonuç
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
        // Unity zaten Physics.gravity uyguluyor; biz düşüşte extra ekliyoruz
        if (rb.linearVelocity.y < 0f)
        {
            // Ekstra yerçekimi: (fallMultiplier - 1) kadar ilave ivme
            Vector3 extra = Physics.gravity * (fallMultiplier - 1f);
            rb.AddForce(extra, ForceMode.Acceleration);
        }
    }

    // 🎯 Kamera yönüne göre hizalama (sağ tık nişan)
    private void AlignWithCamera()
    {
        if (isGunPlay && cam != null)
        {
            Vector3 cameraForward = cam.forward; cameraForward.y = 0f;
            if (cameraForward.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(cameraForward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void UpdateAnimator()
    {
        if (!animator) return;

        // XZ hızından Speed (zıplamanın Y bileşeni dahil değil)
        Vector3 planar = rb.linearVelocity; planar.y = 0f;
        float normalized = 0f;
        if (runSpeed > 0.01f) normalized = Mathf.Clamp01(planar.magnitude / runSpeed);

        // Shift yoksa yürüme bandını biraz düşük tut
        float speedParam = isRunning ? normalized : normalized * 0.5f;

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
