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
    
    [Header("상태이상: 화상")]
    [SerializeField] private int burnDamage = 3;
    [SerializeField] private float burnInterval = 1f;
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private GameObject burnEffect;

    [Header("상태이상: 동상")]
    [SerializeField] private float freezeDuration = 2f;
    [SerializeField] private float freezeSlowMultiplier = 0.3f;
    [SerializeField] private GameObject freezeEffect;

    private bool _isBurning = false;
    private bool _isFrozen = false;
    private float _originalMoveSpeed;
    
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
    
    public void ApplyBurn()
    {
        if (!_isBurning)
            StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        _isBurning = true;
        if (burnEffect != null) burnEffect.SetActive(true);

        float elapsed = 0f;
        while (elapsed < burnDuration)
        {
            CombatSystem.Instance.DealDamage(gameObject, burnDamage);
            yield return new WaitForSeconds(burnInterval);
            elapsed += burnInterval;
        }

        if (burnEffect != null) burnEffect.SetActive(false);
        _isBurning = false;
    }

    public void ApplyFreeze()
    {
        if (!_isFrozen)
            StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        _isFrozen = true;
        if (freezeEffect != null) freezeEffect.SetActive(true);

        _originalMoveSpeed = moveSpeed;
        moveSpeed *= freezeSlowMultiplier;   // 이동 속도 감소

        yield return new WaitForSeconds(freezeDuration);

        moveSpeed = _originalMoveSpeed;      // 복구
        if (freezeEffect != null) freezeEffect.SetActive(false);
        _isFrozen = false;
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GroundFire"))
        {
            ApplyBurn();
        }
        if (other.CompareTag("GroundIce"))
        {
            ApplyFreeze();
        }
    }
    
    
}
