using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;      
    public Rigidbody playerRb;      
    public Transform pivot;         
    public Transform cam;            

    [Header("Orbit (Mouse)")]
    public float mouseXSens = 60f;  
    public float mouseYSens = 60f;   
    public float minPitch = -25f;
    public float maxPitch = 60f;

    [Header("Follow (Position)")]
    public bool useSmoothFollow = false;
    public float camFollowLerp = 40f;    
    Vector3 _followVel;

    [Header("Behind Lock")]
    public bool lockBehind = true;       
    public float maxSideAngle = 85f;     
    public float autoCenterLerp = 0.2f; 

    [Header("Yaw Follow Speeds (daha yavaş için küçük tut)")]
    public float yawFollowLerpIdle    = 2.0f; 
    public float yawFollowLerpForward = 2.0f; 
    public float yawFollowLerpStrafe  = 1.4f; 
    public float yawFollowLerpBack    = 1.2f; 

    [Header("Player Align (opsiyonel)")]
    public bool alignPlayerToCamera = false; 
    public float playerTurnLerp = 18f;
    float _turnVel;

    float yaw;         
    float pitch;       
    float sideOffset;  
    float _yawVel;     

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

       
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

       
        pitch -= my * mouseYSens * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
        pivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        
        if (lockBehind)
        {
            sideOffset += mx * mouseXSens * Time.deltaTime;
            sideOffset  = Mathf.Clamp(sideOffset, -maxSideAngle, maxSideAngle);
        }
        else
        {
            sideOffset += mx * mouseXSens * Time.deltaTime;
        }

        
        float playerY   = player.eulerAngles.y;
        float targetYaw = lockBehind ? (playerY + sideOffset) : (yaw + mx * mouseXSens * Time.deltaTime);

        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float lerp = yawFollowLerpIdle; 

        if (Mathf.Abs(v) > 0.01f || Mathf.Abs(h) > 0.01f)
        {
            if (v > 0.01f)           lerp = yawFollowLerpForward; 
            else if (v < -0.01f)     lerp = yawFollowLerpBack;    
            else                     lerp = yawFollowLerpStrafe; 
        }

        float yawSmoothTime = (lerp <= 0f ? 0f : (1f / lerp));
        yaw = Mathf.SmoothDampAngle(yaw, targetYaw, ref _yawVel, yawSmoothTime);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

      
        cam.localPosition = new Vector3(0f, 0f, -4.5f);
        cam.localRotation = Quaternion.identity;

       
        if (alignPlayerToCamera)
        {
            float newY = Mathf.SmoothDampAngle(playerY, yaw, ref _turnVel,
                        (playerTurnLerp <= 0f ? 0f : (1f / playerTurnLerp)));
            player.rotation = Quaternion.Euler(0f, newY, 0f);
        }

        bool isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
        if (lockBehind && isMoving && autoCenterLerp > 0f)
            sideOffset = Mathf.Lerp(sideOffset, 0f, autoCenterLerp * Time.deltaTime);
    }
}
