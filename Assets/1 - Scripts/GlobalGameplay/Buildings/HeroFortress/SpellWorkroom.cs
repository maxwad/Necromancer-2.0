using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SpellWorkroom : SpecialBuilding
{
    private SpellManager spellManager;

    private Dictionary<Spells, SpellSO> findedSpells = new Dictionary<Spells, SpellSO>();

    [Header("UI")]
    [SerializeField] private GameObject warning;
    [SerializeField] private List<SpellItemUI> spellItemUIList;

    [SerializeField] private SpellDetailUI currentSpellDetails;
    [SerializeField] private SpellDetailUI nextLevelSpellDetails;

    private SpellSO currentSpell;
    

    public override GameObject Init(CastleBuildings building)
    {
        if(spellManager == null)
        {
            spellManager = GlobalStorage.instance.spellManager;
        }

        ResetForm();

        gameObject.SetActive(true);
        return gameObject;
    }


    #region SETTINGS

    private void ResetForm()
    {
        ResetSpellStorage();

        findedSpells = spellManager.GetFindedSpells();
        warning.SetActive(!(findedSpells.Count > 0));

        InitSpellStorage();

        EnableDetails(false, false);
    }

    private void ResetSpellStorage()
    {
        foreach(var spellItemUI in spellItemUIList)
        {
            spellItemUI.ResetItem();
        }
    }

    private void InitSpellStorage()
    {
        int counter = 0;
        foreach(var spell in findedSpells)
        {
            if(counter >= spellItemUIList.Count)
                break;

            spellItemUIList[counter].Init(this, spell.Value);
            counter++;
        }        
    }

    public void SetCurrentSpell(SpellSO spell)
    {
        foreach(var spellItemUI in spellItemUIList)
        {
            spellItemUI.EnableLightning(false);
        }

        currentSpell = spell;
        ShowSpellsDetails();
    }


    private void EnableDetails(bool firstMode, bool secondMode)
    {
        currentSpellDetails.gameObject.SetActive(firstMode);
        nextLevelSpellDetails.gameObject.SetActive(firstMode);
    }
    #endregion


    #region GETTINGS

    public int GetSpellMaxLevel(Spells spell)
    {
        return spellManager.GetSpellMaxLevel(spell);
    }   
    
    public int GetSpellLevel(Spells spell)
    {
        return spellManager.GetSpelLevel(spell);
    }

    #endregion

    public void ShowSpellsDetails()
    {
        int spellLevel = GetSpellLevel(currentSpell.spell);

        bool createMode = false;
        bool nextLevelMode = false;

        if(spellLevel == 0)
        {
            createMode = true;
        }
        else if(spellLevel < GetSpellMaxLevel(currentSpell.spell))
        {
            nextLevelMode = true;
        }

        EnableDetails(true, nextLevelMode);
        currentSpellDetails.FillData(this, currentSpell, null, true, createMode);

        if(nextLevelMode == true)
        {
            SpellSO nextLevelSpell = spellManager.GetNextLevelSpell(currentSpell);
            nextLevelSpellDetails.gameObject.SetActive(true);
            nextLevelSpellDetails.FillData(this, nextLevelSpell, currentSpell, false, false);
        }
        else
        {
            nextLevelSpellDetails.gameObject.SetActive(false);
        }
    }

    public void UpgradeSpell(SpellSO spell)
    {
        spellManager.UpgradeSpell(spell.spell);

        ResetForm();

        foreach(var item in spellItemUIList)
        {
            if(item.CheckSpell(currentSpell.spell))
            {
                item.Select();
                break;
            }
        }
    }
}