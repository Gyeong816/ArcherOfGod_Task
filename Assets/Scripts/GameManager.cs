using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemyController enemyController;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
