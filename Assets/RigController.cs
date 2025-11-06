using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    [Header("Referanslar")]
    public RigBuilder rigBuilder;      
    public Animator animator;          
    public string aimParameter = "IsAiming";  
    
    private float targetWeight = 0f;

    void Start()
    {
        if (rigBuilder == null)
            rigBuilder = GetComponent<RigBuilder>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
       
        bool isAiming = animator.GetBool(aimParameter);

       
        targetWeight = isAiming ? 1f : 0f;

        
        foreach (var layer in rigBuilder.layers)
        {
            if (layer.rig != null)
                layer.rig.weight = Mathf.Lerp(layer.rig.weight, targetWeight, Time.deltaTime * 6f);
        }
    }
}
