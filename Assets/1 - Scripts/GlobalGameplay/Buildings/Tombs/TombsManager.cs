using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class TombsManager : MonoBehaviour
{
    private Dictionary<GameObject, Vector3> tombsDict = new Dictionary<GameObject, Vector3>();
    private List<SpellSO> hiddenSpells;

    private SpellManager spellManager;
    private Dictionary<GameObject, SpellSO> spellsInTombs = new Dictionary<GameObject, SpellSO>();

    public void Register(GameObject building, Vector3 position)
    {
        tombsDict.Add(building, position);
    }

    private void Start()
    {
        spellManager = GlobalStorage.instance.spellManager;
        hiddenSpells = spellManager.GetSpellsForTombs();
        HideSpells();
    }

    private void HideSpells()
    {
        while(hiddenSpells.Count < tombsDict.Count)
        {
            hiddenSpells.Add(null);
        }

        foreach(var tomb in tombsDict)
        {
            int index = Random.Range(0, hiddenSpells.Count);
            SpellSO spell = hiddenSpells[index];

            spellsInTombs.Add(tomb.Key, spell);
            hiddenSpells.RemoveAt(index);
        }
    }

    public Dictionary<GameObject, Vector3> GetTombs()
    {
        return tombsDict;
    }

    public SpellSO GetSpell(GameObject tomb)
    {
        return spellsInTombs[tomb];
    }

    public void UnlockSpell(SpellSO spell)
    {
        if(spell != null)
            spellManager.UnlockSpell(spell);
    }
}
