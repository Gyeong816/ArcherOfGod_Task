using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill : BaseSkill
{
    [SerializeField] private float spawnHeight = 10f;

    public override void Activate(PlayerController player, EnemyController enemy, CombatObjectPool pool, CharacterType characterType, Transform targetPoint)
    {
        if (characterType == CharacterType.Player)
        {
            GameObject shieldObj = pool.Get(PoolType.Shield); 
            shieldObj.transform.position = player.ShootPoint.position + Vector3.up * spawnHeight;

            Canvas canvas = pool.GetCanvas();
            
            Shield shield = shieldObj.GetComponent<Shield>();
            shield.Initialize(OwnerType.Player, pool, canvas.transform);
        }
        else if (characterType == CharacterType.Enemy)
        {
            GameObject shieldObj = pool.Get(PoolType.Shield);
            shieldObj.transform.position = enemy.ShootPoint.position + Vector3.up * spawnHeight;
            
            Canvas canvas = pool.GetCanvas();

            Shield shield = shieldObj.GetComponent<Shield>();
            shield.Initialize(OwnerType.Enemy, pool, canvas.transform);
        }
    }
}
