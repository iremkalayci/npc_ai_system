using UnityEngine;

public class CrossFollowRotation : MonoBehaviour
{
    public Camera playerCamera;     // Ana kamera
    public float distance = 6f;     // Kameranın önünde sabit mesafe
    public float heightOffset = 0f; // Yukarı-aşağı
    public float lateralOffset = 0f;// Sağa-sola (omuz kamera için)
    public bool detachOnStart = true;

    void Awake()
    {
        if (detachOnStart) transform.SetParent(null, true);
    }

    void LateUpdate()
    {
        if (!playerCamera) return;

        var cam = playerCamera.transform;

        Vector3 desired =
            cam.position +
            cam.forward * distance +
            Vector3.up * heightOffset +
            cam.right * lateralOffset;

        transform.position = desired;
        transform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
    }
}
