using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VacantController
{
    public Player vacant;
    private Data _gameData;
    public VacantController(Data gameData)
    {
        _gameData = gameData;
        vacant = new Player(_gameData.allPlayer.Find(x => x.playerType == Enums.PlayerType.Vacant), _gameData);
    }
    public void Update()
    {
        vacant.Update();
    }

    public void LostSpaceBase(SpaceBase spaceBase)
    {
        vacant.playerBases.Remove(spaceBase);
    }
    public void RestartVacant(Data gameData)
    {
        _gameData = gameData;
        vacant.UpdateData(_gameData, _gameData.allPlayer[2]);
    }

    public void EndVacant()
    {
        vacant.ClearPlayer();
    }
    public void VacantAddResources()
    {
        vacant.AddResources();
    }
    public SpaceBase SelectBaseForAttack(GameObject baseForAttackObject)
    {
        for(int i = 0; i<= vacant.playerBases.Count - 1; i++)
        {
            if (baseForAttackObject == vacant.playerBases[i].baseObject)
            {
                return vacant.playerBases[i];
            }
        }
        return null;
    }
    public SpaceBase SelectBaseForAttack()
    {
        var choiseBase = Random.Range(0, vacant.playerBases.Count - 1);
        if (vacant.playerBases.Count == 0)
        {
            return MainApp.Instance.gameManager.playerController.SelectBaseForAttack();
        }
        return vacant.playerBases[choiseBase];
    }
}
