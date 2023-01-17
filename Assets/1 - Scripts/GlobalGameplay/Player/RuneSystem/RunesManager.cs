using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static NameManager;

public class RunesManager : MonoBehaviour
{
    public List<RuneSO> runes;
    [HideInInspector] public List<RuneSO> allSystemRunes = new List<RuneSO>();
    [HideInInspector] public List<RuneSO> calendarRunes = new List<RuneSO>();
    [HideInInspector] public List<RuneSO> enemySystemRunes = new List<RuneSO>();

    private List<RuneSO> createdRunes = new List<RuneSO>();
    private List<RuneSO> createdRunesInBild = new List<RuneSO>();
    private List<RuneSO> createdRunesFree = new List<RuneSO>();
    private Dictionary<RuneSO, int> createdRunesDict = new Dictionary<RuneSO, int>();

    private RunesType[] runesTypes;
    private int maxRuneLevel = 4;
    private int bossRuneLevel = 2;

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
            if(runes[i].source == BoostSender.Rune)
            {
                allSystemRunes.Add(runes[i]);
                List<Cost> runeCost = allSystemRunes[i].cost;
                for(int j = 0; j < runeCost.Count; j++)
                {
                    Cost realCost = new Cost();
                    realCost.type = runeCost[j].type;
                    realCost.amount = runeCost[j].amount * allSystemRunes[i].level;

                    allSystemRunes[i].realCost[j] = realCost;
                } 
            }

            if(runes[i].source == BoostSender.Calendar)
            {
                calendarRunes.Add(runes[i]);
            }

            if(runes[i].source == BoostSender.EnemySystem)
            {
                enemySystemRunes.Add(runes[i]);
            }
        }
    }

    public List<RuneSO> GetAvailableRunes()
    {
        return SortingRunes(createdRunesFree);
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
        //allSystemRunes.Remove(rune);
        //allSystemRunes = SortingRunes(allSystemRunes);
        //createdRunesInBild.Remove(rune);
        //allSystemRunes = SortingRunes(allSystemRunes);

        createdRunesFree.Remove(rune);
        createdRunesFree = SortingRunes(createdRunesFree);

        createdRunesInBild.Add(rune);
    }

    public void ClearCell(RuneSO rune)
    {
        //allSystemRunes.Add(rune);
        //allSystemRunes = SortingRunes(allSystemRunes);


        createdRunesFree.Add(rune);
        createdRunesFree = SortingRunes(createdRunesFree);

        createdRunesInBild.Remove(rune);
    }


    #region GETTINGS

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

    public string GetRuneDescription(RunesType type, bool descriptionMode)
    {
        string text;

        foreach(var rune in runes)
        {
            if(rune.rune == type)
            {
                text = (descriptionMode == true) ? rune.positiveDescription : rune.negativeDescription;
                return text;
            }
        }

        return "";
    }

    public bool GetRuneInvertion(RunesType type)
    {
        bool result;

        foreach(var rune in runes)
        {
            if(rune.rune == type)
            {
                result = rune.isInvertedRune;
                return result;
            }
        }

        return false;
    }

    public RuneSO GetRuneForBoss()
    {
        RuneSO bossRune = null;
        int count = 0;
        while(bossRune == null)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemySystemRunes.Count);
            count++;

            if(enemySystemRunes[randomIndex].level == bossRuneLevel && enemySystemRunes[randomIndex].rune == RunesType.EnemyHealth)
            //if(enemySystemRunes[randomIndex].level == bossRuneLevel)
                bossRune = enemySystemRunes[randomIndex];
        }

        return bossRune;
    }

    public List<RuneSO> GetEnemySystemRunes()
    {
        return enemySystemRunes;
    }

    #endregion


    #region RUNE WORKROOM

    public void AddCreatedRune(RuneSO rune)
    {
        if(createdRunesDict.ContainsKey(rune) == true)
        {
            createdRunesDict[rune]++;
        }
        else
        {
            createdRunesDict.Add(rune, 1);
        }

        //createdRunes.Add(rune);
        createdRunesFree.Add(rune);
    }

    public Dictionary<RuneSO, int> GetCreatedRunes()
    {
        return createdRunesDict;
    }

    public List<RuneSO> GetRunesForStorage()
    {
        List<RuneSO> runes = new List<RuneSO>();

        foreach(var rune in allSystemRunes)
        {
            if(rune.level == 1 && rune.source == BoostSender.Rune)
                runes.Add(rune);
        }

        return runes;
    }

    public RuneSO GetRune(RunesType type, int level)
    {
        RuneSO newRune = null;

        foreach(var rune in allSystemRunes)
        {
            if(rune.rune == type && rune.level == level)
            {
                newRune = rune;
                break;
            }
        }

        return newRune;
    }

    public List<RuneSO> GetRuneFamily(RunesType runeType)
    {
        List<RuneSO> runeList = new List<RuneSO>();

        foreach(var rune in allSystemRunes)
        {
            if(rune.rune == runeType)
                runeList.Add(rune);
        }

        return runeList;
    }

    public void DestroyRune()
    {

    }

    public int GetRunesAmount(RuneSO rune)
    {
        return (createdRunesDict.ContainsKey(rune) == true) ? createdRunesDict[rune] : 0;
    }
    #endregion
}
