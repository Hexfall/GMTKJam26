using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void FinishDeath()
    {
        if(enemy != null)
        {
            enemy.FinishDeath();
        }
    }
}
