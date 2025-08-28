using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour
{
    [SerializeField] private float duration = 1f;   
    [SerializeField] private float arcScale = 0.3f;
    [SerializeField] private float arrowSpeed = 5f; 
    [SerializeField] private int damage = 20; 
    [SerializeField] private float groundFireDuration = 5f;
    [SerializeField] private GameObject groundFireEffect;

    private Vector3 _start;
    private Vector3 _target;
    private Vector3 _controlPoint;
    private float _time;
    private CombatObjectPool _pool;
    private OwnerType _owner;

    public void Initialize(Vector3 targetPos, CombatObjectPool pool, OwnerType owner)
    {
        _start = transform.position;
        _target = targetPos;
        _pool = pool;
        _owner = owner;
        float distance = Vector3.Distance(_start, _target);
        float arcHeight = distance * arcScale;

        _controlPoint = (_start + _target) / 2f + Vector3.up * arcHeight;
        _time = 0f;
        
        duration = distance / arrowSpeed;
        
        Vector3 dir = (_target - _start).normalized;
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_time < 1f)
        {
            _time += Time.deltaTime / duration;
            Vector3 newPos = CalculateBezierPoint(_time);
            
            Vector3 direction = (newPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            transform.position = newPos;
        }
        else
        {
            _pool.Return(PoolType.FireArrow, gameObject);
        }
    }

    private Vector3 CalculateBezierPoint(float t)
    {
        Vector3 q0 = Vector3.Lerp(_start, _controlPoint, t);
        Vector3 q1 = Vector3.Lerp(_controlPoint, _target, t);
        return Vector3.Lerp(q0, q1, t);
    }
    
    private IEnumerator ReturnAfterFire(float delay)
    {
        groundFireEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        groundFireEffect.SetActive(false);
        _pool.Return(PoolType.FireArrow, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(ReturnAfterFire(groundFireDuration));
            return;
        }
        if (other.TryGetComponent(out Shield shield))
        {
            if (shield.Owner == _owner)
                return;
            
            shield.TakeDamage(damage);
            return;
        }
        
        if (other.TryGetComponent(out Health health))
        {
            CombatSystem.Instance.DealDamage(other.gameObject, damage);
            _pool.Return(PoolType.FireArrow, gameObject);
            return;
        }
    }
    
}
