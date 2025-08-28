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
            Arrow arrow = arrowObj.GetComponent<Arrow>();
            arrow.Initialize(targetPoint.position, pool, OwnerType.Player);
        }
        else if (characterType == CharacterType.Enemy)
        {
            GameObject arrowObj = pool.Get(PoolType.FireArrow);
            arrowObj.transform.position = enemy.ShootPoint.position;
            Arrow arrow = arrowObj.GetComponent<Arrow>();
            Transform target = GameManager.Instance.PlayerTarget;
            arrow.Initialize(target.position, pool, OwnerType.Enemy);
        }
    }
}
