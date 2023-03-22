using System.Linq;
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
        createdRunesFree.Remove(rune);
        createdRunesFree = SortingRunes(createdRunesFree);

        createdRunesInBild.Add(rune);
    }

    public void ClearCell(RuneSO rune)
    {
        createdRunesFree.Add(rune);
        createdRunesFree = SortingRunes(createdRunesFree);

        createdRunesInBild.Remove(rune);
    }


    #region GETTINGS

    public RunesType[] GetRunesTypes() => runesTypes;

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

    public List<RuneSO> GetEnemySystemRunes() => enemySystemRunes;


    public RuneSO GetRune(RunesType type, int level)
    {
        return allSystemRunes.Where(i => i.rune == type && i.level == level).First();
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

        createdRunesFree.Add(rune);
    }

    public Dictionary<RuneSO, int> GetCreatedRunes() => createdRunesDict;

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

    public bool CanIDestroyRune(RuneSO rune) => createdRunesFree.Contains(rune);

    public void DestroyRune(RuneSO rune)
    {
        createdRunesDict[rune]--;

        if(createdRunesDict[rune] == 0)
            createdRunesDict.Remove(rune);

        createdRunesFree.Remove(rune);
    }

    public int GetRunesAmount(RuneSO rune)
    {
        return (createdRunesDict.ContainsKey(rune) == true) ? createdRunesDict[rune] : 0;
    }
    #endregion

    #region SAVE/LOAD

    public RunesEffectsLists Save()
    {
        RunesEffectsLists saveData = new RunesEffectsLists();

        foreach(var runeItem in createdRunesInBild)
        {
            RunesEffectsData runeData = new RunesEffectsData();
            runeData.rune = runeItem.rune;
            runeData.level = runeItem.level;

            saveData.createdRunesInBild.Add(runeData);
            saveData.createdRunes.Add(runeData);
        }

        foreach(var runeItem in createdRunesFree)
        {
            RunesEffectsData runeData = new RunesEffectsData();
            runeData.rune = runeItem.rune;
            runeData.level = runeItem.level;

            saveData.createdRunesFree.Add(runeData);
            saveData.createdRunes.Add(runeData);
        }

        foreach(var runeItem in createdRunesDict)
        {
            RunesEffectsData runeData = new RunesEffectsData();
            runeData.rune = runeItem.Key.rune;
            runeData.level = runeItem.Key.level;
            runeData.quantity = runeItem.Value;

            saveData.createdRunesDict.Add(runeData);
        }

        return saveData;
    }

    public void Load(RunesEffectsLists saveData)
    {
        //foreach(var runeItem in saveData.createdRunesInBild)
        //{
        //    RuneSO rune = GetRune(runeItem.rune, runeItem.level);
        //    createdRunesInBild.Add(rune);
        //}

        foreach(var runeItem in saveData.createdRunes)
        {
            RuneSO rune = GetRune(runeItem.rune, runeItem.level);
            createdRunesFree.Add(rune);
        }

        foreach(var runeItem in saveData.createdRunesDict)
        {
            RuneSO rune = GetRune(runeItem.rune, runeItem.level);
            if(createdRunesDict.ContainsKey(rune) == false)
            {
                createdRunesDict.Add(rune, runeItem.quantity);
            }
            else
            {
                Debug.Log("RUNE ERROR");
            }
        }
    }

    #endregion
}

public class RunesEffectsLists
{
    public List<RunesEffectsData> createdRunesInBild = new List<RunesEffectsData>();
    public List<RunesEffectsData> createdRunesFree = new List<RunesEffectsData>();
    public List<RunesEffectsData> createdRunes = new List<RunesEffectsData>();

    public List<RunesEffectsData> createdRunesDict = new List<RunesEffectsData>();
}