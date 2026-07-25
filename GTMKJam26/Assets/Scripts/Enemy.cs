using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum AgentStatus
    {
        Idle,
        Hunting,
        Attacking,
        Staggered,
        Dead,
    }
    private AgentStatus _status = AgentStatus.Idle;

    public AgentStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            switch (value)
            {
                case AgentStatus.Idle:
                    // TODO: Play Idle animation
                    break;
                case AgentStatus.Hunting:
                    // TODO: Play Walking animation
                    break;
                case  AgentStatus.Attacking:
                    attackTime = 0.0f;
                    // TODO: Play Attack animation
                    // AUDIO TODO: play start of attack audio
                    break;
                case AgentStatus.Staggered:
                    staggeredTime = 0.0f;
                    // TODO: Play Stagger animation
                    // AUDIO TODO: Play start of "take damage" audio
                    break;
                case AgentStatus.Dead:
                    // TODO: Play death animation
                    // AUDIO TODO: Play death audio
                    break;
                default:
                    break;
            }
            if (value != AgentStatus.Hunting)
                agent.SetDestination(transform.position);
            LogStatus();
        }
    }
    
    [Header("Hunting")]
    private GameObject player;
    private NavMeshAgent agent;

    [Header("Defending")]
    public float staggerDuration = 1f;
    private float staggeredTime = 0f;
    
    [Header("Attacking")]
    public float attackRange = 5f;

    private float attackRangeSqr = 25f;
    public float attackDuration = 2f;
    private float attackTime = 0f;
    
    void Start()
    {
        attackRangeSqr = attackRange * attackRange;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void HuntPlayer()
    {
        Status =  AgentStatus.Hunting;
    }

    void FixedUpdate()
    {
        switch (Status)
        {
            case AgentStatus.Idle:
                Idle();
                break;
            case AgentStatus.Hunting:
                Hunt();
                break;
            case  AgentStatus.Attacking:
                Attack();
                break;
            case AgentStatus.Staggered:
                Stagger();
                break;
        }
    }

    void Idle()
    {
        agent.SetDestination(transform.position);
    }

    void Hunt()
    {
        agent.SetDestination(player.transform.position);
        if (attackRangeSqr > (transform.position - player.transform.position).sqrMagnitude)
            Status = AgentStatus.Attacking;
    }

    void Attack()
    {
        attackTime += Time.fixedDeltaTime;
        if (!(attackTime > attackDuration))
            return;
        
        Status = AgentStatus.Hunting;
        Debug.Log("Player was attacked");
        // TODO: Make player take damage.
        // AUDIO TODO: Play attack landing audio
    }

    void Stagger()
    {
        staggeredTime += Time.fixedDeltaTime;
        if (!(staggeredTime > staggerDuration))
            return;

        Status = AgentStatus.Hunting;
    }

    public void Damage()
    {
        if (Status != AgentStatus.Attacking)
            Status = AgentStatus.Staggered;
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }

    private void LogStatus() => Debug.Log($"Enemy '{gameObject.name}' is now in {Status} state.");
}
