using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(EnemyPerception))]
public class EnemyDecisionBridge : MonoBehaviour
{
    private EnemyAI ai;
    private EnemyPerception perception;

    [Header("Decision Tuning")]
    public float chaseDistance = 20f;  // Algıladıktan sonra kovalamaya başlama
    public float attackDistance = 10f; // Bu mesafede saldır

    void Start()
    {
        ai = GetComponent<EnemyAI>();
        perception = GetComponent<EnemyPerception>();
    }

    void Update()
    {
        if (ai == null || perception == null) return;
        if (perception.target == null) return;

        float distance = Vector3.Distance(transform.position, perception.target.position);

        if (perception.CanSeeTargetNow)
        {
            if (distance <= attackDistance)
            {
                ai.SendMessage("AttackPlayer", SendMessageOptions.DontRequireReceiver);
            }
            else if (distance <= chaseDistance)
            {
                ai.SendMessage("ChasePlayer", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (perception.RecentlySeen)
        {
            ai.SendMessage("ChasePlayer", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            ai.SendMessage("Patrol", SendMessageOptions.DontRequireReceiver);
        }
    }
}
