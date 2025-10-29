using UnityEngine;
using System.Collections;

public class EnemyPerception : MonoBehaviour
{
    [Header("FOV (Görüş)")]
    [Tooltip("NPC'nin merkezden kaç metreyi görebildiği")]
    public float viewRadius = 18f;

    [Tooltip("Toplam görüş açısı (derece) – örn: 120")]
    [Range(1f, 360f)] public float viewAngle = 120f;

    [Tooltip("Baş hizasında bir nokta (ray buradan atılacak)")]
    public Transform eye;                         

    [Header("Katmanlar")]
    [Tooltip("Hedef oyuncu katmanları")]
    public LayerMask targetMask;                
    [Tooltip("Duvar/engel katmanları")]
    public LayerMask obstacleMask;               

    [Header("Hafıza")]
    [Tooltip("Görüş kaybolduktan sonra hatırlama süresi (sn)")]
    public float memorySeconds = 2.0f;

    // Çıktılar (AI'nin okuyacağı alanlar)
    [HideInInspector] public bool targetVisible;
    [HideInInspector] public Transform target;    
    [HideInInspector] public Vector3 lastSeenPosition;
    [HideInInspector] public float lastSeenTime;

    void Start()
    {
        if (target == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) target = playerObj.transform;
        }
        if (eye == null) eye = transform; 
        StartCoroutine(ScanLoop());
    }

    IEnumerator ScanLoop()
    {
        var wait = new WaitForSeconds(0.1f); 
        while (true)
        {
            Scan();
            yield return wait;
        }
    }

    void Scan()
    {
        bool seen = false;

        if (target != null)
        {
            Vector3 toTarget = (target.position - eye.position);
            float dist = toTarget.magnitude;

            if (dist <= viewRadius)
            {
                Vector3 dirNorm = toTarget.normalized;
                float angle = Vector3.Angle(eye.forward, dirNorm);

                if (angle <= viewAngle * 0.5f)
                {
                    // Line of sight: önünde engel var mı?
                    if (!Physics.Raycast(eye.position, dirNorm, dist, obstacleMask))
                    {
                        seen = true;
                        lastSeenPosition = target.position;
                        lastSeenTime = Time.time;
                    }
                }
            }
        }

        targetVisible = seen;
    }

    /// AI tarafından kullanılabilecek yardımcı propertiler
    public bool CanSeeTargetNow => targetVisible;
    public bool RecentlySeen => (Time.time - lastSeenTime) <= memorySeconds;

    
    void OnDrawGizmosSelected()
    {
        if (eye == null) eye = transform;

        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawWireSphere(eye.position, viewRadius);

        
        Vector3 left = DirFromAngle(-viewAngle / 2f);
        Vector3 right = DirFromAngle(+viewAngle / 2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(eye.position, eye.position + left * viewRadius);
        Gizmos.DrawLine(eye.position, eye.position + right * viewRadius);

        
        if (RecentlySeen)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(lastSeenPosition, 0.15f);
        }

    
        if (targetVisible && target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eye.position, target.position);
        }
    }

    Vector3 DirFromAngle(float angleDeg)
    {
        float rad = (eye.eulerAngles.y + angleDeg) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));
    }
}
