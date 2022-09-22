using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerResurrection playerResurrection;

    private GameObject globalPlayer;
    private GameObject battlePlayer;

    private GameObject globalMap;

    private void Start()
    {
        playerResurrection = GetComponent<PlayerResurrection>();
        globalPlayer = GlobalStorage.instance.globalPlayer.gameObject;
        battlePlayer = GlobalStorage.instance.battlePlayer.gameObject;

        globalMap = GlobalStorage.instance.globalMap;
    }

    private void MovePlayerToTheGlobal(bool mode)
    {
        if (mode == false)
        {
            //globalPlayer.SetActive(false);
            //globalMap.SetActive(false);

            battlePlayer.SetActive(true);
        }
        else
        {
            //globalPlayer.SetActive(true);
            //globalMap.SetActive(true);

            battlePlayer.SetActive(false);
            //battleMap we never turn off, just clear in BattleMap script
        }
    }


    private void ResurrectPlayer()
    {
        playerResurrection.StartResurrection();
    }

    private void OnEnable()
    {
        EventManager.ChangePlayer += MovePlayerToTheGlobal;
        EventManager.Defeat += ResurrectPlayer;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer -= MovePlayerToTheGlobal;
        EventManager.Defeat -= ResurrectPlayer;
    }

}
