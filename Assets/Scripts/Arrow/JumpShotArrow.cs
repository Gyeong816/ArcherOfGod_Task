using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpShotArrow : MonoBehaviour
{
    [SerializeField] private float speed = 12f;      
    [SerializeField] private int damage = 10;    
    [SerializeField] private float maxLifeTime = 5f;
    [SerializeField] private GameObject flashEffect; 
    [SerializeField] private GameObject arrowBody;
    [SerializeField] private float flashDuration = 1f;

    private Vector3 _target;
    private CombatObjectPool _pool;
    private float _timer;
    private OwnerType _owner;
    private bool _isStopped;
    private Vector3 _direction; 

    public void Initialize(Vector3 targetPos, CombatObjectPool pool, OwnerType ownerType)
    {
        _target = targetPos;
        _pool = pool;
        _timer = 0f;
        _owner = ownerType;
        
        _direction = (targetPos - transform.position).normalized;
        
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        gameObject.SetActive(true);
        arrowBody.SetActive(true);
        _isStopped = false;
    }

    private void Update()
    {
        if(_isStopped) return;
        
        transform.position += _direction * speed * Time.deltaTime;
        
        _timer += Time.deltaTime;
        if (_timer >= maxLifeTime)
        {
            _pool.Return(PoolType.JumpShotArrow, gameObject);
        }
    }

    private IEnumerator OnFlash(float delay)
    {
        _isStopped = true;
        arrowBody.SetActive(false);
        flashEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        flashEffect.SetActive(false);
        _pool.Return(PoolType.JumpShotArrow, gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_isStopped) return;
        
        if (other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(OnFlash(flashDuration));
            return;
        }
        
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
