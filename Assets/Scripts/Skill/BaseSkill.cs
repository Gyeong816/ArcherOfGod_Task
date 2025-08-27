using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    public SkillType skillType;
    public float cooldown = 3f;

    private float cooldownTimer = 0f; 


    public void TickCooldown(float deltaTime)
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= deltaTime;
    }
    
    public bool CanUse()
    {
        return cooldownTimer <= 0f;
    }
    
    public float GetRemainingCooldown()
    {
        return Mathf.Max(0f, cooldownTimer);
    }
    
    public void SetTimer()
    {
         cooldownTimer = cooldown; 
    }

    public abstract void Activate(PlayerController player, EnemyController enemy, CombatObjectPool pool, CharacterType characterType);
}
