using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MainApp : MonoBehaviour
{
    public static MainApp Instance;
    public GameManager gameManager;
    public UIManager uiManager;

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        GetManager();
    }
    public void GetManager()
    {
        gameManager = new GameManager();
        uiManager = new UIManager(gameManager);
        
    }
    private void Update()
    {
        gameManager.Update();
    }
}
