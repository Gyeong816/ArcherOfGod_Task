using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("활 설정")]
    [SerializeField] private CombatObjectPool combatObjectPool;
    [SerializeField] private Transform shootPoint;   
    [SerializeField] private float fireTime = 0.7f;
    
    
    [SerializeField] private int burnDamage = 5;    
    [SerializeField] private float burnInterval = 1f; 
    [SerializeField] private float burnDuration = 5f; 
    [SerializeField] private GameObject burnEffect; 
    
    [SerializeField] private float freezeDuration = 3f; 
    [SerializeField] private float freezeSlowMultiplier = 0.5f; 
    [SerializeField] private GameObject freezeEffect;
    
    
    public Transform  targetTransform;
    public Transform  ShootPoint => shootPoint;
    public Animator Animator => _animator;
    public Rigidbody2D Rigidbody2D => _rigidbody2D;
    
    private bool _isBurning = false;
    private bool _hasFired = false;
    private Rigidbody2D _rigidbody2D;
    private float _moveInput;
    private Animator _animator;
    private int _isWalkingHash;
    private int _isDeadHash;
    private int _victory;
    private Queue<ICommand> skillQueue = new Queue<ICommand>();
    private bool isExecutingSkill = false;
    private bool _isFrozen = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isDeadHash = Animator.StringToHash("IsDead");
        _victory = Animator.StringToHash("Victory");
    }

    private void Update()
    {
        if (isExecutingSkill)
        { ; 
            return;
        }
        
        _moveInput = Input.GetAxisRaw("Horizontal");
        
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
    
    private void FixedUpdate()
    {
        _rigidbody2D.velocity = new Vector2(_moveInput * moveSpeed, _rigidbody2D.velocity.y);
    }
    
    public void EnqueueSkill(ICommand skillCommand)
    {
        skillQueue.Enqueue(skillCommand);
    }
    
    private IEnumerator ProcessSkill(ICommand skill)
    {
        isExecutingSkill = true;
        yield return StartCoroutine(skill.Execute(this));
        isExecutingSkill = false;
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
        burnEffect.SetActive(false);
        freezeEffect.SetActive(false);
        _animator.SetTrigger(_isDeadHash);
    }

    public void OnVictory()
    {
        _animator.SetTrigger(_victory);
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
