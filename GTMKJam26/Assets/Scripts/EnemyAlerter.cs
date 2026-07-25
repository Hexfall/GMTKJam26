using System.Collections.Generic;
using UnityEngine;

public class EnemyAlerter : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] bool alertOnTriggerEnter = true;

    void OnTriggerEnter(Collider other)
    {
        if (!alertOnTriggerEnter)
            return;
        if (other.CompareTag("Player"))
            Alert();            
    }

    public void Alert()
    {
        foreach (var enemy in enemies)
            enemy.Status = Enemy.AgentStatus.Hunting;
    }
}
