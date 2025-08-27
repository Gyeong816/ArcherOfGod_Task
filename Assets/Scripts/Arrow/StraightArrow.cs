using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightArrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f; 
    [SerializeField] private int damage = 20;    
    [SerializeField] private float lifeTime = 10f; 

    private Vector3 _direction;
    private CombatObjectPool _pool;
    private float _timer;
    
    public void Initialize(CombatObjectPool pool, Vector3 direction)
    {
        _pool = pool;
        _direction = direction;
        
        _timer = 0f;
    }

    private void Update()
    {
        transform.position += _direction * speed * Time.deltaTime;
        
        _timer += Time.deltaTime;
        if (_timer >= lifeTime)
        {
            _pool.Return(PoolType.StraightArrow, gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Health health))
        {
            CombatSystem.Instance.DealDamage(other.gameObject, damage);
            _pool.Return(PoolType.StraightArrow, gameObject);
        }
    }
}
