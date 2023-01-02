using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private List<SpellSO> allSpells;
    //private List<SpellSO> availableSpells = new List<SpellSO>();
    private List<SpellSO> currentSpells = new List<SpellSO>();
    private Dictionary<Spells, int> spellLevels = new Dictionary<Spells, int>();

    private void Start()
    {
        //foreach(var spell in allSpells)
        //{
        //    if(spell.isFinded == true) availableSpells.Add(spell);            
        //}

        //foreach(var spell in availableSpells)
        //{
        //    spell.isUnlocked = true;
        //}

        foreach(var spell in allSpells)
        {
            if(spellLevels.ContainsKey(spell.spell) == false)
                spellLevels.Add(spell.spell, 0);
        }

        //currentSpells = availableSpells;
    }

    //TODO: handling for finding new spells and unlocking
    public void UnlockSpell(SpellSO spell)
    {
        currentSpells.Add(spell);

        spellLevels[spell.spell]++;


        //foreach(var item in availableSpells)
        //{
        //    if(item.spell == spell.spell) 
        //    {
        //        item.isUnlocked = true;
        //        break;
        //    }   
        //}
    }

    public void UpgradeSpell(SpellSO newSpell)
    {
        for(int i = 0; i < currentSpells.Count; i++)
        {
            if(currentSpells[i].spell == newSpell.spell)
            {
                currentSpells[i] = newSpell;
                spellLevels[currentSpells[i].spell]++;
                Debug.Log("Spell " + currentSpells[i].spell + " is " + spellLevels[currentSpells[i].spell] + " level now!");
                break;
            }
        }
    }

    public List<SpellSO> GetCurrentSpells()
    {
        return currentSpells;
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
}
