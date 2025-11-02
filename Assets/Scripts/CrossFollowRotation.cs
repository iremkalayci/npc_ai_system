using UnityEngine;

public class CrossFollowRotation : MonoBehaviour
{
    public Transform firePoint;   // silahın ucu (FirePoint)
    public Camera playerCamera;   // ana kamera
    public float distance = 2f;   // cross'un kameranın önünde duracağı mesafe
    public float heightOffset = 0f; // istersen yukarı-aşağı oynatmak için

    void LateUpdate()
    {
        if (firePoint == null || playerCamera == null) return;

        // Kameranın forward yönü (mouse hareketini takip eder)
        Vector3 forward = playerCamera.transform.forward;

        // FirePoint hizasında ama kameranın baktığı yöne göre konum
        Vector3 basePos = firePoint.position;
        Vector3 targetPos = basePos + forward * distance;
        targetPos.y += heightOffset;

        // Pozisyonu ve rotasyonu yumuşak değil, direkt uygula (gecikmeyi kaldırır)
        transform.position = targetPos;
        transform.rotation = Quaternion.LookRotation(forward);
    }
}
