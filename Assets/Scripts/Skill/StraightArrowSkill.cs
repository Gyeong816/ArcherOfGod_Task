using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightArrowSkill : BaseSkill
{
    private CombatObjectPool _combatObjectPool;
    private Transform _targetPoint;

    public override void Activate(Transform caster, Transform target, CombatObjectPool combatObjectPool)
    {
        GameObject straightArrow = combatObjectPool.Get(PoolType.StraightArrow);
        
        straightArrow.transform.position = caster.position;
        straightArrow.transform.rotation = Quaternion.identity;
        
        StraightArrow arrow = straightArrow.GetComponent<StraightArrow>();
        arrow.Initialize(target.position, combatObjectPool);
    }
}
