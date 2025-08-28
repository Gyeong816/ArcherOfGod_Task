using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVictoryState : IEnemyState
{
    public void EnterState(EnemyController enemy)
    {
        enemy.transform.localScale = new Vector3(1, 1, 1);
        enemy.Animator.SetTrigger(enemy.VictoryHash);
        enemy.Rigidbody2D.velocity = Vector2.zero; 
    }

    public void UpdateState(EnemyController enemy) { }
    public void ExitState(EnemyController enemy) { }
}
