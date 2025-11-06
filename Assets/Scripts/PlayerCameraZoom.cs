using UnityEngine;

public class PlayerCameraZoom : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    public Camera playerCamera;
    [Min(1f)] public float normalFOV = 60f;
    [Min(1f)] public float zoomFOV   = 35f;
    [Min(0.1f)] public float zoomSpeed = 10f;
    public KeyCode aimKey = KeyCode.Mouse1;

    [Header("Animator (opsiyonel)")]
    public Animator playerAnimator;                    
    [Tooltip("Nişan bool parametresi (ör. IsAiming ya da IsGunPlay) - boş bırakılabilir")]
    public string aimingBoolName = "IsAiming";
    [Tooltip("İstersen aynı anda IsGunPlay de senkronlansın - boş bırakılabilir")]
    public string extraGunplayBoolName = "IsGunPlay";

    [Header("Üst Gövde Aim Layer")]
    [Tooltip("Üst gövde/aim layer index (Avatar Mask: UpperBody). Yoksa -1 yap.")]
    public int aimingLayerIndex = 1;
    [Tooltip("Ayaktayken hedef layer weight (0..1)")]
    [Range(0f, 1f)] public float idleAimLayerWeight = 1f;
    [Tooltip("Hareket/koşu sırasında hedef layer weight (0..1)")]
    [Range(0f, 1f)] public float moveAimLayerWeight = 0.35f;
    [Tooltip("Layer ağırlık geçiş hızı")]
    [Min(0f)] public float layerBlendSpeed = 8f;

    [Header("Hız Algılama (opsiyonel)")]
    [Tooltip("Animator’da hız parametresi varsa adı (ör. Speed/MoveSpeed). Boşsa CC/Rigidbody hızından hesaplar.")]
    public string locomotionSpeedParam = "Speed";
    [Tooltip("Loco hızı dışarıdan okunuyorsa normalize etmek için maksimum koşu hızı (m/s)")]
    [Min(0.1f)] public float maxMoveSpeed = 6f;

    
    private bool isAiming;
    private CharacterController cc;
    private Rigidbody rb;

    void Reset()
    {
        playerCamera = Camera.main;
    }

    void Awake()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        cc = GetComponentInParent<CharacterController>();
        if (cc == null) cc = GetComponent<CharacterController>();

        rb = GetComponentInParent<Rigidbody>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
       
        isAiming = Input.GetKey(aimKey);

        
        if (playerCamera != null)
        {
            float target = Mathf.Clamp(isAiming ? zoomFOV : normalFOV, 1f, 179f);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, target, Time.deltaTime * zoomSpeed);
        }

        
        if (playerAnimator != null)
        {
            if (!string.IsNullOrEmpty(aimingBoolName))
                playerAnimator.SetBool(aimingBoolName, isAiming);

            if (!string.IsNullOrEmpty(extraGunplayBoolName))
                playerAnimator.SetBool(extraGunplayBoolName, isAiming);
        }
    }

    void LateUpdate()
    {
        
        if (playerAnimator != null && aimingLayerIndex >= 0 && aimingLayerIndex < playerAnimator.layerCount)
        {
            float speed01 = GetMove01(); 
            float targetWeight = isAiming
                ? Mathf.Lerp(idleAimLayerWeight, moveAimLayerWeight, speed01) 
                : 0f;

            float current = playerAnimator.GetLayerWeight(aimingLayerIndex);
            float next = Mathf.Lerp(current, targetWeight, Time.deltaTime * layerBlendSpeed);
            playerAnimator.SetLayerWeight(aimingLayerIndex, next);
        }
    }

    

    
    float GetMove01()
    {
        
        if (playerAnimator != null && !string.IsNullOrEmpty(locomotionSpeedParam))
        {
         
            return Mathf.Clamp01(playerAnimator.GetFloat(locomotionSpeedParam));
        }

        float speed = 0f;
        if (cc != null)       speed = new Vector3(cc.velocity.x, 0f, cc.velocity.z).magnitude;
        else if (rb != null)  speed = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;

        return Mathf.Clamp01(speed / Mathf.Max(0.1f, maxMoveSpeed));
    }
}
