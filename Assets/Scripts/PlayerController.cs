using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 설정")]
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D _rigidbody2D;
    private float _moveInput;
    private Animator _animator;

    private int _isWalkingHash;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _isWalkingHash = Animator.StringToHash("IsWalking");
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
    }
    
    
}
