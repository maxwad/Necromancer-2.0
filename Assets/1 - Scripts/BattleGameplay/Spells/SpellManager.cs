using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SpellManager : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private List<SpellSO> allSpells;
    private List<SpellSO> readySpells = new List<SpellSO>();
    private Dictionary<Spells, int> spellsLevels = new Dictionary<Spells, int>();
    private Dictionary<Spells, SpellSO> findedSpells = new Dictionary<Spells, SpellSO>();

    private List<Spells> spellsInStorage = new List<Spells>();
    private List<Spells> spellsForBattle = new List<Spells>();

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
    }

    public void UnlockSpell(SpellSO spell)
    {
        foreach(var itemSpell in allSpells)
        {
            if(itemSpell.spell == spell.spell && itemSpell.level == 1)
            {
                if(spellsLevels.ContainsKey(spell.spell) == true) break;

                spellsLevels.Add(spell.spell, 0);
                findedSpells.Add(spell.spell, spell);
                //Debug.Log("Spell " + spell + " is unlock!");
            }
        }
    }

    public void UpgradeSpell(Spells spell)
    {
        int level = spellsLevels[spell] + 1;

        foreach(var itemSpell in allSpells)
        {
            if(itemSpell.spell == spell && itemSpell.level == level)
            {
                spellsLevels[spell] = level;
                findedSpells[spell] = itemSpell;
                break;
            }
        }

        foreach(var itemSpell in readySpells)
        {
            if(itemSpell.spell == spell)
            {
                readySpells.Remove(itemSpell);
                break;
            }
        }

        readySpells.Add(findedSpells[spell]);

        if(level == 1) 
            spellsInStorage.Add(findedSpells[spell].spell);
    }

    #region GETTINGS

    public List<SpellSO> GetCurrentSpells()
    {
        return readySpells;
    }

    public List<SpellSO> GetSpellsForTombs()
    {
        List<SpellSO> spellsForTombs = new List<SpellSO>();

        foreach(var spell in allSpells)
        {
            if(spell.level == 1)
                spellsForTombs.Add(spell);
        }

        return spellsForTombs;
    }

    public Dictionary<Spells, SpellSO> GetFindedSpells()
    {
        return findedSpells;
    }

    public int GetSpellMaxLevel(Spells spell)
    {
        int maxLevel = 0;
        foreach(var spellItem in allSpells)
        {
            if(spellItem.spell == spell && spellItem.level > maxLevel)
                maxLevel = spellItem.level;
        }

        return maxLevel;
    }

    public int GetSpelLevel(Spells spell)
    {
        return spellsLevels[spell];
    }

    public SpellSO GetNextLevelSpell(SpellSO spell)
    {
        foreach(var spellItem in allSpells)
        {
            if(spellItem.spell == spell.spell && spellItem.level == spell.level + 1)
                return spellItem;
        }

        return null;
    }

    public List<SpellSO> GetSpellsForBattle()
    {
        List<SpellSO> tempList = new List<SpellSO>();

        foreach(var spell in spellsForBattle)
            tempList.Add(findedSpells[spell]);

        return tempList;
    }

    public List<SpellSO> GetSpellsForStorage()
    {
        List<SpellSO> tempList = new List<SpellSO>();

        foreach(var spell in spellsInStorage)
            tempList.Add(findedSpells[spell]);

        return tempList;
    }

    #endregion

    #region WORKROOM

    public bool SwitchSpells(Spells spell, bool isFromStorage)
    {
        if(isFromStorage == true)
        {
            if(playerStats.GetCurrentParameter(PlayersStats.Spell) < spellsForBattle.Count + 1)
            {
                InfotipManager.ShowWarning("You no longer have free spell slots.");
                return false;
            }
        }

        List<Spells> source = (isFromStorage == true) ? spellsInStorage : spellsForBattle;
        List<Spells> target = (isFromStorage == true) ? spellsForBattle : spellsInStorage;

        foreach(var spellItem in source)
        {
            if(spellItem == spell)
            {
                target.Add(spell);
                break;
            }
        }

        source.Remove(spell);

        return true;
    }

    #endregion

}
