using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText; 
    [SerializeField] private CharacterType characterType;
    
    private float _currentHp;
    private bool _isGameOver = false;
    private float _damageMultiplier = 1f; 
    private void Awake()
    {
        _currentHp = maxHp;
        hpSlider.maxValue = maxHp;
    }
    public void TakeDamage(int damage)
    {
        if(_isGameOver) return;
        
        _currentHp -= damage * _damageMultiplier;
        _currentHp = Mathf.Max(_currentHp, 0);
        
        hpSlider.value = _currentHp;
        UpdateHpUI();
        if (_currentHp <= 0)
        {
            _currentHp = 0;

            Die();
        }
    }

    public void SetDamageMultiplier(float damageMultiplier)
    {
        _damageMultiplier = damageMultiplier;
    }
    private void UpdateHpUI()
    {
        hpText.text = $"{_currentHp}";
    }
    public void GameOver()
    {
        _isGameOver = true;
    }

    private void Die()
    {
        if (characterType == CharacterType.Player)
        {
            GameManager.Instance.OnPlayerDead();
            
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
                playerController.Die();
            
        }
        else if (characterType == CharacterType.Enemy)
        {
            GameManager.Instance.OnEnemyDead();

            EnemyController enemyController = GetComponent<EnemyController>();
             if(enemyController != null)
                 enemyController.Die();
        }
   
    }
}
