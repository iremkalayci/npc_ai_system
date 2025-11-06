using UnityEngine;

public class PoseYawFix : MonoBehaviour
{
    [Header("Bağlantılar")]
    public Animator animator;          
    public Transform modelPivot;      

    [Header("Açı Ayarları (derece)")]
    public float jumpYawFix = 90f;     
    public float crouchYawFix = 0f;    

    [Header("Animator State İsimleri / Etiketleri")]
    public string jumpStateName = "Jump";         
    public string crouchMoveStateName = "Crouch";  

    Quaternion baseLocalRot;

    void Start()
    {
        if (modelPivot == null) modelPivot = transform;
        baseLocalRot = modelPivot.localRotation;
    }

    void LateUpdate()
    {
        if (animator == null || modelPivot == null) return;

        
        AnimatorStateInfo s0 = animator.GetCurrentAnimatorStateInfo(0);

       
        Quaternion target = baseLocalRot;

      
        if (s0.IsName(jumpStateName))
        {
            target = baseLocalRot * Quaternion.Euler(0f, jumpYawFix, 0f);
        }
        
        else if (!string.IsNullOrEmpty(crouchMoveStateName) && s0.IsName(crouchMoveStateName))
        {
            target = baseLocalRot * Quaternion.Euler(0f, crouchYawFix, 0f);
        }

        
        modelPivot.localRotation = Quaternion.Slerp(modelPivot.localRotation, target, Time.deltaTime * 12f);
    }
}
