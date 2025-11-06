using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Ayarları")]
    [Min(1f)] public float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    
    public event Action<float, float> OnHealthChanged; 
    public event Action OnDied;

    void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        CurrentHealth = maxHealth;
        IsDead = false;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        SetHealth(CurrentHealth - amount);

        if (!IsDead && CurrentHealth <= 0f)
        {
            IsDead = true;
            OnDied?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        SetHealth(CurrentHealth + amount);
    }

    public void SetHealth(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, maxHealth);
        if (!Mathf.Approximately(clamped, CurrentHealth))
        {
            CurrentHealth = clamped;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
