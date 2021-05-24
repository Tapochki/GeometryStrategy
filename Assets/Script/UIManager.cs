using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class UIManager
{
    private Canvas _canvas;
    private Text _goldMineText;
    private Text _barrackText;
    private Text _houseText;
    private Text _playerGold;
    private Text _enemyGold;
    private GameObject _baseMenu;
    private Button _goldMineUpgrade;
    private Button _barrackUpgrade;
    private Button _houseUpgrade;
    private SpaceBase _targetSpaceBase;
    private Text _winText;
    private Button _winButton;
    private Image _imageColor;
    private Image _mainMenuImage;
    private Button _startButton;
    private Button _exitButton;
    private GameManager _gameManager;
    public UIManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        _canvas = GameObject.Find("GameCanvas").GetComponent<Canvas>();
        _mainMenuImage = _canvas.transform.Find("Image_MainMenu").GetComponent<Image>();
        _startButton = _mainMenuImage.transform.Find("Button_Start").GetComponent<Button>();
        _exitButton = _mainMenuImage.transform.Find("Button_Exit").GetComponent<Button>();
        _startButton.onClick.AddListener(StartGame);
        _exitButton.onClick.AddListener(ExitGame);
    }
    private void RestartGame()
    {
        _gameManager.RestartGame();
        _gameManager.isGameStart = true;
        _winButton.gameObject.SetActive(false);
    }
    private void StartGame()
    {
        _gameManager.StartGame();
        _baseMenu = _canvas.transform.Find("PlayerBaseMenu").gameObject;
        _goldMineUpgrade = _baseMenu.transform.Find("Button_GoldMineUpgrade").GetComponent<Button>();
        _goldMineText = _goldMineUpgrade.transform.Find("Text_GoldMineUpgrade").GetComponent<Text>();
        _barrackUpgrade = _baseMenu.transform.Find("Button_BarrackUpgrade").GetComponent<Button>();
        _barrackText = _barrackUpgrade.transform.Find("Text_BarrackUpgrade").GetComponent<Text>();
        _houseUpgrade = _baseMenu.transform.Find("Button_HouseUpgrade").GetComponent<Button>();
        _houseText = _houseUpgrade.transform.Find("Text_HouseUpgrade").GetComponent<Text>();
        _winButton = _canvas.transform.Find("Button_Win").GetComponent<Button>();
        _imageColor = _winButton.GetComponent<Image>();
        _winText = _winButton.transform.Find("Text_Win").GetComponent<Text>();
        _playerGold = _canvas.transform.Find("Text_PlayerGold").GetComponent<Text>();
        _enemyGold = _canvas.transform.Find("Text_EnemyGold").GetComponent<Text>();
        _winButton.gameObject.SetActive(false);
        _winButton.onClick.AddListener(RestartGame);
        _barrackUpgrade.onClick.AddListener(BarrackUpgradeClick);
        _goldMineUpgrade.onClick.AddListener(GoldMineClick);
        _houseUpgrade.onClick.AddListener(HouseUpgradeClick);
        _mainMenuImage.gameObject.SetActive(false);
        BaseMenuDeActive();
        _gameManager.isGameStart = true;
        _gameManager.playerController.playerGold += PlayerGoldUpdate;
        _gameManager.enemyAIController.enemyGold += EnemyGoldUpdate;
        _gameManager.gameWin += Win;
    }
    private void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }
    public void BaseMenuDeActive()
    {
        if (_targetSpaceBase != null)
        {
            _targetSpaceBase = null;
        }
        _baseMenu.gameObject.SetActive(false);
    }
    public void GetBase(SpaceBase selectedSpaceBase)
    {
        _targetSpaceBase = selectedSpaceBase;
        UpdateText(_houseText, _targetSpaceBase.CostBuildUpgrade(Enums.BuildType.House, _targetSpaceBase.houseLevel), "House", _targetSpaceBase.houseLevel);
        UpdateText(_goldMineText, _targetSpaceBase.CostBuildUpgrade(Enums.BuildType.GoldMine,_targetSpaceBase.goldMineLevel), "GoldMine", _targetSpaceBase.goldMineLevel);
        UpdateText(_barrackText, _targetSpaceBase.CostBuildUpgrade(Enums.BuildType.Barrack,_targetSpaceBase.barackLevel), "Barrack", _targetSpaceBase.barackLevel);
        BaseMenuActive();
    }

    public void PlayerGoldUpdate(int playerGold)
    {
        _playerGold.text = $"Gold: {playerGold}";
    }
    public void EnemyGoldUpdate(int enemyGold)
    {
        _enemyGold.text = $"Enemy Gold {enemyGold}";
    }
    private void Win(string text, Color color)
    {
        _imageColor.color = color;
        _gameManager.isGameStart = false;
        _winButton.gameObject.SetActive(true);
        _winText.text = text;
        _gameManager.EndGame();
        BaseMenuDeActive();
        Debug.Log("Win");
    }
    private void BaseMenuActive()
    {
        if (_targetSpaceBase != null)
        {
            Debug.Log(_targetSpaceBase.baseObject.name);
            _baseMenu.gameObject.SetActive(true);
        }
    }
    public void HouseUpgradeClick()
    {
        _targetSpaceBase.HouseUpgrade();
        UpdateText(_houseText, _targetSpaceBase.CostBuildUpgrade(Enums.BuildType.House,_targetSpaceBase.houseLevel),"House", _targetSpaceBase.houseLevel);
        Debug.Log("HouseUpgrade");
    }
    public void GoldMineClick()
    {
        _targetSpaceBase.GoldMineUpgrade();
        UpdateText(_goldMineText, _targetSpaceBase.CostBuildUpgrade(Enums.BuildType.GoldMine,_targetSpaceBase.goldMineLevel), "GoldMine", _targetSpaceBase.goldMineLevel);
        Debug.Log("GoldMine Upgrade");
    }
    public void BarrackUpgradeClick()
    {
        Debug.Log("Barrack Upgrade");
        _targetSpaceBase.BarrackUpgrade();
        UpdateText(_barrackText, _targetSpaceBase.CostBuildUpgrade(Enums.BuildType.Barrack,_targetSpaceBase.barackLevel), "Barrack", _targetSpaceBase.barackLevel);
    }
    private void UpdateText(Text buildText, int costLevel, string text, int buildLevel)
    {
        if (buildLevel == 4)
        {
            buildText.text = $"{text} is max level";
        }
        else
        {
            buildText.text = $"{text} Upgrade " +
                             $"Cost {costLevel} " +
                             $"Level {buildLevel}";
        }
    }
}
