using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyPlaceholder : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private Rigidbody rb;

    public Rigidbody Rigidbody => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    public void ApplyKnockback(Vector3 force, float damage = 0f)
    {
        rb.AddForce(force, ForceMode.Impulse);
        if (damage > 0f)
        {
            TakeDamage(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= Mathf.RoundToInt(amount);
        Debug.Log($"{name} took {amount} damage (health now {currentHealth}).");
        if (currentHealth <= 0)
        {
            Debug.Log($"{name} health depleted (placeholder - no death handling implemented yet).");
        }
    }
}
