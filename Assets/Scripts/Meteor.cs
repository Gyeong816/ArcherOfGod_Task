using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private GameObject meteorBody;
    [SerializeField] private GameObject flashEffect;
    [SerializeField] private float flashDuration = 0.5f;
    [SerializeField] private int damage = 20; 
    private CombatObjectPool _pool;
    private Rigidbody2D _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    public void Initialize(CombatObjectPool pool)
    {
        flashEffect.SetActive(false);
        meteorBody.SetActive(true);
        _pool =  pool;
        _rb.isKinematic = false;
        _rb.velocity = Vector2.zero;
    }
    
    private IEnumerator OnFlash(float delay)
    {
        meteorBody.SetActive(false);
        flashEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        flashEffect.SetActive(false);
        _pool.Return(PoolType.Meteor, gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true;

            flashEffect.SetActive(true);
            GameManager.Instance.ShakeCamera(0.2f, 0.2f);
            StartCoroutine(OnFlash(flashDuration));
            return;
        }
        if (other.gameObject.CompareTag("ObjectDestroyZone"))
        {
            _pool.Return(PoolType.Meteor, gameObject);
            return;
        }
        
        if (other.TryGetComponent(out Health health))
        {
            CombatSystem.Instance.DealDamage(other.gameObject, damage);
        }
    }
}
