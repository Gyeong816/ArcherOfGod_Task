using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    [SerializeField] private SkillManager skillManager;
    [Header("활 설정")]
    [SerializeField] private CombatObjectPool combatObjectPool;
    [SerializeField] private Transform shootPoint; 
    [SerializeField] private ArmorType currentArmor = ArmorType.None;
    public ArmorType CurrentArmor => currentArmor;
    public Transform ShootPoint => shootPoint;
    
    public SkillManager SkillManager => skillManager;

    public CombatObjectPool CombatObjectPool => combatObjectPool;
    
    public float moveDurationMax = 4f;
    public float moveDurationMin = 1f;
    public float attackDurationMax = 5f;
    public float attackDurationMin = 3f;
    public float fireTime = 0.7f;
    public float moveSpeed = 2f;

    //public int testSkillnum;
    
    public Rigidbody2D Rigidbody2D { get; private set; }
    public Animator Animator { get; private set; }
    public int IsWalkingHash { get; private set; }
    public int IsDeadHash { get; private set; }
    public int VictoryHash { get; private set; }

    private IEnemyState _currentState;
    private Collider2D _collider;
    private bool _isGameStarted = false;
    
    [Header("상태이상: 화상")]
    [SerializeField] private int burnDamage = 3;
    [SerializeField] private float burnInterval = 1f;
    [SerializeField] private float burnDuration = 5f;
    [SerializeField] private GameObject burnEffect;

    [Header("상태이상: 동상")]
    [SerializeField] private float freezeDuration = 3f;
    [SerializeField] private float freezeSlowMultiplier = 0.3f;
    [SerializeField] private GameObject freezeEffect;

    private bool _isBurning = false;
    private bool _isFrozen = false;
    private float _originalMoveSpeed;
    private int _attackHash;
    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        IsWalkingHash = Animator.StringToHash("IsWalking");
        IsDeadHash = Animator.StringToHash("IsDead");
        VictoryHash = Animator.StringToHash("Victory");
        _attackHash = Animator.StringToHash("Attack"); 
    }


    private void Update()
    {
        if (!_isGameStarted) return; 
        _currentState?.UpdateState(this);
    }
    
    private void FixedUpdate()
    {
        if (!_isGameStarted) return; 
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

    public void SetNerf(AttackBuffType attackType)
    {
        switch (attackType)
        {
            case AttackBuffType.Fire:
                burnDuration *= 2;
                break;
            case AttackBuffType.Ice:
                freezeDuration *= 2;
                break;
        }
    }
    
    public void SetArmor(ArmorType armor)
    {
        currentArmor = armor;
    }

    public void StartGame()
    {
        _isGameStarted = true;
        Animator.SetTrigger(_attackHash);    
        ChangeState(new EnemyAttackState());  
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
        _isGameStarted = false;
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
        moveSpeed *= freezeSlowMultiplier;   

        yield return new WaitForSeconds(freezeDuration);

        moveSpeed = _originalMoveSpeed;    
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
            if (currentArmor == ArmorType.FireImmunity)
                return; 
            ApplyBurn();
        }
        if (other.CompareTag("GroundIce"))
        {
            if (currentArmor == ArmorType.IceImmunity)
                return; 
            ApplyFreeze();
        }
        
        if (_currentState is EnemyPatrolState patrolState)
        {
            if (other.CompareTag("GroundFire") || other.CompareTag("GroundIce"))
            {
                patrolState.ReverseDirection();
            }
        }
    }
    
  
    
}
