using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class GameMaster : MonoBehaviour
{
    private GlobalCamera gmCamera;
    private AISystem aiSystem;
    private InputSystem inputSystem;

    private bool isAIActive = true;

    //for Testing
    private bool isAIEnable = true;

    private void Start()
    {
        gmCamera = Camera.main.GetComponent<GlobalCamera>();
        aiSystem = GlobalStorage.instance.aiSystem;
        inputSystem = GlobalStorage.instance.inputSystem;
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
        //Debug.Log("All enemies finished their moves.");
        inputSystem.ActivateInput(true);
        //gmCamera.SetObserveObject();
    }

    //private void OnEnable()
    //{
    //    EventManager.NewMove += EnemyTurn;
    //}

    //private void OnDisable()
    //{
    //    EventManager.NewMove -= EnemyTurn;
    //}
}
