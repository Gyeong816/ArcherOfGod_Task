using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrowSkill : BaseSkill
{
    public override void Activate(PlayerController player, EnemyController enemy, CombatObjectPool pool, CharacterType characterType, Transform targetPoint)
    {
        if (characterType == CharacterType.Player)
        {
            GameObject arrowObj = pool.Get(PoolType.IceArrow);
            arrowObj.transform.position = player.ShootPoint.position;
            IceArrow iceArrow = arrowObj.GetComponent<IceArrow>();
            iceArrow.Initialize(targetPoint.position, pool, OwnerType.Player);
        }
        else if (characterType == CharacterType.Enemy)
        {
            GameObject arrowObj = pool.Get(PoolType.IceArrow);
            arrowObj.transform.position = enemy.ShootPoint.position;
            IceArrow iceArrow = arrowObj.GetComponent<IceArrow>();
            iceArrow.Initialize(targetPoint.position, pool, OwnerType.Enemy);
        }
    }
}
