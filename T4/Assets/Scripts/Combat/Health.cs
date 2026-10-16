
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    // Events
    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnDamaged;
    public event Action OnDied;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        float actualDamage = Mathf.Min(damage, CurrentHealth);

        CurrentHealth -= actualDamage;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnDamaged?.Invoke(actualDamage);

        if (IsDead)
        {
            OnDied?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        float previousHealth = CurrentHealth;

        CurrentHealth = Mathf.Min(
            CurrentHealth + amount,
            maxHealth
        );

        if (CurrentHealth != previousHealth)
        {
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}

