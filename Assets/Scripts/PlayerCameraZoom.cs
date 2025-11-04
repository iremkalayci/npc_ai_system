using UnityEngine;

public class PlayerCameraZoom : MonoBehaviour
{
    [Header("Zoom Ayarları")]
    public Camera playerCamera;
    [Min(1f)] public float normalFOV = 60f;
    [Min(1f)] public float zoomFOV = 35f;
    [Min(0.1f)] public float zoomSpeed = 10f;   // FOV geçiş hızı

    [Header("Animator (opsiyonel)")]
    public Animator playerAnimator;            // Karakter Animator
    [Tooltip("IsAiming bool parametresini ayarla")]
    public string aimingBoolName = "IsAiming";
    [Tooltip("Aiming layer index (varsa)")]
    public int aimingLayerIndex = 1;           // Üst gövde/aim layer'ı (yoksa -1 yap)

    private bool isAiming = false;

    void Update()
    {
        // Sağ tık ile aim
        isAiming = Input.GetMouseButton(1);

        // Kamera FOV zoom
        if (playerCamera != null)
        {
            float targetFOV = Mathf.Clamp(isAiming ? zoomFOV : normalFOV, 1f, 179f);
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                Time.deltaTime * zoomSpeed
            );
        }

        // Animator bool
        if (playerAnimator != null && !string.IsNullOrEmpty(aimingBoolName))
            playerAnimator.SetBool(aimingBoolName, isAiming);
    }

    void LateUpdate()
    {
        // Animator aiming layer ağırlığı (varsa)
        if (playerAnimator != null && aimingLayerIndex >= 0 &&
            aimingLayerIndex < playerAnimator.layerCount)
        {
            float targetWeight = isAiming ? 1f : 0f;
            float currentWeight = playerAnimator.GetLayerWeight(aimingLayerIndex);
            playerAnimator.SetLayerWeight(
                aimingLayerIndex,
                Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 8f)
            );
        }
    }
}
