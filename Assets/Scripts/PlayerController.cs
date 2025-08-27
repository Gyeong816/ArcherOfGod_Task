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
    [SerializeField] private Transform enemyTargetTransform;
    
    public Transform  targetTransform;
    public Transform  ShootPoint => shootPoint;
    
    private bool _hasFired = false;
    private Rigidbody2D _rigidbody2D;
    private float _moveInput;
    private Animator _animator;
    private int _isWalkingHash;
    private int _isDeadHash;
    private int _victory;
    private bool _isDead;
    private bool _isWon;
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
        _moveInput = Input.GetAxisRaw("Horizontal");
       
        
        if (_moveInput != 0f)
        {
            Move();  
        }
        else
        {
            Attack(); 
        }
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = new Vector2(_moveInput * moveSpeed, _rigidbody2D.velocity.y);
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
        if (enemyTargetTransform == null) return;
        
        GameObject arrowObj = combatObjectPool.Get(PoolType.Arrow);
        arrowObj.transform.position = shootPoint.position;
        arrowObj.transform.rotation = Quaternion.identity;
        
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.Initialize(enemyTargetTransform.position, combatObjectPool);
    }

    public void Die()
    {
        if(_isDead || _isWon) 
            return;
        _animator.SetTrigger(_isDeadHash);
        _isDead = true;
    }

    public void OnVictory()
    {
        _isWon = true;
        _animator.SetTrigger(_victory);
    }
}
