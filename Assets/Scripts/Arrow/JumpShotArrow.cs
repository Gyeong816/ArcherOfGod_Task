using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpShotArrow : MonoBehaviour
{
    [SerializeField] private float speed = 12f;      
    [SerializeField] private int damage = 10;    
    [SerializeField] private float maxLifeTime = 5f;  

    private Vector3 _target;
    private CombatObjectPool _pool;
    private float _timer;
    
    private OwnerType _owner;

    public void Initialize(Vector3 targetPos, CombatObjectPool pool, OwnerType ownerType)
    {
        _target = targetPos;
        _pool = pool;
        _timer = 0f;
        _owner = ownerType;
        
        Vector3 dir = (_target - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        gameObject.SetActive(true);
    }

    private void Update()
    {
        
        Vector3 dir = (_target - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        
 
        _timer += Time.deltaTime;
        if (_timer >= maxLifeTime)
        {
            _pool.Return(PoolType.JumpShotArrow, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Shield shield))
        {
            if (shield.Owner == _owner)
                return;
            
            shield.TakeDamage(damage);
            _pool.Return(PoolType.JumpShotArrow, gameObject);
            return;
        }
        
        if (other.TryGetComponent(out Health health))
        {
            CombatSystem.Instance.DealDamage(other.gameObject, damage);
            _pool.Return(PoolType.JumpShotArrow, gameObject);
        }
    }
}
