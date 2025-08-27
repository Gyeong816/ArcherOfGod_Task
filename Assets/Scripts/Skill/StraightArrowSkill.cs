using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightArrowSkill : BaseSkill
{
    private CombatObjectPool projectilePool;
    private Transform shootPoint;

    public override void Activate(Transform caster, Transform target)
    {
        GameObject straightArrow = projectilePool.Get(PoolType.StraightArrow);
        straightArrow.transform.position = shootPoint.position;

        Rigidbody2D rb = straightArrow.GetComponent<Rigidbody2D>();
        rb.velocity = (caster.localScale.x > 0 ? Vector2.right : Vector2.left) * 10f;
    }
}
