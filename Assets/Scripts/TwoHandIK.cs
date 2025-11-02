using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TwoHandIK : MonoBehaviour
{
    [Header("IK Ayarları")]
    public bool enableIK = true;             // IK açık mı?
    public Transform leftHandTarget;         // Sol elin hedef noktası (LeftHand_Grip)
    public Transform rightHandTarget;        // (İstersen sağ el için de kullanılabilir)
    public float ikWeight = 1.0f;            // IK ağırlığı (1 = tamamen hedefte)

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || !enableIK)
            return;

        // Sol el için IK hedefi uygula
        if (leftHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }

        // (İstersen sağ el için de aktif edebilirsin)
        if (rightHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        }
    }
}
