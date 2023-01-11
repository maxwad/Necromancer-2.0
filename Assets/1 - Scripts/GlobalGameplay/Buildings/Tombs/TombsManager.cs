using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class TombsManager : MonoBehaviour
{
    private Dictionary<GameObject, TombInfo> tombsDict = new Dictionary<GameObject, TombInfo>();
    private List<SpellSO> hiddenSpells;

    private SpellManager spellManager;
    private ResourcesManager resourcesManager;
    private RewardManager rewardManager;
    /// <summary>
    /// 
    /// </summary>
    private int countForTest = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Insert) == true)
        {
            TestOpenSpell();
        }
    }

    private void TestOpenSpell()
    {
        List<GameObject> tombs = new List<GameObject>(tombsDict.Keys);
        if(countForTest > tombs.Count - 1) return;

        SpellSO spell = tombsDict[tombs[countForTest]].spell;

        countForTest++;
        if(spell == null)
        {
            TestOpenSpell();
        }
        else
        {
            UnlockSpell(spell);
        }
    }

    /// <summary>
    /// 
    /// </summary>


    public void Register(GameObject building)
    {
        TombInfo tombInfo = new TombInfo();
        tombInfo.position = building.transform.position;
        tombsDict.Add(building, tombInfo);
    }

    private void Start()
    {
        spellManager = GlobalStorage.instance.spellManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        rewardManager = GlobalStorage.instance.rewardManager;
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

            tomb.Value.spell = spell;
            hiddenSpells.RemoveAt(index);
        }
    }

    public void UnlockSpell(SpellSO spell)
    {
        if(spell != null)
            spellManager.UnlockSpell(spell);
    }

    #region GETTINGS

    public Dictionary<GameObject, TombInfo> GetTombs()
    {
        return tombsDict;
    }

    public SpellSO GetSpell(GameObject tomb)
    {
        return tombsDict[tomb].spell;
    }


    public Reward GetReward(GameObject tomb)
    {
        if(tombsDict[tomb].spell == null) 
            return null;
        else
        {
            if(tombsDict[tomb].reward == null)
            {
                Reward reward = rewardManager.GetTombReward();

                for(int i = 0; i < reward.resourcesList.Count; i++)
                    resourcesManager.ChangeResource(reward.resourcesList[i], reward.resourcesQuantity[i]);

                tombsDict[tomb].reward = reward;
            }

            return tombsDict[tomb].reward;
        }
    }

    #endregion
}
