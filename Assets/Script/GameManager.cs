using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager
{
    public EnemyAIController enemyAIController;
    public PlayerController playerController;
    public VacantController vacantController;
    private float _gameManagerAddTimer;
    public Data gameData;
    private string _dataName;
    public bool isGameStart;
    public event Action<string, Color> gameWin;
    public GameManager()
    {
        _gameManagerAddTimer = 1f;
        isGameStart = false;
    }
    public void GetData()
    {
        var index = Random.Range(1, 6);
        Debug.Log(index);
        gameData = new Data(index);
    }
    public void Update()
    {
        if (isGameStart)
        {
            playerController.Update();
            enemyAIController.Update();
            vacantController.Update();
            _gameManagerAddTimer -= Time.deltaTime;
        
            if (_gameManagerAddTimer <= 0)
            {
                ControllerBaseAddResources();
                _gameManagerAddTimer = 1f;
            }
        }
        if (isGameStart)
        {
            if (playerController.mainPlayer.playerBases.Count == 0)
            {
                gameWin.Invoke("Blue Wins", Color.blue);
            }
            else if (enemyAIController.enemy.playerBases.Count == 0)
            {
                gameWin.Invoke("Red Wins", Color.red);
            }
        }
    }
    private void ControllerBaseAddResources()
    {
        playerController.PlayerAddResources();
        enemyAIController.EnemyAddResources();
        vacantController.VacantAddResources();
    }
    public void EndGame()
    {
        playerController.EndPlayer();
        enemyAIController.EndEnemy();
        vacantController.EndVacant();
    }
    public void StartGame()
    {
        GetData();
        playerController = new PlayerController(gameData);
        vacantController = new VacantController(gameData);
        enemyAIController = new EnemyAIController(gameData);
    }
    public void RestartGame()
    {
        GetData();
        playerController.RestartPlayer(gameData);
        enemyAIController.RestartEnemy(gameData);
        vacantController.RestartVacant(gameData);
    }
}

