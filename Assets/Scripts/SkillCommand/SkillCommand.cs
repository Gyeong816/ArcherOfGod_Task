using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCommand : ICommand
{
    private BaseSkill _skill;
    private CharacterType _characterType;
    private PlayerController _player;
    private EnemyController _enemy;
    private CombatObjectPool _pool;
    private string _animHash;
    private bool _isActive;

    public SkillCommand(BaseSkill skill, CharacterType characterType, PlayerController player, EnemyController enemy, CombatObjectPool pool, string animHash)
    {
        _skill = skill;
        _characterType = characterType;
        _player = player;
        _enemy = enemy;
        _pool = pool;
        _animHash = animHash;
    }

    public IEnumerator Execute(PlayerController playerController)
    {
        Animator anim = playerController.Animator;
        anim.SetTrigger(_animHash);

        yield return null; 

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        float animLength = state.length;
        float timer = 0f;

        while (timer < animLength)
        {
            timer += Time.deltaTime;

            float normalizedTime = timer / animLength; 
            if (!_isActive && normalizedTime >= _skill.spawnTime)
            {
                ActiveSkill();
            }

            yield return null;
        }
    }

    private void ActiveSkill()
    {
        if (_isActive)
            return;
        
        _skill.Activate(_player, _enemy, _pool, _characterType);
        
        _isActive = true;
    }
}
