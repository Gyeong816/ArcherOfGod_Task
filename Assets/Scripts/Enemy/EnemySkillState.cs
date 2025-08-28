using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillState : IEnemyState
{
    private bool _isActive;
    private float _animLength;
    private float _timer;
    private float _spawnTime;
    private BaseSkill _skill;
    private EnemyController _enemy;
    private CombatObjectPool _pool;
    
    public void EnterState(EnemyController enemy)
    {
        int randomNum = UnityEngine.Random.Range(0,enemy.SkillManager.EnemySkillCount); 
        
        _enemy = enemy;
        _pool = enemy.CombatObjectPool;
        _skill = enemy.SkillManager.GetEnemySkill(randomNum);
        _spawnTime = _skill.spawnTime;
        
        Animator anim = enemy.Animator;
        string animName = _skill.animName;
        anim.SetTrigger(animName);
        
        _animLength = 0f; 
        _timer = 0f;
        _isActive = false;
        
    }

    public void UpdateState(EnemyController enemy)
    {
        if (_animLength <= 0f)
        {
            AnimatorStateInfo state = enemy.Animator.GetCurrentAnimatorStateInfo(0);
            _animLength = state.length;
        }

        _timer += Time.deltaTime;

        float normalizedTime = _timer / _animLength; 
        if (!_isActive && normalizedTime >= _spawnTime)
        {
            ActiveSkill();
        }
        
        if (_timer >= _animLength)
        {
            enemy.ChangeState(new EnemyPatrolState());
        }
    }

    public void ExitState(EnemyController enemy)
    {
        _isActive = false;
        _timer = 0f;
    }
    
    private void ActiveSkill()
    {
        if (_isActive)
            return;

        Transform target = GameManager.Instance.EnemyTarget;
        
        _skill.Activate(null, _enemy, _pool, CharacterType.Enemy, target);
        
        _isActive = true;
    }
}
