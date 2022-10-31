using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class RunesManager : MonoBehaviour
{
    public class RuneBoost
    {
        public int level;
        public float boost;
        public int row;
        public int cell;

        public RuneBoost(int rowIndex, int cellIndex, RuneSO rune)
        {
            float value = rune.value;
            if(rowIndex == 1 && rune.rune != RunesType.CoolDown) value = -value;
            if(rowIndex != 1 && rune.rune == RunesType.CoolDown) value = -value;

            row = rowIndex;
            cell = cellIndex;
            boost = value;
            level = rune.level;
        }
    }

    private MacroLevelUpManager levelUpManager;
    private BattleBoostManager boostManager;
    [HideInInspector] public RunesStorage runesStorage;
    private RunesWindow runesWindow;

    [HideInInspector] public List<RuneSO> availableRunes = new List<RuneSO>();

    private List<RuneSO[]> runesRows = new List<RuneSO[]>();

    private RunesType[] runesTypes;
    private Dictionary<RunesType, List<RuneBoost>> runeBoostesDict = new Dictionary<RunesType, List<RuneBoost>>();
    private Dictionary<RunesType, float> commonBoostDict = new Dictionary<RunesType, float>();

    private float limitValue = -99;

    public void Init()
    {
        runesStorage = gameObject.GetComponentInChildren<RunesStorage>();
        runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
        levelUpManager = GlobalStorage.instance.macroLevelUpManager;
        boostManager = GlobalStorage.instance.unitBoostManager;

        runesStorage.Init();
        availableRunes = runesStorage.GetAvailableRunes();

        for(int i = 0; i < 3; i++)
        {
            runesRows.Add(new RuneSO[(int)levelUpManager.GetMaxLevel()]);
        }

        runesTypes = runesStorage.GetRunesTypes();

        foreach(var runeType in runesTypes)
        {
            runeBoostesDict[runeType] = new List<RuneBoost>();
            commonBoostDict[runeType] = 0;
        }

        GlobalStorage.instance.LoadNextPart();
        //runesWindow
    }

    public void FillCell(RuneSO rune)
    {
        runesStorage.FillCell(rune);
    }

    public void ClearCell(RuneSO rune)
    {
        runesStorage.ClearCell(rune);
    }

    public void ApplyRune(int row, int cell, RuneSO rune)
    {
        runesRows[row][cell] = rune;

        ApplyEffect(row, cell, rune);

        //for(int i = 0; i < runesRows.Count; i++)
        //{
        //    string runes = i + " row = ";
        //    for(int j = 0; j < runesRows[i].Length; j++)
        //    {
        //        if(runesRows[i][j] != null)
        //        {
        //            runes += (j + 1) + ":" + runesRows[i][j].name + " || ";
        //        }
        //    }

        //    Debug.Log(runes);
        //}

        CheckOverflow();
    }

    private void CheckOverflow()
    {
        RunesType overflowType;
        bool finded = false;
        int cell = 0;
        int row = 1;

        foreach(var boost in commonBoostDict)
        {
            if(boost.Value < limitValue)
            {
                overflowType = boost.Key;
                 List<RuneBoost> tempList = runeBoostesDict[overflowType];
                for(int i = 0; i < tempList.Count; i++)
                {
                    if(tempList[i].row == 1)
                    {
                        continue;
                    }
                    else
                    {
                        finded = true;
                        cell = tempList[tempList.Count - 1].cell;
                        row = tempList[tempList.Count - 1].row;
                        break;
                    }
                }                
            }
        }

        if(finded == true) runesWindow.FindAndClearRune(row, cell);
    }

    private void ApplyEffect(int row, int cell, RuneSO rune)
    {
        RunesType currentType = RunesType.Exp;

        if(rune != null)
        {
            runeBoostesDict[rune.rune].Add(new RuneBoost(row, cell, rune));
            currentType = rune.rune;
        }
        else
        {
            bool isfinded = false;

            for(int i = 0; i < runesTypes.Length; i++)
            {
                if(isfinded == true) break;

                List<RuneBoost> checkList = runeBoostesDict[runesTypes[i]];
                for(int j = 0; j < checkList.Count; j++)
                {
                    if(checkList[j].row == row && checkList[j].cell == cell)
                    {
                        checkList.Remove(checkList[j]);
                        currentType = runesTypes[i];
                        isfinded = true;
                        break;
                    }
                }
            }
        }

        float result = 0;
        List<RuneBoost> runeList = runeBoostesDict[currentType];

        for(int i = 0; i < runeList.Count; i++)
        {
            result += runeList[i].boost;
        }

        commonBoostDict[currentType] = result;

        runesWindow.UpdateParameters(currentType, result);
    }

    public bool CanIUseThisRune(bool negativeMode, RuneSO rune)
    {
        bool result;

        if(negativeMode == true)
        {
            if(rune.rune != RunesType.CoolDown)
                result = (commonBoostDict[rune.rune] - rune.value >= limitValue);
            else
                result = true;
        }
        else
        {
            if(rune.rune == RunesType.CoolDown)
                result = (commonBoostDict[rune.rune] - rune.value >= limitValue);
            else
                result = true;
        }

        return result;  
    }

    public bool CanIReplaceThisRune(bool negativeMode, RuneSO oldRune, RuneSO newRune)
    {
        bool result;

        if(negativeMode == true)
        {
            if(newRune.rune != RunesType.CoolDown)
                result = (commonBoostDict[oldRune.rune] - newRune.value + oldRune.value >= -99);
            else
                result = true;
        }
        else
        {
            if(newRune.rune == RunesType.CoolDown)
                result = (commonBoostDict[oldRune.rune] - newRune.value + oldRune.value >= -99);
            else
                result = true;
        }

        return result;
    }

    public void TurnOnRune(float level)
    {
        foreach(var boostList in runeBoostesDict)
        {
            foreach(var boost in boostList.Value)
            {
                if(boost.cell == level)
                {
                    boostManager.SetBoost(BoostConverter.instance.RuneToBoostType(boostList.Key), BoostSender.Rune, boost.boost);
                }
            }            
        }
    }
}
