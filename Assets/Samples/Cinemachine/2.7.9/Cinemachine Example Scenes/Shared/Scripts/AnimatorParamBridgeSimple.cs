using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorParamBridgeSimple : MonoBehaviour
{
    public Rigidbody rb;                 
    public string speedParam = "Speed";

    Animator anim;
    int speedHash;

    void Awake()
    {
        anim = GetComponent<Animator>();
        speedHash = Animator.StringToHash(speedParam);
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float speedFromInput = new Vector2(h, v).magnitude;          
        float speedFromRb    = (rb != null) ? rb.linearVelocity.magnitude : 0f;

        float speed = (speedFromRb > 0.01f) ? speedFromRb : speedFromInput;
        anim.SetFloat(speedHash, speed);
    }
}
