using UnityEngine;

public class HandFistPose : MonoBehaviour
{
    [Header("El Kemikleri")]
    public Transform thumb1;
    public Transform index1;
    public Transform middle1;
    public Transform ring1;
    public Transform pinky1;

    [Header("Bükülme Açıları")]
    [Range(0, 90)] public float curlAngle = 60f;

    void LateUpdate()
    {
        if (thumb1) thumb1.localRotation = Quaternion.Euler(curlAngle, 0, 0);
        if (index1) index1.localRotation = Quaternion.Euler(curlAngle, 0, 0);
        if (middle1) middle1.localRotation = Quaternion.Euler(curlAngle, 0, 0);
        if (ring1) ring1.localRotation = Quaternion.Euler(curlAngle, 0, 0);
        if (pinky1) pinky1.localRotation = Quaternion.Euler(curlAngle, 0, 0);
    }
}
