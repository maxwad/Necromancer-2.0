using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class GameMaster : MonoBehaviour
{
    private GlobalCamera gmCamera;
    private AISystem aiSystem;

    private void Start()
    {
        gmCamera = Camera.main.GetComponent<GlobalCamera>();
        aiSystem = GlobalStorage.instance.aiSystem;
    }

    private void EnemyTurn()
    {
        aiSystem.StartMoves();
    }

    public void EndEnemyMoves()
    {
        Debug.Log("All enemies finished their moves.");
        gmCamera.SetObserveObject();
    }

    private void OnEnable()
    {
        EventManager.NewMove += EnemyTurn;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= EnemyTurn;
    }
}
