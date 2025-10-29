using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    private AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [Header("Camera & Mouse")]
    [SerializeField] Transform camFollowPos;
    [SerializeField] float mouseSensitivity = 1f;

    private float xRotation;
    private float yRotation;

    [HideInInspector] public Animator anim;
    [HideInInspector] public CinemachineVirtualCamera vCam;

    [Header("FOV Settings")]
    public float adsFov = 40f;
    [HideInInspector] public float hipFov;
    [HideInInspector] public float currentFov;
    public float fovSmoothSpeed = 10f;

    [Header("Aim Settings")]
    public Transform aimPos;          // Hedef noktası (PlayerAimPos objesi)
    [SerializeField] float aimSmoothSpeed = 20f;
    [SerializeField] LayerMask aimMask; // Default + Enemy katmanlarını dahil et

    private void Start()
    {
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        if (!vCam)
            Debug.LogError("CinemachineVirtualCamera bulunamadı! Kamerayı karakterin altına child yap.");

        hipFov = vCam.m_Lens.FieldOfView;
        currentFov = hipFov;

        anim = GetComponentInChildren<Animator>();
        SwitchState(Hip);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 🎮 Mouse kontrolü
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);

        // 🔁 Durum güncelle
        currentState?.UpdateState(this);

        // 🎥 FOV geçişini yumuşat
        if (vCam)
            vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

        // 🎯 Hedef pozisyonunu kamera merkezine göre güncelle
        Vector2 screenCentre = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
        {
            // Çarpan objeye doğru
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
        }
        else
        {
            // Eğer bir şey vurmazsa, ileriye sabit bir nokta hedefle
            aimPos.position = Vector3.Lerp(aimPos.position, ray.GetPoint(100f), aimSmoothSpeed * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (camFollowPos)
        {
            camFollowPos.localEulerAngles = new Vector3(yRotation, 0f, 0f);
            transform.eulerAngles = new Vector3(0f, xRotation, 0f);
        }
    }

    public void SwitchState(AimBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
        currentFov = (state == Aim) ? adsFov : hipFov;
    }

    public bool IsAiming()
    {
        return Input.GetMouseButton(1); // Sağ tık nişan
    }
}
