using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Kamera Ayarları")]
    public Transform target;                 
    public Vector3 offset = new Vector3(0f, 2f, -4f);
    public float mouseSensitivity = 120f;
    public float rotationSmoothness = 10f;

    private float pitch = 0f;
    private float yaw = 0f;

    private Vector3 currentVelocity;
    public float positionSmoothTime = 0.05f; 
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
            yaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 45f);

        
        Quaternion desiredRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + desiredRotation * offset;

        
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothTime);

       
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothness * Time.deltaTime);
        transform.rotation = smoothRotation;

    }
}
