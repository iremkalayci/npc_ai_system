using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Enemy Health: {health}");

        if (health <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("💀 Enemy öldü!");
        Destroy(gameObject);
    }
}
