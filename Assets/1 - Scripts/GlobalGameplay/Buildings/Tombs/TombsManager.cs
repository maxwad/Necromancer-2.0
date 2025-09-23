using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Enums;
using Zenject;

public class TombsManager : MonoBehaviour
{
    private Dictionary<GameObject, TombInfo> tombsDict = new Dictionary<GameObject, TombInfo>();
    private List<SpellSO> hiddenSpells;

    private SpellManager spellManager;
    private ResourcesManager resourcesManager;
    private RewardManager rewardManager;

    private int countForTest = 0;

    [Inject]
    public void Construct(
        SpellManager spellManager,
        ResourcesManager resourcesManager,
        RewardManager rewardManager)
    {
        this.spellManager = spellManager;
        this.resourcesManager = resourcesManager;
        this.rewardManager = rewardManager;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Insert) == true)
            TestOpenSpell();
    }

    private void TestOpenSpell()
    {
        List<GameObject> tombs = new List<GameObject>(tombsDict.Keys);
        if(countForTest > tombs.Count - 1) return;

        SpellSO spell = tombsDict[tombs[countForTest]].spell;

        countForTest++;
        if(spell == null)
            TestOpenSpell();
        else
            UnlockSpell(spell);
    }


    public void Register(GameObject building)
    {
        TombInfo tombInfo = new TombInfo();
        tombInfo.position = building.transform.position;
        tombsDict.Add(building, tombInfo);
    }

    public void HideSpells()
    {
        hiddenSpells = spellManager.GetSpellsForTombs();

        while(hiddenSpells.Count < tombsDict.Count)
            hiddenSpells.Add(null);

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

    #region SAVE/LOAD


    public List<TombsSD> GetSaveData()
    {
        List<TombsSD> saveData = new List<TombsSD>();

        foreach(var tomb in tombsDict)
        {
            TombsSD saveItem = new TombsSD();
            saveItem.position = tomb.Value.position.ToVec3();
            saveItem.status = tomb.Value.isVisited;
            saveItem.reward = tomb.Value.reward;
            saveItem.spell = (tomb.Value.spell == null) ? (Spells)(-1) : tomb.Value.spell.spell;

            EnemyArmyOnTheMap enemy = tomb.Key.GetComponent<EnemyArmyOnTheMap>();
            if(enemy == null)
            {
                saveItem.enemyGarrison = null;
            }
            else
            {
                EnemySD enemyData = new EnemySD();
                enemyData.typeOfArmy = enemy.typeOfArmy;
                enemyData.army = enemy.army;

                saveItem.enemyGarrison = enemyData;
            }

            saveData.Add(saveItem);
        }

        return saveData;
    }

    public void LoadTomb(List<TombsSD> tombsData)
    {
        foreach(var point in tombsDict.Keys.ToList())
        {
            foreach(var tomb in tombsData)
            {
                if(tomb.position.ToVector3() == point.transform.position)
                {
                    TombInfo tombInfo = new TombInfo();
                    tombInfo.position = point.transform.position;
                    tombInfo.isVisited = tomb.status;
                    tombInfo.reward = tomb.reward;
                    tombInfo.spell = ((int)tomb.spell == -1) ? null : spellManager.GetSpellsForTombs().First(s => s.spell == tomb.spell);

                    tombsDict[point] = tombInfo;

                    EnemyArmyOnTheMap enemy = point.GetComponent<EnemyArmyOnTheMap>();
                    if(tomb.enemyGarrison == null)
                    {
                        Destroy(enemy);
                        point.GetComponent<ObjectOwner>().SetVisitStatus(true);
                    }
                    else
                    {
                        enemy.typeOfArmy = tomb.enemyGarrison.typeOfArmy;
                        enemy.army = tomb.enemyGarrison.army;
                        enemy.isEnemyGarrison = true;
                    }

                    break;
                }
            }
        }
    }
    
    #endregion
}
