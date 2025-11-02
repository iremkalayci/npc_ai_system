using UnityEngine;

public class JumpRotationFix : MonoBehaviour
{
    public Animator animator;
    public Transform modelRoot;   // Karakter modelinin üst kısmı (örneğin CH15)
    public float rotationFixY = 15f; // Sola dönüyorsa +, sağa dönüyorsa -

    private bool wasJumping = false;

    void Update()
    {
        if (animator == null || modelRoot == null) return;

        bool isJumping = animator.GetCurrentAnimatorStateInfo(0).IsName("Jump");

        // Zıplama başladığında düzelt
        if (isJumping && !wasJumping)
        {
            modelRoot.localRotation *= Quaternion.Euler(0, rotationFixY, 0);
        }

        // Zıplama bittiğinde rotasyonu sıfırla
        if (!isJumping && wasJumping)
        {
            modelRoot.localRotation = Quaternion.identity;
        }

        wasJumping = isJumping;
    }
}
