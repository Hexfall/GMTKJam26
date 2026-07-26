using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Damageable : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float health = 100f;
    private float maxHealth;
    
    [Header("Healing")]
    [SerializeField] private bool heals = false;
    [SerializeField] [Range(0, 10)] private float healingRate = 2.5f;
    [SerializeField] [Range(0, 10)] private float healingDelay = 5f;
    private float sinceLastDamage = 0f;
    private bool isHealing = false;
    
    [Header("Events")]
    [SerializeField] public UnityEvent OnDamage;
    [SerializeField] public UnityEvent OnDeath;
    [SerializeField] public UnityEvent OnStartHealing;
    [SerializeField] public UnityEvent OnFinishHealing;

    private void Start()
    {
        maxHealth = health;
    }

    private void Update()
    {
        if (!heals)
            return;

        if (!isHealing)
        {
            sinceLastDamage += Time.deltaTime;
            if (sinceLastDamage < healingDelay)
                return;
            isHealing = true;
            OnStartHealing.Invoke();
        }
        
        if (health >= maxHealth)
            return;
        
        health = Mathf.Min(health + healingRate * Time.deltaTime, maxHealth);
        if (health >= maxHealth)
            OnFinishHealing.Invoke();
    }
    
    public void Damage(float damage)
    {
        isHealing = false;
        sinceLastDamage = 0f;
        OnDamage.Invoke();
        health -= damage;
        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        OnDeath.Invoke();
    }

    public void PlayerDeath()
    {
        GetComponent<PlayerSFX>()?.PlayDeathSound();
        MusicManager.StopMusic();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
}
