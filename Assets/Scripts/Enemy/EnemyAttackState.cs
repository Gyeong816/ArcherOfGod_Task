using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private bool _hasFired = false;
    private float _attackDuration = 2f;
    private float _timer = 0f;

    public void EnterState(EnemyController enemy)
    {
        enemy.transform.localScale = new Vector3(1, 1, 1);
        enemy.Animator.SetBool(enemy.IsWalkingHash, false);
        _timer = 0f;
        _hasFired = false;

        _attackDuration = UnityEngine.Random.Range(enemy.attackDurationMin, enemy.attackDurationMax);
    }

    public void UpdateState(EnemyController enemy)
    {
        AnimatorStateInfo state = enemy.Animator.GetCurrentAnimatorStateInfo(0);
        float cycleTime = state.normalizedTime % 1f;

        if (state.IsName("attack"))
        {
            if (!_hasFired && cycleTime >= enemy.fireTime)
            {
                _hasFired = true;
                enemy.FireArrow();
            }
            if (_hasFired && cycleTime < enemy.fireTime)
            {
                _hasFired = false;
            }
        }

        _timer += Time.deltaTime;
        if (_timer >= _attackDuration)
        {
            enemy.ChangeState(new EnemySkillState()); 
        }
    }

    public void ExitState(EnemyController enemy) { }
}
