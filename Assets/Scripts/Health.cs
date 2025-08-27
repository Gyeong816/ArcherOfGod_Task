using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private CharacterType characterType;
    
    private int _currentHp;

    private void Awake()
    {
        _currentHp = maxHp;
        hpSlider.maxValue = maxHp;
    }
    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        _currentHp = Mathf.Max(_currentHp, 0);
        
        hpSlider.value = _currentHp;

        if (_currentHp <= 0)
            Die();
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
