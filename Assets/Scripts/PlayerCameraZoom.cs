using UnityEngine;

public class PlayerCameraZoom : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float zoomFOV = 35f;
    public float zoomSpeed = 10f;

    [Header("Animator Bağlantısı")]
    public Animator playerAnimator;

    private bool isAiming = false;

    void Start()
    {
        // Fare imlecini gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 🔹 Sağ tık (Mouse1) ile nişan alma
        isAiming = Input.GetMouseButton(1);

        // 🔹 Kamera zoom (FOV geçişi)
        if (playerCamera != null)
        {
            float targetFOV = isAiming ? zoomFOV : normalFOV;
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * zoomSpeed
            );
        }

        // 🔹 Nişan animasyonu
        if (playerAnimator != null)
            playerAnimator.SetBool("IsAiming", isAiming);
    }

    void LateUpdate()
    {
        // 🔹 Üst gövde animasyon ağırlığı (yumuşak geçiş)
        if (playerAnimator != null)
        {
            float targetWeight = isAiming ? 1f : 0f;
            float currentWeight = playerAnimator.GetLayerWeight(1);
            playerAnimator.SetLayerWeight(
                1,
                Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 8f)
            );
        }
    }
}
