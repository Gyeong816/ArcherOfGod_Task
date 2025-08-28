using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : IEnemyState, IEnemyPhysicsState
{
    private float _moveDir = 1f;
    private float _timer = 0f;
    private float _moveDuration;
    
    private float _waitTimer = 0.5f; 
    private bool _isWaiting = true;

    public void EnterState(EnemyController enemy)
    {
        enemy.Animator.SetBool(enemy.IsWalkingHash, true);
        _timer = 0f;
        _moveDuration = UnityEngine.Random.Range(enemy.moveDurationMin, enemy.moveDurationMax);
    }

    public void UpdateState(EnemyController enemy)
    {
        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
            }
            return; 
        }
        
        _timer += Time.deltaTime;
        
        if (_moveDir > 0)
            enemy.transform.localScale = new Vector3(-1, 1, 1);
        else if (_moveDir < 0)
            enemy.transform.localScale = new Vector3(1, 1, 1);
        
        if (_timer >= _moveDuration)
        {
            _timer = 0f;
            enemy.ChangeState(new EnemyAttackState());
        }
    }
    
    public void FixedUpdateState(EnemyController enemy)
    {
        if (_isWaiting)
        {
            return;
        }
        
        enemy.Rigidbody2D.velocity = new Vector2(_moveDir * enemy.moveSpeed, enemy.Rigidbody2D.velocity.y);
    }

    public void ExitState(EnemyController enemy)
    {
        enemy.Rigidbody2D.velocity = Vector2.zero;
        enemy.Animator.SetBool(enemy.IsWalkingHash, false);
    }
    
    public void ReverseDirection()
    {
        _moveDir *= -1f;
    }
}
