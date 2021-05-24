using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public class BaseData
{
   public int id;
   public int unitCount;
   public int goldMineLevel;
   public int houseLevel;
   public int barrackLevel;
   public Vector3 basePosition;
}

public class PlayerData
{
   public Enums.PlayerType playerType;
   public int gold;
   public int[] baseId;
}

public class CostData
{
   public Enums.BuildType buildType;
   public int firstToSecondLevelCost;
   public int secondToThirdLevelCost;
   public int thirdToFourthLevelCost;
}

public class Data
{
   public List<BaseData> allBase;
   public List<PlayerData> allPlayer;
   public List<CostData> allCost;
   public Data(int index)
   {
      var json = Resources.Load<TextAsset>($"Data/PlayerDataLevel_{index.ToString()}").text;
      var jsonCost = Resources.Load<TextAsset>($"Data/CostData").text;
      GameDataSerializeHelper helper = JsonConvert.DeserializeObject<GameDataSerializeHelper>(json);
      allBase = helper.item;
      allPlayer = helper.players;
      CostDataSerializeHelper costHelper = JsonConvert.DeserializeObject<CostDataSerializeHelper>(jsonCost);
      allCost = costHelper.cost;
   }
}
[Serializable]
public class GameDataSerializeHelper
{
   public List<BaseData> item;
   public List<PlayerData> players;
}
public class CostDataSerializeHelper
{
   public List<CostData> cost;
}

