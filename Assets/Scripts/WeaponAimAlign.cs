using UnityEngine;

public class WeaponAimAlign : MonoBehaviour
{
    public Camera playerCamera;   
    public Transform firePoint;  
    public float rotationSpeed = 10f;

    void LateUpdate()
    {
        if (playerCamera == null || firePoint == null) return;

       
        Vector3 targetDirection = playerCamera.transform.forward;

       
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        firePoint.rotation = Quaternion.Slerp(firePoint.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
