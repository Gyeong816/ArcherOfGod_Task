using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] private SkillManager skillManager;
    [Header("활 설정")]
    [SerializeField] private CombatObjectPool combatObjectPool;
    [SerializeField] private Transform shootPoint;   
    [SerializeField] private Transform playerTargetTransform;
    
    public Transform targetTransform;
    public Transform ShootPoint => shootPoint;
    
    public SkillManager SkillManager => skillManager;
    
    public float moveDurationMax = 4f;
    public float moveDurationMin = 1f;
    public float attackDurationMax = 4f;
    public float attackDurationMin = 1f;
    public float fireTime = 0.7f;
    public float moveSpeed = 2f;
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public int IsWalkingHash { get; private set; }
    public int IsDeadHash { get; private set; }
    public int VictoryHash { get; private set; }

    private IEnemyState _currentState;
    
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        IsWalkingHash = Animator.StringToHash("IsWalking");
        IsDeadHash = Animator.StringToHash("IsDead");
        VictoryHash = Animator.StringToHash("Victory");
    }

    private void Start()
    {
        ChangeState(new EnemyPatrolState()); 
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
        if (playerTargetTransform == null) return;
        
        GameObject arrowObj = combatObjectPool.Get(PoolType.Arrow);
        arrowObj.transform.position = shootPoint.position;
        arrowObj.transform.rotation = Quaternion.identity;

        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.Initialize(playerTargetTransform.position, combatObjectPool);
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
