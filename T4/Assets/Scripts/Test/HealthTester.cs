
using UnityEngine;
using UnityEngine.InputSystem;

public class HealthTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [Header("Test Settings")]
    [SerializeField] private float testDamage = 25f;
    [SerializeField] private float testHealing = 20f;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (health == null) return;

        health.OnHealthChanged += HandleHealthChanged;
        health.OnDamaged += HandleDamaged;
        health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (health == null) return;

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnDamaged -= HandleDamaged;
        health.OnDied -= HandleDeath;
    }

    private void Update()
    {
        if (health == null || Keyboard.current == null)
            return;

        // T: Damage
        if (Keyboard.current.tKey.wasPressedThisFrame)
            health.TakeDamage(testDamage);

        // Y: Heal
        if (Keyboard.current.yKey.wasPressedThisFrame)
            health.Heal(testHealing);
    }

    private void HandleHealthChanged(float current, float max)
    {
        Debug.Log($"Health: {current}/{max}");
    }

    private void HandleDamaged(float damage)
    {
        Debug.Log($"Damage received: {damage}");
    }

    private void HandleDeath()
    {
        Debug.Log("ENTITY DIED");
    }
}
