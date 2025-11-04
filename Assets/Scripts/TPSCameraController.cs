using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;         // Idle (player root)
    public Rigidbody playerRb;       // (Varsa) Idle üzerindeki Rigidbody
    public Transform pivot;          // Pivot objesi
    public Transform cam;            // Main Camera

    [Header("Orbit (Mouse)")]
    public float mouseXSens = 60f;   // yatay hassasiyet
    public float mouseYSens = 60f;   // dikey hassasiyet
    public float minPitch = -25f;
    public float maxPitch = 60f;

    [Header("Follow (Position)")]
    public bool useSmoothFollow = false; // anında pozisyon takibi için false
    public float camFollowLerp = 40f;    // SmoothDamp için (useSmoothFollow=true ise)
    Vector3 _followVel;

    [Header("Behind Lock")]
    public bool lockBehind = true;       // Kamerayı arka hemisferde tut
    public float maxSideAngle = 85f;     // Oyuncu yönüne göre +/- izinli açı
    public float autoCenterLerp = 0.2f;  // Hareket ederken ofseti merkeze toplama hızı

    [Header("Yaw Follow Speeds (daha yavaş için küçük tut)")]
    public float yawFollowLerpIdle    = 2.0f; // karakter duruyorsa
    public float yawFollowLerpForward = 2.0f; // W
    public float yawFollowLerpStrafe  = 1.4f; // A / D
    public float yawFollowLerpBack    = 1.2f; // S

    [Header("Player Align (opsiyonel)")]
    public bool alignPlayerToCamera = false; // kameraya göre oyuncuyu döndürme (genelde kapalı)
    public float playerTurnLerp = 18f;
    float _turnVel;

    float yaw;         // rig yaw
    float pitch;       // pivot pitch
    float sideOffset;  // oyuncu yönüne göre yan ofset (-maxSideAngle..+maxSideAngle)
    float _yawVel;     // SmoothDampAngle cache

    void Start()
    {
        yaw   = transform.eulerAngles.y;
        pitch = pivot.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        if (playerRb == null && player != null)
            playerRb = player.GetComponent<Rigidbody>();

        if (player != null)
            sideOffset = Mathf.DeltaAngle(yaw, player.eulerAngles.y);
    }

    void LateUpdate()
    {
        if (player == null || pivot == null || cam == null) return;

        // --- 1) Pozisyon takibi ---
        Vector3 desiredPos = (playerRb != null) ? playerRb.position : player.position;
        if (useSmoothFollow)
        {
            float posSmoothTime = (camFollowLerp <= 0f) ? 0f : (1f / camFollowLerp);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _followVel, posSmoothTime);
        }
        else
        {
            transform.position = desiredPos;
        }

        // --- 2) Fare girdisi ---
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        // Pitch
        pitch -= my * mouseYSens * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
        pivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Yan ofset (sadece mouse ile değişsin)
        if (lockBehind)
        {
            sideOffset += mx * mouseXSens * Time.deltaTime;
            sideOffset  = Mathf.Clamp(sideOffset, -maxSideAngle, maxSideAngle);
        }
        else
        {
            sideOffset += mx * mouseXSens * Time.deltaTime;
        }

        // --- 3) Yaw hedefi ---
        float playerY   = player.eulerAngles.y;
        float targetYaw = lockBehind ? (playerY + sideOffset) : (yaw + mx * mouseXSens * Time.deltaTime);

        // --- 4) HAREKETE GÖRE YUMUŞAKLIK SEÇ ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float lerp = yawFollowLerpIdle; // varsayılan

        if (Mathf.Abs(v) > 0.01f || Mathf.Abs(h) > 0.01f)
        {
            if (v > 0.01f)           lerp = yawFollowLerpForward; // W
            else if (v < -0.01f)     lerp = yawFollowLerpBack;    // S
            else                     lerp = yawFollowLerpStrafe;  // A/D (strafe)
        }

        float yawSmoothTime = (lerp <= 0f ? 0f : (1f / lerp));
        yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref _yawVel, yawSmoothTime);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // --- 5) Kamera mesafe/rotasyon ---
        cam.localPosition = new Vector3(0f, 0f, -4.5f);
        cam.localRotation = Quaternion.identity;

        // --- 6) Oyuncuyu kameraya hizalama (genelde kapalı) ---
        if (alignPlayerToCamera)
        {
            float newY = Mathf.SmoothDampAngle(playerY, yaw, ref _turnVel,
                        (playerTurnLerp <= 0f ? 0f : (1f / playerTurnLerp)));
            player.rotation = Quaternion.Euler(0f, newY, 0f);
        }

        // --- 7) Hareket ederken ofseti yavaşça merkeze çek (opsiyon) ---
        bool isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
        if (lockBehind && isMoving && autoCenterLerp > 0f)
            sideOffset = Mathf.Lerp(sideOffset, 0f, autoCenterLerp * Time.deltaTime);
    }
}
