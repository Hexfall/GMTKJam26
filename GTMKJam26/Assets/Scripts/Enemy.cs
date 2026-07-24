using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public bool followPlayer;
    public float attackRange = 5f;
    private NavMeshAgent agent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            agent.SetDestination(player.transform.position);
            if (attackRange > (transform.position - player.transform.position).magnitude)
                Attack();
        }
        else
        {
            Idle();
        }
    }

    void Attack()
    {
        Debug.Log("Player was attacked");
    }

    void Idle()
    {
        agent.SetDestination(transform.position);
    }
}
