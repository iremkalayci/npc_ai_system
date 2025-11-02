using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class JumpHandler : MonoBehaviour
{
    [Header("Refs")]
    public Rigidbody rb;                  // Player'ın Rigidbody'si
    public Transform groundCheck;         // Ayak altındaki nokta
    public LayerMask groundMask;          // Ground layer(lar)ı

    [Header("Jump Ayarları")]
    [Min(0f)] public float jumpForce = 5.2f;           // 4.5–6.0 arası tipik
    [Range(0.05f, 0.4f)] public float groundCheckRadius = 0.15f;
    public KeyCode jumpKey = KeyCode.Space;
    [Min(0f)] public float jumpCooldown = 0.15f;       // spam önleme

    [Header("Düşüş Hissi (opsiyonel)")]
    [Min(0f)] public float extraFallGravity = 14f;     // düşüşü hızlandırır
    [Min(0f)] public float lowJumpGravity = 6f;        // kısa basışta daha kısa zıplama

    [Header("Animator (opsiyonel)")]
    public Animator animator;                           // Idle üzerindeki Animator
    public string jumpTriggerName = "Jump";             // Trigger adı (kullanıyorsan)

    float lastJumpTime = -999f;
    bool isGroundedCached;

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
        var gc = transform.Find("GroundCheck");
        if (gc) groundCheck = gc;

        // Varsayılan: sadece "Ground" layer'ı
        int ground = LayerMask.NameToLayer("Ground");
        if (ground >= 0) groundMask = 1 << ground;

        // Varsayılan fizik kilitleri
        rb.freezeRotation = true;
    }

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        // Küçük güvenlik: kütle/drag default kalsın
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (!rb || !groundCheck) return;

        // Zemin kontrolü
        isGroundedCached = Physics.CheckSphere(
            groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        // Zıplama
        if (Input.GetKeyDown(jumpKey) && isGroundedCached && Time.time >= lastJumpTime + jumpCooldown)
        {
            // Y hızını sıfırla → tutarlı zıplama
            var v = rb.linearVelocity; v.y = 0f; rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time;

            // Animator Trigger kullanıyorsan tetikle
            if (animator && !string.IsNullOrWhiteSpace(jumpTriggerName))
                animator.SetTrigger(jumpTriggerName);
        }

        // Düşüş fiziği (daha tok his)
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
