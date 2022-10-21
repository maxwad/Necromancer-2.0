using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesRowWrapper : MonoBehaviour
{
    [SerializeField] private List<RunePlaceItem> runesList;
    [SerializeField] private int rowNumber;
    [SerializeField] private bool isNegativeRow = false;


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

        float levelCount = levelUpManager.GetCurrentLevel();

        bool mode;

        for(int i = 0; i < runesList.Count; i++)
        {
            mode = (i + 1 > levelCount) ? false : true;

            runesList[i].Init(mode, isNegativeRow, rowNumber, i);
        }
    }

}
