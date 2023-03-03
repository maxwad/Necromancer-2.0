using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class RunesSystem : MonoBehaviour
{
    private MacroLevelUpManager levelUpManager;
    private BoostManager boostManager;
    [HideInInspector] public RunesManager runesStorage;
    private RunesWindow runesWindow;

    //[HideInInspector] public List<RuneSO> availableRunes = new List<RuneSO>();
    //[HideInInspector] public List<RuneSO> hiddenRunes = new List<RuneSO>();

    private List<RuneSO[]> runesRows = new List<RuneSO[]>();

    private RunesType[] runesTypes;
    private Dictionary<RunesType, List<RuneBoost>> runeBoostesDict = new Dictionary<RunesType, List<RuneBoost>>();
    private Dictionary<RunesType, float> commonBoostDict = new Dictionary<RunesType, float>();

    private float limitValue = -99;

    //private int shardOfRunes = 0;

    public void Init()
    {
        runesStorage = gameObject.GetComponentInChildren<RunesManager>();
        runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
        levelUpManager = GlobalStorage.instance.macroLevelUpManager;
        boostManager = GlobalStorage.instance.boostManager;

        runesStorage.Init();

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
                Debug.Log("We find problem with " + boost.Key);
                bool isRuneInverted = runesStorage.GetRuneInvertion(boost.Key);
                overflowType = boost.Key;
                List<RuneBoost> tempList = runeBoostesDict[overflowType];
                for(int i = 0; i < tempList.Count; i++)
                {
                    if(isRuneInverted == true)
                    {
                        Debug.Log("Rune INVERTED");
                        if(tempList[i].row == 1)
                        {
                            continue;
                        }
                        else
                        {
                            Debug.Log("Try to find overflow in positive cells");
                            finded = true;
                            cell = tempList[tempList.Count - 1].cell;
                            row = tempList[tempList.Count - 1].row;
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("Rune NOT inverted");
                        if(tempList[i].row != 1)
                        {
                            continue;
                        }
                        else
                        {
                            Debug.Log("Try to find overflow in negative cells");
                            finded = true;
                            cell = tempList[tempList.Count - 1].cell;
                            row = tempList[tempList.Count - 1].row;
                            break;
                        }
                    }
                    
                }                
            }
        }

        if(finded == true) runesWindow.FindAndClearRune(row, cell);
    }

    private void ApplyEffect(int row, int cell, RuneSO rune)
    {
        RunesType currentType = RunesType.WeaponSpeed;

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
            if(rune.isInvertedRune == false)
                result = (commonBoostDict[rune.rune] - rune.value >= limitValue);
            else
                result = true;
        }
        else
        {
            if(rune.isInvertedRune == true)
                result = (commonBoostDict[rune.rune] - rune.value >= limitValue);
            else
                result = true;
        }

        return result;  
    }

    public bool CanIReplaceThisRune(bool negativeMode, RuneSO oldRune, RuneSO newRune, int row, int cell)
    {
        bool result;

        if(negativeMode == true)
        {
            if(newRune.isInvertedRune == false)
                result = (commonBoostDict[oldRune.rune] - newRune.value + oldRune.value >= limitValue);
            else
                result = true;
        }
        else
        {
            if(newRune.isInvertedRune == true)
                result = (commonBoostDict[oldRune.rune] - newRune.value + oldRune.value >= limitValue);
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
                    boostManager.SetBoost(TypesConverter.RuneToBoostType(boostList.Key), BoostSender.Rune, BoostEffect.PlayerBattle, boost.boost);
                }
            }            
        }
    }
}
