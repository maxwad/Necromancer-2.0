using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private List<SpellSO> allSpells;
    private List<SpellSO> availableSpells = new List<SpellSO>();
    private List<SpellSO> currentSpells = new List<SpellSO>();

    private void Start()
    {
        foreach(var spell in allSpells)
        {
            if(spell.isFinded == true) availableSpells.Add(spell);            
        }

        foreach(var spell in availableSpells)
        {
            spell.isUnlocked = true;
        }

        currentSpells = availableSpells;
    }

    //TODO: handling for finding new spells and unlocking
    public void UnlockSpell(SpellSO spell)
    {
        foreach(var item in availableSpells)
        {
            if(item.spell == spell.spell) 
            {
                item.isUnlocked = true;
                break;
            }   
        }
    }

    public List<SpellSO> GetCurrentSpells()
    {
        return currentSpells;
    }
}
