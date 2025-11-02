using UnityEngine;

public class WeaponAimAlign : MonoBehaviour
{
    public Camera playerCamera;   // Ana kamera
    public Transform firePoint;   // Namlunun ucu
    public float rotationSpeed = 10f; // Yumuşak dönüş hızı

    void LateUpdate()
    {
        if (playerCamera == null || firePoint == null) return;

        // Kameranın baktığı yön
        Vector3 targetDirection = playerCamera.transform.forward;

        // FirePoint'i kameranın yönüne yumuşak şekilde döndür
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        firePoint.rotation = Quaternion.Slerp(firePoint.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
