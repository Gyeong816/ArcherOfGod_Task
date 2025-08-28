using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrowSkill : BaseSkill
{
   

    public override void Activate(PlayerController player, EnemyController enemy, CombatObjectPool pool, CharacterType characterType, Transform targetPoint)
    {
        if (characterType == CharacterType.Player)
        {
            GameObject arrowObj = pool.Get(PoolType.FireArrow);
            arrowObj.transform.position = player.ShootPoint.position;
            FireArrow fireArrow = arrowObj.GetComponent<FireArrow>();
            fireArrow.Initialize(targetPoint.position, pool, OwnerType.Player);
        }
        else if (characterType == CharacterType.Enemy)
        {
            GameObject arrowObj = pool.Get(PoolType.FireArrow);
            arrowObj.transform.position = enemy.ShootPoint.position;
            FireArrow fireArrow = arrowObj.GetComponent<FireArrow>();
            fireArrow.Initialize(targetPoint.position, pool, OwnerType.Enemy);
        }
    }
}
