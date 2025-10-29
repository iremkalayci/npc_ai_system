using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"🔥 Player Health: {health}");

        if (health <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("💀 Player öldü!");
        gameObject.SetActive(false);
    }
}
