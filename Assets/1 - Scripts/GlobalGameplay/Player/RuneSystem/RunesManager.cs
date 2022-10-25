using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesManager : MonoBehaviour
{
    [HideInInspector] public RunesStorage runesStorage;
    private RunesWindow runesWindow;

    [HideInInspector] public List<RuneSO> availableRunes = new List<RuneSO>();

    public void Init()
    {
        runesStorage = gameObject.GetComponentInChildren<RunesStorage>();
        runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();

        runesStorage.Init();
        availableRunes = runesStorage.GetAvailableRunes();

        GlobalStorage.instance.LoadNextPart();
        //runesWindow
    }

    public void FillCell(RuneSO rune)
    {
        runesStorage.FillCell(rune);
    }
}
