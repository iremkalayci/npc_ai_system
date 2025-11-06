using UnityEngine;

public class JumpRotationFix : MonoBehaviour
{
    public Animator animator;        
    public Transform modelRoot;      
    public float rotationFixY = 15f;   

    private Quaternion initialLocalRot;
    private int jumpHash;
    private bool wasJumping;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (modelRoot != null) initialLocalRot = modelRoot.localRotation;

        
        jumpHash = Animator.StringToHash("Base Layer.Jump");
       
    }

    void Update()
    {
        if (animator == null || modelRoot == null) return;

        
        var st = animator.GetCurrentAnimatorStateInfo(0);
        bool isJumping = st.fullPathHash == jumpHash;       

        if (isJumping)
        {
            
            modelRoot.localRotation = initialLocalRot * Quaternion.Euler(0f, rotationFixY, 0f);
        }
        else
        {
            
            if (wasJumping)
                modelRoot.localRotation = initialLocalRot;
        }

        wasJumping = isJumping;
    }
}
