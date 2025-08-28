using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] private SkillManager skillManager;
    [Header("활 설정")]
    [SerializeField] private CombatObjectPool combatObjectPool;
    [SerializeField] private Transform shootPoint;   
    public Transform ShootPoint => shootPoint;
    
    public SkillManager SkillManager => skillManager;

    public CombatObjectPool CombatObjectPool => combatObjectPool;
    
    public float moveDurationMax = 4f;
    public float moveDurationMin = 1f;
    public float attackDurationMax = 4f;
    public float attackDurationMin = 1f;
    public float fireTime = 0.7f;
    public float moveSpeed = 2f;

    public int testSkillnum;
    
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator Animator { get; private set; }
    public int IsWalkingHash { get; private set; }
    public int IsDeadHash { get; private set; }
    public int VictoryHash { get; private set; }

    private IEnemyState _currentState;
    private Collider2D _collider;
    
    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        IsWalkingHash = Animator.StringToHash("IsWalking");
        IsDeadHash = Animator.StringToHash("IsDead");
        VictoryHash = Animator.StringToHash("Victory");
    }

    private void Start()
    {
        ChangeState(new EnemyAttackState()); 
    }
    private void Update()
    {
        _currentState?.UpdateState(this);
    }
    
    private void FixedUpdate()
    {
        if (_currentState is IEnemyPhysicsState physicsState)
        {
            physicsState.FixedUpdateState(this);
        }
    }
    public void ChangeState(IEnemyState newState)
    {
        _currentState?.ExitState(this);
        _currentState = newState;
        _currentState?.EnterState(this);
    }

    public void FireArrow()
    {
        GameObject arrowObj = combatObjectPool.Get(PoolType.Arrow);
        arrowObj.transform.position = shootPoint.position;

        Arrow arrow = arrowObj.GetComponent<Arrow>();
        Transform target = GameManager.Instance.EnemyTarget;
        
        arrow.Initialize(target.position, combatObjectPool, OwnerType.Enemy);
    }

    public void Die()
    {
        ChangeState(new EnemyDeadState());
    }
    
    public void OnVictory()
    {
        ChangeState(new EnemyVictoryState());
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (_currentState is EnemyPatrolState patrolState)
            {
                Debug.Log("벽 부딫힘");
                patrolState.ReverseDirection();
            }
        }
    }
    
    
    
}
