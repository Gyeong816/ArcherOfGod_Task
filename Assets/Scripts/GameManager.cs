using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemyController enemyController;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform enemyTransform;
    public static GameManager Instance { get; private set; }

    private List<Shield> _playerShields = new List<Shield>();
    private List<Shield> _enemyShields = new List<Shield>();

    public Transform PlayerTarget { get; private set; }
    public Transform EnemyTarget { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        PlayerTarget = enemyTransform;
        EnemyTarget = playerTransform;
    }
    
    public void RegisterShield(Shield shield)
    {
        if (shield.Owner == OwnerType.Player)
        {
            _playerShields.Add(shield);
            EnemyTarget = shield.transform;
        }
        else
        {
            _enemyShields.Add(shield);
            PlayerTarget = shield.transform;
        }
    }
    
    public void UnregisterShield(Shield shield)
    {
        if (shield.Owner == OwnerType.Player)
        {
            _playerShields.Remove(shield);

            if (_playerShields.Count > 0)
                EnemyTarget = _playerShields[0].transform;
            else
                EnemyTarget = playerTransform;
        }
        else
        {
            _enemyShields.Remove(shield);

            if (_enemyShields.Count > 0)
                PlayerTarget = _enemyShields[0].transform;
            else
                PlayerTarget = enemyTransform;
        }
    }
    
    public void OnPlayerDead()
    {
        Debug.Log("패배!");
        enemyController.OnVictory();
    }

    public void OnEnemyDead()
    {
        Debug.Log("승리!");
        playerController.OnVictory();
    }
    
   
}
