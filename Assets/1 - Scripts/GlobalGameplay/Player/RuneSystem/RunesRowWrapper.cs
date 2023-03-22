using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesRowWrapper : MonoBehaviour
{
    private RunesWindow runesWindow;
    [SerializeField] private List<RunePlaceItem> runesList;
    [SerializeField] private int rowNumber;
    [SerializeField] private bool isNegativeRow = false;

    public void Start()
    {
        //Init();
    }

    public void Init(RunesWindow rw)
    {
        runesWindow = rw;
    }

    public void Init(RunesWindow rw, float level, bool negativeMode, bool conditionMode)
    {
        bool mode;
        isNegativeRow = negativeMode;

        for(int i = 0; i < runesList.Count; i++)
        {
            mode = (i + 1 > level) ? false : true;

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

    public void ForceRuneClearing(int cell)
    {
        runesList[cell].ClearCell(); ;
    }

    public int CheckCell(int index)
    {
        return (runesList[index].currentRune == null) ? -1 : runesList[index].currentRune.level;
    }

    public List<RunesEffectsData> GetRunes()
    {
        List<RunesEffectsData> runeList = new List<RunesEffectsData>();

        foreach(var rune in runesList)
        {
            RunesEffectsData runeData = null;

            if(rune.currentRune != null)
            {
                runeData = new RunesEffectsData();
                runeData.rune = rune.currentRune.rune;
                runeData.level = rune.currentRune.level;
            }

            runeList.Add(runeData);
        }

        return runeList;
    }

    public void LoadRunes(List<RunesEffectsData> runeList)
    {
        for(int i = 0; i < runeList.Count; i++)
        {
            if(runeList[i] != null)
            {
                GameObject runeGO = runesWindow.CreateRuneForLoading(runeList[i].rune, runeList[i].level);
                runesList[i].SetParameters(rowNumber, i);
                runesList[i].InsertRune(runeGO, true);
            }
        }
        
    }

}
