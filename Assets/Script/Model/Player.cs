using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    public List<SpaceBase> playerBases;
    public int gold;
    public Enums.PlayerType playerType;
    public List<Army> playerArmy;
    private int[] _baseCount;
    private PlayerData _playerData;
    private Data _gameData;
    
    
    public void GetBase()
    {
        foreach (var baseId in _baseCount)
        {
            playerBases.Add(SetBase(baseId));
        }
    }
    public void UpdateData(Data gameData, PlayerData playerData)
    {
        _gameData = gameData;
        _playerData = playerData;
        gold = _playerData.gold;
        _baseCount = _playerData.baseId;
        GetBase();
    }
    public void ClearPlayer()
    {
        for (int i = 0; i <= playerArmy.Count - 1; i++)
        {
            playerArmy[i].DestroyArmy();
        }
        foreach (var playerBase in playerBases)
        {
            playerBase.DestroyBase();
        }
        playerBases.Clear();
    }
    public Player(PlayerData playerData, Data gameData)
    {
        playerArmy = new List<Army>();
        playerBases = new List<SpaceBase>();
        _playerData = playerData;
        playerType = _playerData.playerType;
        _gameData = gameData;
        gold = _playerData.gold;
        _baseCount = _playerData.baseId;
        GetBase();
    }
    public void Update()
    {
        for (int i = 0; i <= playerArmy.Count - 1; i++)
        {
            playerArmy[i].Update();
        }
    }
    public void AddResources()
    {
        foreach (var _base in playerBases)
        {
            _base.GoldAdd();
            _base.UnitAdd();
        }
    }
    private SpaceBase SetBase(int idBase)
    {
        SpaceBase newSpaceBase = new SpaceBase(_gameData.allBase[idBase],this, _gameData);
        return newSpaceBase;
    }
}