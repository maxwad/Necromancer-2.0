using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private List<SpellStat> allSpells;
    private List<SpellStat> availableSpells = new List<SpellStat>();
    private List<SpellStat> currentSpells = new List<SpellStat>();

    private void Start()
    {
        foreach(var spell in allSpells)
        {
            if(spell.isFinded == true) availableSpells.Add(spell);            
        }

        foreach(var spell in availableSpells)
        {
            if(spell.isUnlocked == true) currentSpells.Add(spell);
        }
    }

    //TODO: handling for finding new spells and unlocking

    public List<SpellStat> GetCurrentSpells()
    {
        return currentSpells;
    }
}
