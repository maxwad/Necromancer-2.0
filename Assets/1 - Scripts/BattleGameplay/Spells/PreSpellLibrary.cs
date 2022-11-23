using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class PreSpellLibrary : MonoBehaviour
{
   [ Serializable]
    public class PreSpellItem
    {
        public PreSpells preSpell;
        public GameObject preSpellGO;
        public int level;
    }

    public List<PreSpellItem> preSpellsList;

    public void Activate(SpellSO spell, bool mode)
    {
        PreSpells preSpell = EnumConverter.instance.SpellToPreEpell(spell.spell);

        foreach(var item in preSpellsList)
        {
            if(item.preSpell == preSpell && item.level == spell.level)
            {
                item.preSpellGO.SetActive(mode);
                item.preSpellGO.transform.localScale = new Vector3(spell.radius, spell.radius, 1) * 2;
            }
        }
    }

    private void DisableAllPreSpells(bool mode)
    {
        foreach(var item in preSpellsList)
        {
            item.preSpellGO.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += DisableAllPreSpells;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= DisableAllPreSpells;
    }
}
