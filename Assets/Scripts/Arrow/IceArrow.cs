using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : MonoBehaviour
{
    [SerializeField] private float duration = 1f;   
    [SerializeField] private float arcScale = 0.3f;
    [SerializeField] private float arrowSpeed = 5f; 
    [SerializeField] private int damage = 20; 
    [SerializeField] private float groundIceDuration = 5f;
    [SerializeField] private GameObject groundIce;
    [SerializeField] private GameObject arrowBody;
    [SerializeField] private GameObject flashEffect;
    [SerializeField] private float flashDuration = 0.5f; 
    

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
        _start = transform.position;
        _target = targetPos;
        _pool = pool;
        _owner = owner;
        _isStopped = false;
        float distance = Vector3.Distance(_start, _target);
        float arcHeight = distance * arcScale;

        arrowBody.SetActive(true);
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
        _isStopped = true;
        arrowBody.SetActive(false);
        flashEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        groundIce.SetActive(false);
        flashEffect.SetActive(false);
        _pool.Return(PoolType.IceArrow, gameObject);
    }

    private IEnumerator GroundIceAndReturn(float iceDuration)
    {
        _isStopped = true;
        arrowBody.SetActive(false);
        groundIce.SetActive(true);

        SpriteRenderer sr = groundIce.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        
        yield return new WaitForSeconds(iceDuration);
        
        float fadeTime = 1f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        sr.color = originalColor;
        
        flashEffect.SetActive(false);
        groundIce.SetActive(false);

        _pool.Return(PoolType.IceArrow, gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            flashEffect.SetActive(true);
            GameManager.Instance.ShakeCamera(0.1f, 0.1f);
            StartCoroutine(GroundIceAndReturn(groundIceDuration));
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
        }
    }
}
