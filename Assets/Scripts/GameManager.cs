using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Health enemyHealth;
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform enemyTransform;
    [SerializeField] private UiManager uiManager;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private CombatObjectPool combatObjectPool;
    [SerializeField] private GameObject endgameIcon;
    
    [Header("라운드 시간 설정")]
    [SerializeField] private float roundTime = 90f;   
    private float _remainingTime;
    private bool _isRunning = false; 
    private Camera mainCamera;
    public static GameManager Instance { get; private set; }

    private List<Shield> _playerShields = new List<Shield>();
    private List<Shield> _enemyShields = new List<Shield>();

    public Transform PlayerTarget { get; private set; }
    public Transform EnemyTarget { get; private set; }

    private bool _isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        mainCamera = Camera.main;
        PlayerTarget = enemyTransform;
        EnemyTarget = playerTransform;
        
    }
    
    private void Update()
    {
        if (_isRunning && _remainingTime > 0f)
        {
            _remainingTime -= Time.deltaTime;
            
            uiManager.UpdateTimerUI(_remainingTime);

            if (_remainingTime <= 0f)
            {
                _isRunning = false;
                EndRound();
            }
        }
    }
    
    public void StartGame()
    {
        _remainingTime = roundTime;
        _isRunning = true;
        skillManager.StartGame();
        playerController.StartGame();
        enemyController.StartGame();
    }

    private void EndRound()
    {
        endgameIcon.SetActive(true);
        StartCoroutine(SpawnMeteorRoutine());
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
    
    private IEnumerator SpawnMeteorRoutine()
    {
        while (!_isGameOver) 
        {
            SpawnMeteor();
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void SpawnMeteor()
    {
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];
        
        GameObject meteorObj = combatObjectPool.Get(PoolType.Meteor);
        
        Meteor meteor = meteorObj.GetComponent<Meteor>();
        meteor.Initialize(combatObjectPool);
        
        float xOffset = UnityEngine.Random.Range(-1f, 1f);  
        Vector3 spawnPos = spawnPoint.position + new Vector3(xOffset, 0f, 0f);
        meteorObj.transform.position = spawnPos;
        meteorObj.SetActive(true);
    }
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(CameraShake(duration, magnitude));
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = mainCamera.transform.localPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }
    public void OnPlayerDead()
    {
        if (_isGameOver) return; 
        
        _isGameOver = true;
        enemyHealth.GameOver();
        playerHealth.GameOver();
        enemyController.OnVictory();
        StartCoroutine(ShowResultPanel(false));
    }

    public void OnEnemyDead()
    {
        if (_isGameOver) return; 
        
        _isGameOver = true;
        enemyHealth.GameOver();
        playerHealth.GameOver();
        playerController.OnVictory();
        StartCoroutine(ShowResultPanel(true));
    }
    private IEnumerator ShowResultPanel(bool isVictory)
    {
        yield return new WaitForSeconds(2f); 

        if (isVictory)
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(true);
        }
        else
        {
            if (defeatPanel != null)
                defeatPanel.SetActive(true);
        }
    }
   
}
