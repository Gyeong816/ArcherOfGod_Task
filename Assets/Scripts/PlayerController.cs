using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 이동 설정")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("이동 버튼")]
    [SerializeField] private HoldButton leftButton;
    [SerializeField] private HoldButton rightButton;

    [Header("활 설정")]
    [SerializeField] private CombatObjectPool combatObjectPool;
    [SerializeField] private Transform shootPoint;   
    [SerializeField] private float fireTime = 0.6f;

    [Header("방어구 및 상태")]
    [SerializeField] private ArmorType currentArmor = ArmorType.None;

    [Header("불 상태이상")]
    [SerializeField] private int burnDamage = 20;    
    [SerializeField] private float burnInterval = 1f; 
    [SerializeField] private float burnDuration = 5f; 
    [SerializeField] private GameObject burnEffect; 

    [Header("얼음 상태이상")]
    [SerializeField] private float freezeDuration = 5f; 
    [SerializeField] private float freezeSlowMultiplier = 0.3f; 
    [SerializeField] private GameObject freezeEffect;

    // === 공개 프로퍼티 ===
    public Transform targetTransform;
    public Transform ShootPoint => shootPoint;
    public Animator Animator => _animator;
    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    // === 내부 상태 플래그 ===
    private bool _isBurning = false;
    private bool _hasFired = false;
    private bool _isExecutingSkill = false;
    private bool _isFrozen = false;
    private bool _isGameStarted = false;

    // === 컴포넌트 캐싱 ===
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    // === 애니메이터 해시값 ===
    private int _isWalkingHash;
    private int _isDeadHash;
    private int _victory;
    private int _attack;

    // === 스킬 관련 ===
    private Queue<ICommand> skillQueue = new Queue<ICommand>();

    // === 입력 처리 ===
    private float _moveInput;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isDeadHash = Animator.StringToHash("IsDead");
        _victory = Animator.StringToHash("Victory");
        _attack = Animator.StringToHash("Attack");
    }

    private void Update()
    {
        if (_isExecutingSkill || ! _isGameStarted)
        { 
            return;
        }
        
        float axisInput = Input.GetAxisRaw("Horizontal");
        
        float buttonInput = 0f;
        if (leftButton.IsPressed) buttonInput = -1f;
        if (rightButton.IsPressed) buttonInput = 1f;
        
        _moveInput = axisInput != 0 ? axisInput : buttonInput;
        
        if (_moveInput != 0f)
        {
            Move();  
        }
        else
        {
            if (skillQueue.Count > 0)
            {
                StartCoroutine(ProcessSkill(skillQueue.Dequeue()));
            }
            else
            {
                Attack();
            }
        }
    }
    
    public void SetArmor(ArmorType armor)
    {
        currentArmor = armor;
    }
    
    private void FixedUpdate()
    {
        if (_isExecutingSkill || ! _isGameStarted)
        { 
            return;
        }
        
        _rigidbody2D.velocity = new Vector2(_moveInput * moveSpeed, _rigidbody2D.velocity.y);
    }
    
    public void StartGame()
    {
        _animator.SetTrigger(_attack);
        _isGameStarted = true;
    }
    public void EnqueueSkill(ICommand skillCommand)
    {
        skillQueue.Enqueue(skillCommand);
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
    private IEnumerator ProcessSkill(ICommand skill)
    {
        _isExecutingSkill = true;
        yield return StartCoroutine(skill.Execute(this));
        _isExecutingSkill = false;
    }
    
    private void Move()
    {
        if (_moveInput > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (_moveInput < 0)
            transform.localScale = new Vector3(1, 1, 1);
        
        _animator.SetBool(_isWalkingHash, true);
    }

    private void Attack()
    {
        transform.localScale = new Vector3(-1, 1, 1); 
        _animator.SetBool(_isWalkingHash, false);
        
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        
        float cycleTime = state.normalizedTime % 1f; 

        if (state.IsName("attack"))
        {
            if (!_hasFired && cycleTime >= fireTime)
            {
                _hasFired = true;
                FireArrow(); 
            }
            if (_hasFired && cycleTime < fireTime)
            {
                _hasFired = false;
            }
        }
        else
        {
            _hasFired = false;
        }
    
    }
    private void FireArrow()
    {
        GameObject arrowObj = combatObjectPool.Get(PoolType.Arrow);
        arrowObj.transform.position = shootPoint.position;

        Arrow arrow = arrowObj.GetComponent<Arrow>();
        Transform target = GameManager.Instance.PlayerTarget;
        arrow.Initialize(target.position, combatObjectPool, OwnerType.Player);
    }
    
    private void ApplyFreeze()
    {
        if (!_isFrozen)
            StartCoroutine(FreezeCoroutine());
    }

    private IEnumerator FreezeCoroutine()
    {
        _isFrozen = true;
        freezeEffect.SetActive(true);

        float originalSpeed = moveSpeed;             
        moveSpeed *= freezeSlowMultiplier;          

        yield return new WaitForSeconds(freezeDuration);

        moveSpeed = originalSpeed;               
        freezeEffect.SetActive(false);
        _isFrozen = false;
    }

    private void ApplyBurn()
    {
        if (!_isBurning)  
            StartCoroutine(BurnCoroutine());
        
    }

    private IEnumerator BurnCoroutine()
    {
        _isBurning = true;
        burnEffect.SetActive(true);
        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            CombatSystem.Instance.DealDamage(gameObject, burnDamage);

            yield return new WaitForSeconds(burnInterval);
            elapsed += burnInterval;
        }
        burnEffect.SetActive(false);
        _isBurning = false;
    }
    public void Die()
    {
        _isGameStarted = false;
        burnEffect.SetActive(false);
        freezeEffect.SetActive(false);
        _animator.SetTrigger(_isDeadHash);
    }

    public void OnVictory()
    {
        _isGameStarted = false;
        _animator.SetTrigger(_victory);
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
    }
}
