using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static NameManager;

public class RunesStorage : MonoBehaviour
{
    public List<RuneSO> runes;
    [HideInInspector] public List<RuneSO> availableRunes = new List<RuneSO>();

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

        availableRunes = runes;
    }

    public List<RuneSO> GetAvailableRunes()
    {
        return  SortingRunes(availableRunes);
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
}
