using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightArrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f; 
    [SerializeField] private int damage = 20;    
    [SerializeField] private float lifeTime = 10f; 
    [SerializeField] private GameObject flashEffect; 
    [SerializeField] private GameObject arrowBody;
    [SerializeField] private float flashDuration = 1f;
    
    private Vector3 _direction;
    private CombatObjectPool _pool;
    private float _timer;
    private OwnerType _owner;
    private bool _isStopped;
    public void Initialize(CombatObjectPool pool, Vector3 direction, OwnerType ownerType)
    {
        _pool = pool;
        _direction = direction;
        _owner = ownerType;
        _timer = 0f;
        gameObject.SetActive(true);
        arrowBody.SetActive(true);
        _isStopped = false;
    }

    private void Update()
    {
        if(_isStopped) return;
        
        transform.position += _direction * speed * Time.deltaTime;
        
        _timer += Time.deltaTime;
        if (_timer >= lifeTime)
        {
            _pool.Return(PoolType.StraightArrow, gameObject);
        }
    }
    
    private IEnumerator OnFlash(float delay)
    {
        _isStopped = true;
        arrowBody.SetActive(false);
        flashEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        flashEffect.SetActive(false);
        _pool.Return(PoolType.StraightArrow, gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_isStopped) return;
        
        if (other.TryGetComponent(out Shield shield))
        {
            if (shield.Owner == _owner)
                return;
            
            shield.TakeDamage(damage);
            StartCoroutine(OnFlash(flashDuration));
            return;
        }
        
        if (other.TryGetComponent(out Health health))
        {
            CombatSystem.Instance.DealDamage(other.gameObject, damage);
            StartCoroutine(OnFlash(flashDuration));
        }
    }
}
