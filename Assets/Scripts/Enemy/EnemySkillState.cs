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
    private PlayerController _player;
    private EnemyController _enemy;
    private CombatObjectPool _pool;
    private CharacterType _characterType;
    
    public void EnterState(EnemyController enemy)
    {
        Animator anim = enemy.Animator;
        
   
        _skill = enemy.SkillManager.GetEnemySkill(1);
        _spawnTime = _skill.spawnTime;
        
        string animName = _skill.animName;
        anim.SetTrigger(animName);
        
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        _animLength = state.length;
        _timer = 0f;
        
    }

    public void UpdateState(EnemyController enemy)
    {
        _timer += Time.deltaTime;

        float normalizedTime = _timer / _animLength; 
        if (!_isActive && normalizedTime >= _spawnTime)
        {
            ActiveSkill();
        }
    }

    public void ExitState(EnemyController enemy)
    {
        throw new System.NotImplementedException();
    }
    
    private void ActiveSkill()
    {
        if (_isActive)
            return;
        
        _skill.Activate(null, _enemy, _pool, _characterType);
        
        _isActive = true;
    }
}
