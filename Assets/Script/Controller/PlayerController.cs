using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PlayerController
{
    public GameObject selectedObject;
    private SpaceBase _spaceBaseForAttack;
    public SpaceBase selectedSpaceBase;
    public Player mainPlayer;
    private SpriteRenderer _baseColor;
    private Data _gameData;
    public event Action<int> playerGold;
    private Army _baseArmy;
    private Camera _camera;
    public PlayerController(Data gameData)
    {
        _gameData = gameData;
        mainPlayer = new Player(_gameData.allPlayer.Find(x => x.playerType == Enums.PlayerType.MainPlayer), _gameData);
        _camera = GameObject.Find("Camera").GetComponent<Camera>();   
        Start();
    }
    public void ArmyCapture(Army invaderArmy, SpaceBase attackedSpaceBase)
    {
        Debug.Log("Player Start Capture");
        if (attackedSpaceBase.baseOwner.playerType == invaderArmy.armyOwner.playerType)
        {
            Debug.Log("Its your capture");
            attackedSpaceBase.unitCount += invaderArmy.armyCount;
        }
        if (attackedSpaceBase.baseOwner.playerType != invaderArmy.armyOwner.playerType)
        {
            if (attackedSpaceBase.unitCount >= invaderArmy.armyCount)
            {
                Debug.Log("Low army Unit");
                attackedSpaceBase.unitCount -= invaderArmy.armyCount;
            }
            else if (attackedSpaceBase.unitCount < invaderArmy.armyCount)
            {
                invaderArmy.armyCount -= attackedSpaceBase.unitCount;
                attackedSpaceBase.unitCount = invaderArmy.armyCount;
                if (attackedSpaceBase.baseOwner.playerType == Enums.PlayerType.Enemy && invaderArmy.armyOwner.playerType == Enums.PlayerType.MainPlayer)
                {
                    MainApp.Instance.gameManager.enemyAIController.LostSpaceBase(attackedSpaceBase);
                    CaptureBase(attackedSpaceBase, invaderArmy.armyOwner);
                    Debug.Log("Player capture Enemy base");
                }
                if (attackedSpaceBase.baseOwner.playerType == Enums.PlayerType.Vacant && invaderArmy.armyOwner.playerType == Enums.PlayerType.MainPlayer)
                {
                    MainApp.Instance.gameManager.vacantController.LostSpaceBase(attackedSpaceBase); 
                    CaptureBase(attackedSpaceBase,invaderArmy.armyOwner);
                    Debug.Log("Player capture Vacant base");
                }
            }
        }
        attackedSpaceBase.UpdateText();
    }
    public void EndPlayer()
    {
        Deselect();
        mainPlayer.ClearPlayer();
    }
    public void LostSpaceBase(SpaceBase spaceBase)
    {
        if (spaceBase == selectedSpaceBase)
        {
            Deselect();
        }
        mainPlayer.playerBases.Remove(spaceBase);
    }
    public void CaptureBase(SpaceBase spaceBase, Player invader)
    {
        mainPlayer.playerBases.Add(spaceBase);
        spaceBase.baseOwner = invader;
        spaceBase.UpdateColor();
    }
    public void RestartPlayer(Data gameData)
    {
        _gameData = gameData;
        mainPlayer.UpdateData(_gameData, _gameData.allPlayer[0]);
    }
    public void PlayerAddResources()
    {
        mainPlayer.AddResources();
        playerGold.Invoke(mainPlayer.gold);
    }
    public void Update()
    {
        Debug.Log($" Mouse Position :{_camera.ScreenToWorldPoint(Input.mousePosition)}");
        mainPlayer.Update();
        if (Input.GetMouseButtonDown(0))
        {
            Select();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SendArmy();
        }
    }
    public void Start()
    {
        selectedSpaceBase = null;
        selectedObject = null;
    }
    private void Select()
    {
        selectedObject = null;
        Vector2 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.Raycast(curMousePos, Vector2.zero);
        if (rayHit.transform != null)
        {
            selectedObject = rayHit.transform.gameObject;
            for (int i = 0; i <= mainPlayer.playerBases.Count - 1; i++)
            { 
                if (mainPlayer.playerBases[i].baseObject == selectedObject) 
                {
                    if (selectedSpaceBase != null)
                    {
                        Deselect();
                    }
                    selectedSpaceBase = mainPlayer.playerBases[i];
                    _baseColor = selectedSpaceBase.baseObject.GetComponent<SpriteRenderer>();
                    Debug.Log("Select Base", selectedSpaceBase.baseObject);
                    SelectedBase();
                    break;
                }
                Deselect();
            }
        }
        else if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Ui Click");
        }
        else
        {
            Debug.Log("Miss");
            Deselect();
        }
    }

   
    private void SelectedBase()
    {
        if (selectedSpaceBase != null)
        {
            _baseColor.color = Color.magenta; 
            MainApp.Instance.uiManager.GetBase(selectedSpaceBase);
        }
    }
    public void Deselect()
    {
        if (selectedSpaceBase != null)
        {
            selectedSpaceBase.UpdateColor();
            selectedSpaceBase = null;
            MainApp.Instance.uiManager.BaseMenuDeActive();
        }
    }
    private SpaceBase SelectBaseForAttack(GameObject baseForAttackObject)
    {
        for(int i = 0; i<= mainPlayer.playerBases.Count - 1; i++)
        {
            if (baseForAttackObject == mainPlayer.playerBases[i].baseObject)
            {
                return mainPlayer.playerBases[i];
            }
        }
        return null;
    }
    public void SpawnArmy(SpaceBase spaceBaseForAttack, SpaceBase homeBase)
    {
        if (homeBase.unitCount > 1)
        {
            _baseArmy = new Army(homeBase.unitCount, spaceBaseForAttack, homeBase);
            _baseArmy.armyEndMove += ArmyCapture;
            homeBase.unitCount = 0;
            mainPlayer.playerArmy.Add(_baseArmy);
        }
        else
        {
            Debug.Log("Too far or few unit");
        }
    }
    public SpaceBase SelectBaseForAttack()
    {
        var choiseBase = Random.Range(0, mainPlayer.playerBases.Count - 1);
        return mainPlayer.playerBases[choiseBase];
    }
    public void SendArmy()
    {
        if (selectedSpaceBase != null)
        {
            selectedObject = null;
            Vector2 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(curMousePos, Vector2.zero, LayerMask.GetMask("Clickable"));
            if (rayHit.transform != null)
            {
                selectedObject = rayHit.transform.gameObject;
                
                _spaceBaseForAttack = MainApp.Instance.gameManager.enemyAIController.SelectBaseForAttack(selectedObject);
                if (_spaceBaseForAttack == null)
                {
                    _spaceBaseForAttack = SelectBaseForAttack(selectedObject);
                }

                if (_spaceBaseForAttack == null)
                {
                    _spaceBaseForAttack = MainApp.Instance.gameManager.vacantController.SelectBaseForAttack(selectedObject);
                }
                SpawnArmy(_spaceBaseForAttack, selectedSpaceBase);
            }
        }
    }
}
