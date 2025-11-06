using UnityEngine;

namespace EnvironmentProps
{
    public class RadarSpinner : MonoBehaviour
    {
        [Header("Spin Settings")]
        public float spinSpeed = 20f; 

        private void Update()
        {
            
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
        }
    }
}
