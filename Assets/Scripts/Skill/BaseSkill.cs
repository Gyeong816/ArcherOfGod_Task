using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    public string skillName;
    public float cooldown = 3f;

    private float _lastUseTime = -999f;

    public bool CanUse()
    {
        return Time.time >= _lastUseTime + cooldown;
    }

    public float GetRemainingCooldown()
    {
        float remaining = (_lastUseTime + cooldown) - Time.time;
        return Mathf.Max(0, remaining);
    }

    public void TryActivate(Transform caster, Transform target)
    {
        if (CanUse())
        {
            Activate(caster, target);
            _lastUseTime = Time.time;
        }
    }

    public abstract void Activate(Transform caster, Transform target);
}
