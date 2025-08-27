using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void DealDamage(GameObject target, int damage)
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth == null) return;
        
        targetHealth.TakeDamage(damage);
        
    }
}
