using UnityEngine;

public class CrossFollowRotation : MonoBehaviour
{
    [Header("Referanslar")]
    public Camera playerCamera;         
    public Transform firePoint;         

    [Header("Konumlandırma")]
    public float maxRayDistance = 150f; 
    public float fallbackDistance = 12f;
    public float surfaceOffset = 0.02f;
    public float heightOffset = 0f;    
    public float lateralOffset = 0f;   

    [Header("Katmanlar")]
    public LayerMask hitMask = ~0;      
    public bool ignoreTriggers = true;

    [Header("Görsel")]
    public bool faceCamera = true;     
    public float smooth = 0f;           

    [Header("Ağaç Yapısı")]
    public bool detachOnStart = true;   

   
    public Vector3 LastTargetPoint { get; private set; }

    void Reset()
    {
        playerCamera = Camera.main;
        
        int exclude = LayerMask.GetMask("Player", "UI", "Ignore Raycast");
        hitMask = ~exclude;
    }

    void Awake()
    {
        if (detachOnStart) transform.SetParent(null, true);
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (!playerCamera) return;

        var cam = playerCamera.transform;

        
        Vector3 origin = cam.position;
        Vector3 dir    = cam.forward;

        bool hitSomething = Physics.Raycast(
            origin, dir, out RaycastHit hit, maxRayDistance, hitMask,
            ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        Vector3 targetPos = hitSomething
            ? hit.point + hit.normal * surfaceOffset
            : origin + dir * fallbackDistance;

        
        targetPos += cam.up * heightOffset + cam.right * lateralOffset;

        LastTargetPoint = targetPos;

        
        if (smooth > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);
        }
        else
        {
            transform.position = targetPos;
        }

        
        if (faceCamera)
            transform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
        else
            transform.rotation = Quaternion.identity;

        
        if (firePoint != null)
        {
            Vector3 aimDir = (LastTargetPoint - firePoint.position);
            if (aimDir.sqrMagnitude > 0.0001f)
            {
                Quaternion fpRot = Quaternion.LookRotation(aimDir.normalized, Vector3.up);
                firePoint.rotation = smooth > 0f
                    ? Quaternion.Slerp(firePoint.rotation, fpRot, Time.deltaTime * (smooth * 0.8f))
                    : fpRot;
            }
        }
    }
}
