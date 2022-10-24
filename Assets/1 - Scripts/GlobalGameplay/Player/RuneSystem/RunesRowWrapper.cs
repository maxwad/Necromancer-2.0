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
        //Init();
    }

    public void Init(bool negativeMode, bool conditionMode)
    {
        if(levelUpManager == null)
        {
            levelUpManager = GlobalStorage.instance.macroLevelUpManager;
        }

        float levelCount = levelUpManager.GetCurrentLevel();

        bool mode;
        isNegativeRow = negativeMode;

        for(int i = 0; i < runesList.Count; i++)
        {
            mode = (i + 1 > levelCount) ? false : true;

            if(negativeMode == false && conditionMode == false)
            {
                runesList[i].InitCell(mode, rowNumber, i);
            }

            if(negativeMode == true)
            {
                runesList[i].InitNegativeCell(mode, rowNumber, i);
            }

            if(conditionMode == true)
            {
                runesList[i].InitConductionCell(mode, rowNumber, i);
            }
        }
    }

    public List<RunePlaceItem> GetRunePlaceItem()
    {
        return runesList;
    }
}
