using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    public void EnterState(EnemyController enemy)
    {
        enemy.Animator.SetTrigger(enemy.IsDeadHash);
        enemy.enabled = false; 
    }

    public void UpdateState(EnemyController enemy) { }
    public void ExitState(EnemyController enemy) { }
}
