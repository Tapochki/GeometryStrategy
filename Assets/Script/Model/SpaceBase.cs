using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class SpaceBase
{
    public int unitCount;
    public int goldMineLevel;
    public int houseLevel;
    public int barackLevel;
    public GameObject baseObject;
    private TextMeshPro _unitText;
    private float _add;
    private int _maxUnit;
    public Player baseOwner;
    private SpriteRenderer _baseColor;
    private Data _gameData;
    private int _costBuildUpgrade;
    public SpaceBase(BaseData thisBase,
        Player baseOwner, Data gameData)
    {
        goldMineLevel = thisBase.goldMineLevel;
        houseLevel = thisBase.houseLevel;
        barackLevel = thisBase.barrackLevel;
        var position = thisBase.basePosition;
        baseObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>($"Prefab/SpaceBase"), position, Quaternion.identity);
        unitCount = thisBase.unitCount;
        this.baseOwner = baseOwner;
        SetMaxUnit();
        _unitText = baseObject.transform.Find("UnitCountText").gameObject.GetComponent<TextMeshPro>();
        _baseColor = baseObject.GetComponent<SpriteRenderer>();
        _gameData = gameData;
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (baseOwner.playerType == Enums.PlayerType.Enemy)
        {
            _baseColor.color = Color.blue;
        }
        if (baseOwner.playerType == Enums.PlayerType.MainPlayer)
        {
            _baseColor.color = Color.red;
        }
        if (baseOwner.playerType == Enums.PlayerType.Vacant)
        {
            _baseColor.color = Color.grey;
        }
    }
    public int CostBuildUpgrade(Enums.BuildType buildType, int buildLevel)
    {
        var buildTypeCost = _gameData.allCost.Find(x => x.buildType == buildType);
        _costBuildUpgrade = buildLevel switch
        {
            1 => buildTypeCost.firstToSecondLevelCost,
            2 => buildTypeCost.secondToThirdLevelCost,
            3 => buildTypeCost.thirdToFourthLevelCost,
            _ => _costBuildUpgrade
        };
        return _costBuildUpgrade;
    }
    public void DestroyBase()
    {
        MonoBehaviour.Destroy(baseObject);
    }
    public void SetMaxUnit()
    {
        switch (houseLevel)
        {
            case 1:
                _maxUnit = 100;
                baseObject.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case 2:
                _maxUnit = 250;
                baseObject.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                break;
            case 3:
                _maxUnit = 500;
                baseObject.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                break;
            case 4:
                _maxUnit = 1000;
                baseObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
        }
    }

    public void UnitAdd()
    {
        SetMaxUnit();
        _add = barackLevel switch
        {
            1 => 1f,
            2 => 2f,
            3 => 3f,
            4 => 4f,
            _ => _add
        };
        if (_maxUnit > unitCount)
        {
            unitCount += (int) _add;
        }

        if (_maxUnit < unitCount)
        {
            unitCount -= 1;
        }
        UpdateText();
    }

    public void UpdateText()
    {
        _unitText.text = unitCount.ToString();
       
    }

    public void HouseUpgrade()
    {
        int costLevel = CostBuildUpgrade(Enums.BuildType.House, houseLevel);
        if (costLevel <= baseOwner.gold && houseLevel < 4)
        {
            baseOwner.gold -= costLevel;
            houseLevel++;
        }
    }
    public void BarrackUpgrade()
    {
        int costLevel = CostBuildUpgrade(Enums.BuildType.Barrack, barackLevel);
        if (costLevel <= baseOwner.gold && barackLevel < 4)
        {
            baseOwner.gold -= costLevel;
            barackLevel++;
        }
    }
    public void GoldMineUpgrade()
    {
        int costLevel = CostBuildUpgrade(Enums.BuildType.GoldMine, goldMineLevel);
        Debug.Log(costLevel + "GoldMine");
        if (costLevel <= baseOwner.gold && goldMineLevel < 4)
        {
            baseOwner.gold -= costLevel;
            goldMineLevel++;
        }
    }

    public void GoldAdd()
    {
        _add = goldMineLevel switch
        {
            1 => 1f,
            2 => 2f,
            3 => 4f,
            4 => 8f,
            _ => _add
        };
        baseOwner.gold += (int) _add;
    }
}


