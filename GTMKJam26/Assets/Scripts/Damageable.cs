using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;
    public float health = 100f;

    public void Damage(float damage)
    {
        OnDamage.Invoke();
        health -= damage;
        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        OnDeath.Invoke();
    }
}
