using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private List<SpellSO> allSpells;
    private List<SpellSO> readySpells = new List<SpellSO>();
    private Dictionary<Spells, int> spellsLevels = new Dictionary<Spells, int>();
    private Dictionary<Spells, SpellSO> findedSpells = new Dictionary<Spells, SpellSO>();

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
                readySpells.Add(findedSpells[spell]);
                break;
            }
        }

        Debug.Log("Spell " + spell + " is " + spellsLevels[spell] + " level now!");
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

    #endregion
}
