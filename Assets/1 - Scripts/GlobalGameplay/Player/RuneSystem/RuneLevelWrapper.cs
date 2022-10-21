using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneLevelWrapper : MonoBehaviour
{
    [SerializeField] private List<RuneLevelItem> levelList;
    private bool isListReversed = false;

    private MacroLevelUpManager levelUpManager;

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        if(levelUpManager == null)
        {
            levelUpManager = GlobalStorage.instance.macroLevelUpManager;
        }

        if(isListReversed == false)
        {
            levelList.Reverse();
            isListReversed = true;
        }

        float levelCount = levelUpManager.GetCurrentLevel();

        bool mode;

        for(int i = 0; i < levelList.Count; i++)
        {
            mode = (i + 1 > levelCount) ? false : true;

            levelList[i].Init(mode, i + 1);
        }

    }
}
