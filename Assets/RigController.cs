using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    [Header("Referanslar")]
    public RigBuilder rigBuilder;      // RigBuilder bileşeni
    public Animator animator;          // Karakterin Animator'ı
    public string aimParameter = "IsAiming";  // Animator’daki nişan parametresi (bool)
    
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
        // Animator'dan nişan durumunu oku
        bool isAiming = animator.GetBool(aimParameter);

        // Nişan alırken rig aktif, değilken pasif
        targetWeight = isAiming ? 1f : 0f;

        // Rig weight'ini yumuşak geçişle ayarla
        foreach (var layer in rigBuilder.layers)
        {
            if (layer.rig != null)
                layer.rig.weight = Mathf.Lerp(layer.rig.weight, targetWeight, Time.deltaTime * 6f);
        }
    }
}
