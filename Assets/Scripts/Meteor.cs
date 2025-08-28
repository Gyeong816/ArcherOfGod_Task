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
    private bool _isStopped;
    
    public void Initialize(CombatObjectPool pool)
    {
        _pool =  pool;
        _isStopped = false;
    }
    
    private IEnumerator OnFlash(float delay)
    {
        _isStopped = true;
        meteorBody.SetActive(false);
        flashEffect.SetActive(true);
        yield return new WaitForSeconds(delay);
        flashEffect.SetActive(false);
        _pool.Return(PoolType.FireArrow, gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            flashEffect.SetActive(true);
            GameManager.Instance.ShakeCamera(0.2f, 0.2f);
            StartCoroutine(OnFlash(flashDuration));
            return;
        }
        
        
        if (other.TryGetComponent(out Health health))
        {
            CombatSystem.Instance.DealDamage(other.gameObject, damage);
        }
    }
}
