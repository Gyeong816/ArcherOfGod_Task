using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float duration = 1f;   
    [SerializeField] private float arcScale = 0.3f;
    [SerializeField] private float arrowSpeed = 5f; 
    [SerializeField] private int damage = 20; 
    [SerializeField] private float flashDuration = 1f; 
    [SerializeField] private float fadeDuration = 1f; 
    [SerializeField] private GameObject flashEffect; 
    [SerializeField] private SpriteRenderer arrowSprite; 
    
    private Vector3 _start;
    private Vector3 _target;
    private Vector3 _controlPoint;
    private float _time;
    private CombatObjectPool _pool;
    private OwnerType _owner;
    private bool _hasReachedTarget;
    private Vector3 _lastDirection;
    private bool _isStopped;
    public void Initialize(Vector3 targetPos, CombatObjectPool pool, OwnerType owner)
    {
        _isStopped = false;
        _start = transform.position;
        _target = targetPos;
        _pool = pool;
        _owner = owner;
        
        arrowSprite.color = new Color(1f, 1f, 1f, 1f);
        arrowSprite.enabled = true;
        
        float distance = Vector3.Distance(_start, _target);
        float arcHeight = distance * arcScale;
        
        _hasReachedTarget = false;
        _controlPoint = (_start + _target) / 2f + Vector3.up * arcHeight;
        _time = 0f;
        
        duration = distance / arrowSpeed;
        
        Vector3 dir = (_target - _start).normalized;
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        arrowSprite.enabled = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_isStopped) return; 
        
        if (!_hasReachedTarget)
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
                _lastDirection = direction; 
            }
            else
            {
                _hasReachedTarget = true;
            }
        }
        else
        {
            transform.position += _lastDirection * arrowSpeed * Time.deltaTime;
        }
    }

    private Vector3 CalculateBezierPoint(float t)
    {
        Vector3 q0 = Vector3.Lerp(_start, _controlPoint, t);
        Vector3 q1 = Vector3.Lerp(_controlPoint, _target, t);
        return Vector3.Lerp(q0, q1, t);
    }

    private IEnumerator OnFlash(float delay)
    {
        flashEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        flashEffect.SetActive(false);
    }
    private IEnumerator FadeAndReturn(float delay)
    {
        _isStopped = true;

        float elapsed = 0f;
        Color color = arrowSprite.color;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / delay); 
            arrowSprite.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        _pool.Return(PoolType.Arrow, gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(FadeAndReturn(fadeDuration));
            return;
        }
        
        if (other.TryGetComponent(out Shield shield))
        {
            if (shield.Owner == _owner)
                return;
            
            shield.TakeDamage(damage);
            
            StartCoroutine(OnFlash(flashDuration));
            StartCoroutine(FadeAndReturn(fadeDuration));
            return;
        }
        
        if (other.TryGetComponent(out Health health))
        {
            CombatSystem.Instance.DealDamage(other.gameObject, damage);
            StartCoroutine(OnFlash(flashDuration));
        }
    }
  
}
