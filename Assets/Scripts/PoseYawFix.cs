using UnityEngine;

public class PoseYawFix : MonoBehaviour
{
    [Header("Bağlantılar")]
    public Animator animator;          // CH15 üzerindeki Animator
    public Transform modelPivot;       // Idle altına eklediğin ModelPivot

    [Header("Açı Ayarları (derece)")]
    public float jumpYawFix = 90f;     // Zıplarken sola dönüyorsa +90, sağa ise -90 dene
    public float crouchYawFix = 0f;    // Gerekirse çömelme yürüyüşü için de açı verilebilir (örn. +10)

    [Header("Animator State İsimleri / Etiketleri")]
    public string jumpStateName = "Jump";          // Animator'daki Jump state adı
    public string crouchMoveStateName = "Crouch";  // Çömelip yürüme state adı (varsa)

    Quaternion baseLocalRot;

    void Start()
    {
        if (modelPivot == null) modelPivot = transform;
        baseLocalRot = modelPivot.localRotation;
    }

    void LateUpdate()
    {
        if (animator == null || modelPivot == null) return;

        // Animator state kontrolü
        AnimatorStateInfo s0 = animator.GetCurrentAnimatorStateInfo(0);

        // Varsayılan: düz
        Quaternion target = baseLocalRot;

        // Zıplama anında karşı-rotasyon uygula
        if (s0.IsName(jumpStateName))
        {
            target = baseLocalRot * Quaternion.Euler(0f, jumpYawFix, 0f);
        }
        // Çömelip yürürken de gerekiyorsa uygula
        else if (!string.IsNullOrEmpty(crouchMoveStateName) && s0.IsName(crouchMoveStateName))
        {
            target = baseLocalRot * Quaternion.Euler(0f, crouchYawFix, 0f);
        }

        // Yumuşak geçiş (istersen hızını artır/azalt)
        modelPivot.localRotation = Quaternion.Slerp(modelPivot.localRotation, target, Time.deltaTime * 12f);
    }
}
