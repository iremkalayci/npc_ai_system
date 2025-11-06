using UnityEngine;

namespace GunController
{
    public class GunMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public CharacterController controller;

        [Header("Look Settings")]
        public float mouseSensitivity = 2f;
        public float minPitch = -80f;
        public float maxPitch = 80f;

        private float yaw = 0f;
        private float pitch = 0f;

        private void Awake()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
                if (controller == null) controller = gameObject.AddComponent<CharacterController>();
            }

            
            Vector3 forward = transform.forward;
            yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            pitch = Mathf.Asin(forward.y) * Mathf.Rad2Deg;

            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            HandleMovement();
            HandleLook();
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");  
            float vertical = Input.GetAxis("Vertical");      
            float upDown = 0f;

            if (Input.GetKey(KeyCode.E)) upDown += 1f;       
            if (Input.GetKey(KeyCode.Q)) upDown -= 1f;       

            Vector3 move = transform.right * horizontal + transform.forward * vertical + Vector3.up * upDown;
            move.Normalize();

            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        private void HandleLook()
        {
            if (Input.GetMouseButton(2)) 
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                yaw += mouseX;
                pitch -= mouseY;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
