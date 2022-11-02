using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static NameManager;

public class RunesStorage : MonoBehaviour
{
    public List<RuneSO> runes;
    [HideInInspector] public List<RuneSO> availableRunes = new List<RuneSO>();
    [HideInInspector] public List<RuneSO> hiddenRunes = new List<RuneSO>();

    private RunesType[] runesTypes;
    private int maxRuneLevel = 3;

    public void Init()
    {
        runesTypes = new RunesType[Enum.GetValues(typeof(RunesType)).Length];

        int counter = 0;
        foreach(RunesType itemType in Enum.GetValues(typeof(RunesType)))
        {
            runesTypes[counter] = itemType;
            counter++;
        }            


        for(int i = 0; i < runes.Count; i++)
        {
            availableRunes.Add(runes[i]);
            Cost[] runeCost = availableRunes[i].cost;
            for(int j = 0; j < runeCost.Length; j++)
            {
                Cost realCost = new Cost();
                realCost.type = runeCost[j].type;
                realCost.amount = runeCost[j].amount * availableRunes[i].level;

                availableRunes[i].realCost[j] = realCost;
            }            
        }

        //foreach(var rune in runes)
        //{
        //    availableRunes.Add(rune);
        //}
    }

    public List<RuneSO> GetAvailableRunes()
    {

        //Debug.Log(availableRunes[0]);
        return SortingRunes(availableRunes);
    }

    private List<RuneSO> SortingRunes(List<RuneSO> list)
    {
        List<RuneSO> sortedRunes = new List<RuneSO>();

        for(int i = 0; i < runesTypes.Length; i++)
        {
            RunesType cyrrentType = runesTypes[i];

            for(int level = maxRuneLevel; level > 0; level--)
            {
                foreach(var rune in list)
                {
                    if(rune.rune == cyrrentType && rune.level == level)
                    {
                        sortedRunes.Add(rune);
                        //Debug.Log(cyrrentType);
                    }
                }
            }
        }

        return sortedRunes;
    }

    public void FillCell(RuneSO rune)
    {
        availableRunes.Remove(rune);
        availableRunes = SortingRunes(availableRunes);
    }

    public void ClearCell(RuneSO rune)
    {
        availableRunes.Add(rune);
        availableRunes = SortingRunes(availableRunes);
    }

    public RunesType[] GetRunesTypes()
    {
        return runesTypes;
    }

    public Sprite GetRuneIcon(RunesType type)
    {
        Sprite icon;
        
        foreach(var rune in runes)
        {
            if(rune.rune == type)
            {
                icon = rune.activeIcon;
                return icon;
            }
        }

        return null;
    }

    public string GetRuneDescription(RunesType type)
    {
        string text;

        foreach(var rune in runes)
        {
            if(rune.rune == type)
            {
                text = rune.positiveDescription;
                return text;
            }
        }

        return "";
    }
}
