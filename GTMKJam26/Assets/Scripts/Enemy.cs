using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private static readonly int StateParameter =
        Animator.StringToHash("State");

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
            if (_status == value)
                return;

            AgentStatus previousStatus = _status;
            _status = value;

            if (animator != null)
                animator.SetInteger(StateParameter, (int)value);

            switch (value)
            {
                case AgentStatus.Idle:
                    break;
                case AgentStatus.Hunting:
                    ResumeMoving();
                    if (previousStatus == AgentStatus.Idle)
                        enemySFX?.PlayAlertSound();
                    break;
                case  AgentStatus.Attacking:
                    attackTime = 0.0f;
                    enemySFX?.PlayAttackSound();
                    break;
                case AgentStatus.Staggered:
                    staggeredTime = 0.0f;
                    // AUDIO TODO: Play start of "take damage" audio
                    break;
                case AgentStatus.Dead:
                    enemySFX?.PlayDeathSound();
                    break;
                default:
                    break;
            }

            if (value != AgentStatus.Hunting)
                StopMoving();

            LogStatus();
        }
    }

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private EnemySFX enemySFX;
    
    [Header("Hunting")]
    private GameObject player;
    private NavMeshAgent agent;

    [Header("Defending")]
    [SerializeField][Range(0, 5)] private float staggerDuration = 1f;
    private float staggeredTime = 0f;
    
    [Header("Attacking")]
    [SerializeField][Range(0, 10)] private float attackRange = 5f;
    [SerializeField][Range(0, 100)] private float damageDealt = 20f;
    [SerializeField][Range(0, 5)] private float attackDuration = 2f;

    private float attackRangeSqr = 25f;
    private float attackTime = 0f;

    private void Awake()
    {
        enemySFX = GetComponent<EnemySFX>();
    }
    
    void Start()
    {
        attackRangeSqr = attackRange * attackRange;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.SetInteger(StateParameter, (int)Status);
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
        var dmg = player.GetComponent<Damageable>();
        if (dmg != null)
        {
            enemySFX?.PlayAttackHitPlayerSound(player.transform.position);
            dmg.Damage(damageDealt);
        }
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
        if (Status == AgentStatus.Dead)
            return;

        enemySFX?.PlayPainSound();

        if (Status != AgentStatus.Attacking)
            Status = AgentStatus.Staggered;
    }
    
    public void Die()
    {
        if (Status == AgentStatus.Dead)
            return;

        Status = AgentStatus.Dead;
    }

    public void FinishDeath()
    {
        Destroy(gameObject);
    }

    private void StopMoving()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    private void ResumeMoving()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
    }

    private void LogStatus() => Debug.Log($"Enemy '{gameObject.name}' is now in {Status} state.");
}
