using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    public OwnerType Owner => _owner;

    private int _currentHp;
    private bool _isDead = false;

    private Slider _hpSlider;
    private CombatObjectPool _pool;
    private OwnerType _owner;


    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterShield(this);
    }
    public void Initialize(OwnerType owner, CombatObjectPool pool, Transform uiCanvas)
    {
        _owner = owner;
        _currentHp = maxHp;
        _isDead = false;
        _pool = pool;
        
        
        GameManager.Instance.RegisterShield(this);
        
        GameObject sliderObj = pool.GetUI(PoolType.ShieldHpBar, uiCanvas);
        sliderObj.SetActive(true);
        
        _hpSlider = sliderObj.GetComponent<Slider>();
        _hpSlider.maxValue = maxHp;
        _hpSlider.value = maxHp;
        
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_hpSlider != null && Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            _hpSlider.transform.position = screenPos;
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHp -= damage;
        if (_currentHp <= 0)
        {
            _currentHp = 0;
            Die();
        }

        if (_hpSlider != null)
            _hpSlider.value = _currentHp;
    }

    private void Die()
    {
        _isDead = true;

        if (_hpSlider != null)
        {
            _pool.Return(PoolType.ShieldHpBar, _hpSlider.gameObject);
            _hpSlider = null;
        }
        
        _pool.Return(PoolType.Shield, gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnShieldHitGround();
            }
        }
    }
}
