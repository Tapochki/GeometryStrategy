using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Army
{ 
    public GameObject armyObject;
    private TextMeshPro _armyText;
    public int armyCount;
    private float _armySpeed;
    private SpaceBase _spaceBaseForAttack;
    private SpaceBase _homeSpaceBase;
    public Player armyOwner;
    private SpriteRenderer _baseColor;
    public event Action<Army, SpaceBase> armyEndMove;
    public Army(int armyCount, SpaceBase spaceBaseForAttack, SpaceBase homeSpaceBase)
    {
        this.armyCount = armyCount;
        _spaceBaseForAttack = spaceBaseForAttack;
        _homeSpaceBase = homeSpaceBase;
        _homeSpaceBase.UpdateText();
        armyOwner = _homeSpaceBase.baseOwner;
        armyObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>($"Prefab/SpaceArmy"), _homeSpaceBase.baseObject.gameObject.transform.position, quaternion.identity);
        if (armyOwner.playerType == Enums.PlayerType.Enemy)
        {
            armyObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }

        if (armyOwner.playerType == Enums.PlayerType.MainPlayer)
        {
            armyObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        _armySpeed = 1f;
        _armyText = armyObject.transform.Find("ArmyCountText").gameObject.GetComponent<TextMeshPro>();
        _armyText.text = this.armyCount.ToString();
    }
    public void Update()
    {
        try
        {
            if (_spaceBaseForAttack != null)
            {
                Vector3 direction = _spaceBaseForAttack.baseObject.transform.position - armyObject.transform.position;
                armyObject.transform.Translate(direction * _armySpeed * Time.deltaTime);
                if (Vector3.Distance(armyObject.transform.position, _spaceBaseForAttack.baseObject.transform.position) <=
                    0.5f)
                {
                    armyEndMove?.Invoke(this, _spaceBaseForAttack);
                    DestroyArmy();
                }
            }
            else
            {
                _homeSpaceBase.unitCount += armyCount;
                DestroyArmy();
            }
        }
        catch
        {
            DestroyArmy();
        }
       
    }
    public void DestroyArmy()
    {
        MonoBehaviour.Destroy(armyObject);
        armyOwner.playerArmy.Remove(this);
    }
    
}
