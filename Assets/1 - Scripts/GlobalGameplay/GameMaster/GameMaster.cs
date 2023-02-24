using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class GameMaster : MonoBehaviour, IInputableKeys
{
    private SaveLoadManager saveLoadManager;
    private AISystem aiSystem;
    private InputSystem inputSystem;

    //for Testing
    private bool isAIEnable = true;

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        aiSystem = GlobalStorage.instance.aiSystem;
        inputSystem = GlobalStorage.instance.inputSystem;
        RegisterInputKeys();
    }

    public void RegisterInputKeys()
    {
        inputSystem = GlobalStorage.instance.inputSystem;
        inputSystem.RegisterInputKeys(KeyActions.SaveGame, this);
        inputSystem.RegisterInputKeys(KeyActions.LoadGame, this);
    }

    public void InputHandling(KeyActions keyAction)
    {
        if(keyAction == KeyActions.SaveGame)
            saveLoadManager.SaveGame();

        else if(keyAction == KeyActions.LoadGame)
            saveLoadManager.LoadGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F12) == true)
        {
            isAIEnable = !isAIEnable;
        }    
    }

    public void EnemyTurn()
    {
        if(isAIEnable == true)
        {
            inputSystem.ActivateInput(false);
            aiSystem.StartMoves();
        }
    }

    public void EndEnemyMoves()
    {
        inputSystem.ActivateInput(true);
    }

}
