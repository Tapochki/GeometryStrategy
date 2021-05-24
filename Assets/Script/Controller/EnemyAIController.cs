using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAIController
{
   public Player enemy;
   public List<SpaceBase> anotherBase;
   private SpaceBase _spaceBaseForAttack;
   private float _enemyAiTimer;
   private Data _gameData;
   public event Action<int> enemyGold;
   private Army _baseArmy;
    public EnemyAIController(Data gameData)
    {
        _gameData = gameData;
        anotherBase = new List<SpaceBase>();
        enemy = new Player(_gameData.allPlayer.Find(x => x.playerType == Enums.PlayerType.Enemy), _gameData);
        _enemyAiTimer = 2f;
        
    }
    public void ArmyCapture(Army invaderArmy, SpaceBase attackedSpaceBase)
    {
        Debug.Log("Enemy Start Capture");
        if (attackedSpaceBase.baseOwner.playerType == invaderArmy.armyOwner.playerType)
        {
            Debug.Log("Its your Base");
            attackedSpaceBase.unitCount += invaderArmy.armyCount;
        }
        if (attackedSpaceBase.baseOwner.playerType != invaderArmy.armyOwner.playerType)
        {
            if (attackedSpaceBase.unitCount >= invaderArmy.armyCount)
            {
                attackedSpaceBase.unitCount -= invaderArmy.armyCount;
                Debug.Log("You have low armyUnit");
            }
            else if (attackedSpaceBase.unitCount < invaderArmy.armyCount)
            {
                invaderArmy.armyCount -= attackedSpaceBase.unitCount;
                attackedSpaceBase.unitCount = invaderArmy.armyCount;
                if (attackedSpaceBase.baseOwner.playerType == Enums.PlayerType.Vacant)
                {
                    MainApp.Instance.gameManager.vacantController.LostSpaceBase(attackedSpaceBase);
                    CaptureBase(attackedSpaceBase, invaderArmy.armyOwner);
                    Debug.Log("EnemyCapture Vacant");
                }
                if (attackedSpaceBase.baseOwner.playerType == Enums.PlayerType.MainPlayer)
                {
                    MainApp.Instance.gameManager.playerController.LostSpaceBase(attackedSpaceBase);
                    CaptureBase(attackedSpaceBase, invaderArmy.armyOwner);
                    Debug.Log("EnemyCapture Player");
                }
            }
        }
        attackedSpaceBase.UpdateText();
    }
    public void EndEnemy()
    {
        enemy.ClearPlayer();
    }
    public void LostSpaceBase(SpaceBase spaceBase)
    {
        enemy.playerBases.Remove(spaceBase);
    }
    public void CaptureBase(SpaceBase spaceBase, Player invader)
    {
        enemy.playerBases.Add(spaceBase);
        spaceBase.baseOwner = invader;
        spaceBase.UpdateColor();
    }
    public void RestartEnemy(Data gameData)
    {
        _gameData = gameData;
        enemy.UpdateData(_gameData, _gameData.allPlayer[1]);
    }
    public void SpawnArmy(SpaceBase spaceBaseForAttack, SpaceBase homeBase)
    {
        if (homeBase.unitCount > 1)
        {
            _baseArmy = new Army(homeBase.unitCount, spaceBaseForAttack, homeBase); 
            _baseArmy.armyEndMove += ArmyCapture;
            homeBase.unitCount = 0;
            enemy.playerArmy.Add(_baseArmy);
        }
        else
        {
            Debug.Log("Too far or few unit");
        }
    }
    public void EnemyBaseControl()
    {
        UpgradeBaseBuild();
        AttackBase();
    }
    private int ChoiseBase(int count)
    {
        var choiseBase = Random.Range(0, count);
        return choiseBase;
    }

    public SpaceBase SelectBaseForAttack(GameObject baseForAttackObject)
    {
        for(int i = 0; i<= enemy.playerBases.Count - 1; i++)
        {
            if (baseForAttackObject == enemy.playerBases[i].baseObject)
            {
                return enemy.playerBases[i];
            }
        }
        return null;
    }

    public void Update()
    {
        _enemyAiTimer -= Time.deltaTime;
        enemy.Update();
        if (_enemyAiTimer <= 0)
        {
            EnemyBaseControl();
            _enemyAiTimer = 2f;
        }
    }
    public void EnemyAddResources()
    {
        enemy.AddResources();
        enemyGold.Invoke(enemy.gold);
    }
    private void UpgradeBaseBuild()
    {
        var id = ChoiseBase(enemy.playerBases.Count-1);
        var chosenBase = enemy.playerBases[id];
        var chosenBuild = Random.Range(1, 3);
        switch (chosenBuild)
            {
                case 1:
                    switch (chosenBase.barackLevel)
                    {
                        case 1:
                            if (enemy.gold > 100)
                            {
                                chosenBase.BarrackUpgrade();
                            }
                            break;
                        case 2:
                            if (enemy.gold > 200)
                            {
                                if (chosenBase.goldMineLevel == 1)
                                {
                                    chosenBase.GoldMineUpgrade();
                                }
                                else if (chosenBase.houseLevel == 1)
                                {
                                    chosenBase.HouseUpgrade();
                                    chosenBase.SetMaxUnit();
                                }
                                else
                                {
                                    chosenBase.BarrackUpgrade();
                                }
                            }
                            break;
                        case 3:
                            if (enemy.gold > 400)
                            {
                                if (chosenBase.goldMineLevel == 2)
                                {
                                    chosenBase.GoldMineUpgrade();
                                }
                                else if (chosenBase.houseLevel == 2)
                                {
                                    chosenBase.HouseUpgrade();
                                    chosenBase.SetMaxUnit();
                                }
                                else
                                {
                                    chosenBase.BarrackUpgrade();
                                }

                            }
                            break;
                        case 4:
                            break;
                    }

                    break;
                case 2:
                    switch (chosenBase.houseLevel)
                    {
                        case 1:
                            if (enemy.gold > 200)
                            {
                                if (chosenBase.goldMineLevel == 1)
                                {
                                    chosenBase.GoldMineUpgrade();
                                }
                                else
                                {
                                    chosenBase.HouseUpgrade();
                                    chosenBase.SetMaxUnit();
                                }

                            }

                            break;
                        case 2:
                            if (enemy.gold > 400)
                            {
                                chosenBase.HouseUpgrade();
                                chosenBase.SetMaxUnit();
                            }
                            else
                            {
                               //Debug.Log("No Money");
                            }

                            break;
                        case 3:
                            if (enemy.gold > 800)
                            {
                                if (chosenBase.goldMineLevel == 2)
                                {
                                    chosenBase.GoldMineUpgrade();
                                    // Debug.Log("GoldMineUprade lvl3");
                                }
                                else
                                {
                                    //Debug.Log("House Upgrade to lvl4");
                                    chosenBase.HouseUpgrade();
                                    chosenBase.SetMaxUnit();
                                }

                            }
                            else
                            {
                                //Debug.Log("No Money");
                            }

                            break;
                        case 4:
                            //Debug.Log("House MaxLvl");
                            break;
                    }

                    break;
                case 3:
                    switch (chosenBase.goldMineLevel)
                    {
                        case 1:
                            if (enemy.gold > 150)
                            {
                               // Debug.Log("GoldMine Upgrade to lvl2");
                               chosenBase.GoldMineUpgrade();
                            }
                            else
                            {
                               // Debug.Log("No Money");
                            }

                            break;
                        case 2:
                            if (enemy.gold > 300)
                            {
                              //  Debug.Log("GoldMine Upgrade to lvl3")
                              chosenBase.GoldMineUpgrade();
                            }
                            else
                            {
                                //Debug.Log("No Money");
                            }

                            break;
                        case 3:
                            if (enemy.gold > 600)
                            {
                              //  Debug.Log("GoldMine Upgrade to lvl4");
                              chosenBase.GoldMineUpgrade();
                            }
                            else
                            {
                                //Debug.Log("No Money");
                            }
                            break;
                        case 4:
                           // Debug.Log("GoldMine MaxLvl");
                            break;
                    }
                    break;
            }
    }

    private void AttackBase()
    {
        Debug.Log("Enemy AttackStart");
            var id = ChoiseBase(enemy.playerBases.Count-1);
            var choisenBase = enemy.playerBases[id];
            var rand = Random.Range(0, 2);
            if (rand == 0)
            {
                _spaceBaseForAttack = MainApp.Instance.gameManager.playerController.SelectBaseForAttack();
            }
            if (rand == 1)
            {
                _spaceBaseForAttack = MainApp.Instance.gameManager.vacantController.SelectBaseForAttack();
            }
            var typeAttack = Random.Range(0, 10);
                try
                {
                    switch (typeAttack)
                    {
                        case 0:
                           // Debug.Log("Small Attack");
                            SpawnArmy(_spaceBaseForAttack, choisenBase);
                            break;
                        case 1:
                            //Debug.Log("3 Army");
                            SpawnArmy(_spaceBaseForAttack, choisenBase);
                            SpawnArmy(_spaceBaseForAttack, enemy.playerBases[0]);
                            SpawnArmy(_spaceBaseForAttack, enemy.playerBases[enemy.playerBases.Count-1]);
                            break;
                        case 2:
                            //Debug.Log("Big Attack");
                            for (int i = 0; i <= enemy.playerBases.Count - 1; i++)
                            {
                                if (enemy.playerBases[i].unitCount > 10 &&
                                    enemy.playerBases[i].unitCount < 50)
                                {
                                    SpawnArmy(_spaceBaseForAttack, enemy.playerBases[i]);
                                }
                            }

                            break;
                        case 3:
                            int sumUnit = 0;
                            for (int i = 0; i <= enemy.playerBases.Count - 1; i++)
                            {
                                sumUnit += enemy.playerBases[i].unitCount;
                                if (sumUnit >= _spaceBaseForAttack.unitCount + 30)
                                {
                                    for (int j = 0; j <= i; j++)
                                    {
                                        SpawnArmy(_spaceBaseForAttack, enemy.playerBases[j]);
                                    } // Debug.Log("Legendary Attack");
                                    break;
                                }
                            }

                            break;
                        default:
                            if (choisenBase.unitCount > _spaceBaseForAttack.unitCount + 10)
                            {
                                //Debug.Log("Default Attack");
                                SpawnArmy(_spaceBaseForAttack, choisenBase);
                            }
                            break;
                    }
                }
                catch
                {
                    
                }
    }
}
