using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightArrowSkill : BaseSkill
{
    public override void Activate(PlayerController playerController, EnemyController enemyController, CombatObjectPool combatObjectPool, CharacterType characterType)
    {
        
        if (characterType == CharacterType.Player)
        {
            GameObject straightArrow = combatObjectPool.Get(PoolType.StraightArrow);
            straightArrow.transform.position = playerController.ShootPoint.position;
            straightArrow.transform.localScale = new Vector3(1, 1, 1);
            StraightArrow arrow = straightArrow.GetComponent<StraightArrow>();
            arrow.Initialize(combatObjectPool, Vector3.right);
        }
        else if (characterType == CharacterType.Enemy)
        {
            GameObject straightArrow = combatObjectPool.Get(PoolType.StraightArrow);
            straightArrow.transform.position = enemyController.ShootPoint.position;
            straightArrow.transform.localScale = new Vector3(-1, 1, 1);
            StraightArrow arrow = straightArrow.GetComponent<StraightArrow>();
            arrow.Initialize(combatObjectPool, Vector3.left);
        }
    }
}
