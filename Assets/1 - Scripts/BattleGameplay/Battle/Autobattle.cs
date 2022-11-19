using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResultOfAutobattle
{
    public bool result;

    public float losses;
    public float minLosses;
    public float maxLosses;

    public float healthLosses;
    public float manaLosses;
}

public class Autobattle : MonoBehaviour
{
    private AutobattleUI autobattleUI;
    private PlayersArmy playersArmyScript;
    private ResourcesManager resourcesManager;
    private BattleManager battleManager;
    private SpellManager spellManager;
    private PlayerStats playerStats;
    private EnemyManager enemyManager;

    [Header("Start Parameters")]
    private Unit[] playersArmy = new Unit[4];
    private Army currentEnemyArmy;
    private List<SpellStat> currentSpells = new List<SpellStat>();

    private bool isAttackMode = false;
    private bool isManaMode = false;
    private bool isVictory = false;

    private float mana;
    private float lossesMana = 0;
    private float health;
    private float lossesHealth = 0;
    private float luck;
    private float averageLevel = 1;
    private float unitsQuantity = 0;
    private float enemiesQuantity = 0;
    private float lossesGap = 0.2f;
    private float attackReward = 60f;
    private float defenceReward = 30f;


    [Header("Count Settings")]
    [SerializeField] private float enemiesPortionPerUnit = 50f;
    private float currentProportion;
    [SerializeField] private float delta = 10f;
    private bool isMagicEnemy = false;
    private bool isMagicUnits = false;

    private ResultOfAutobattle result;
    private bool isRecalculating = false;

    private void Start()
    {
        autobattleUI = GlobalStorage.instance.autobattleUI;
        playersArmyScript = GlobalStorage.instance.playersArmy;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        battleManager = GlobalStorage.instance.battleManager;
        spellManager = GlobalStorage.instance.spellManager;
        playerStats = GlobalStorage.instance.playerStats;
        enemyManager = GlobalStorage.instance.enemyManager;
    }

    public void Preview(Army enemyArmy)
    {
        currentEnemyArmy = enemyArmy;
        isRecalculating = false;

        StartCalculating();
    }

    public void StartCalculating()
    {
        playersArmy = playersArmyScript.GetPlayersArmyForAutobattle();
        currentSpells = spellManager.GetCurrentSpells();

        isAttackMode = autobattleUI.GetAttackMode();
        isManaMode = autobattleUI.GetManaMode();
        isVictory = false;

        luck = playerStats.GetCurrentParameter(PlayersStats.Luck);
        mana = resourcesManager.GetResource(ResourceType.Mana);
        health = resourcesManager.GetResource(ResourceType.Health);

        currentProportion = enemiesPortionPerUnit;
        lossesMana = 0;
        lossesHealth = 0;
        averageLevel = 1;
        unitsQuantity = 0;
        enemiesQuantity = 0;

        result = null;

        CalculateStrengthOfSides();
    }


    private void CalculateStrengthOfSides()
    {
        // Strength of player's army
        float magicAttackUnits = 0;
        float physicAttackUnits = 0;
        float magicDefenceUnits = 0;
        float physicDefenceUnits = 0;
        float commonQuantitySquads = 0;
        float level = 0;

        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null)
            {
                magicAttackUnits += playersArmy[i].magicAttack * playersArmy[i].unitController.quantity;
                physicAttackUnits += playersArmy[i].physicAttack * playersArmy[i].unitController.quantity;

                magicDefenceUnits += playersArmy[i].magicDefence * playersArmy[i].unitController.quantity;
                physicDefenceUnits += playersArmy[i].physicDefence * playersArmy[i].unitController.quantity;

                unitsQuantity += playersArmy[i].unitController.quantity;

                level += playersArmy[i].level * playersArmy[i].unitController.quantity;
                commonQuantitySquads++;
            }
        }

        if(unitsQuantity == 0) 
        {
            currentProportion = 0;
            CalculateResults();
            return;
        }

        magicAttackUnits /= unitsQuantity;
        physicAttackUnits /= unitsQuantity;
        magicDefenceUnits /= unitsQuantity;
        physicDefenceUnits /= unitsQuantity;

        float quantityMode;
        quantityMode = commonQuantitySquads * delta;
        currentProportion += quantityMode;
        //Debug.Log(magicAttackUnits + " , " + physicAttackUnits + " | " + magicDefenceUnits + " , " + physicDefenceUnits);


        // Strength of enemy's army
        float magicAttackEnemy = 0;
        float physicAttackEnemy = 0;
        float magicDefenceEnemy = 0;
        float physicDefenceEnemy = 0;

        for(int i = 0; i < currentEnemyArmy.squadList.Count; i++)
        {
            EnemySO enemy = enemyManager.GetEnemySO(currentEnemyArmy.squadList[i]);
            int quantity = currentEnemyArmy.quantityList[i];

            magicAttackEnemy += enemy.magicAttack * quantity;
            physicAttackEnemy += enemy.physicAttack * quantity;

            magicDefenceEnemy += enemy.magicDefence * quantity;
            physicDefenceEnemy += enemy.physicDefence * quantity;

            enemiesQuantity += quantity;
        }

        magicAttackEnemy /= enemiesQuantity;
        physicAttackEnemy /= enemiesQuantity;
        magicDefenceEnemy /= enemiesQuantity;
        physicDefenceEnemy /= enemiesQuantity;
        //Debug.Log(magicAttackEnemy + " , " + physicAttackEnemy + " | " + magicDefenceEnemy + " , " + physicDefenceEnemy);


        // Check attack mode
        float modeDelta = 0;
        if(isAttackMode == true)
        {
            modeDelta -= delta;

            isMagicUnits = magicAttackUnits > physicAttackUnits;
            isMagicEnemy = magicDefenceEnemy > physicDefenceEnemy;

            if(isMagicUnits != isMagicEnemy)
                modeDelta += delta;
            else
                modeDelta -= delta;

            // maybe we need to add calculating influention strength enemy on army of hero
        }
        else
        {
            modeDelta += delta;

            isMagicUnits = magicDefenceUnits > physicDefenceUnits;
            isMagicEnemy = magicAttackEnemy > physicAttackEnemy;

            if(isMagicUnits != isMagicEnemy)
                modeDelta += delta;
            else
                modeDelta -= delta;
        }
        currentProportion += modeDelta;


        // Compare strengts
        float levelMode = 0;
        averageLevel = level / unitsQuantity;

        if(averageLevel > (float)currentEnemyArmy.strength)
            levelMode += (delta * Mathf.Ceil(averageLevel - (float)currentEnemyArmy.strength));

        if(averageLevel < (float)currentEnemyArmy.strength)
            levelMode -= (delta * Mathf.Ceil((float)currentEnemyArmy.strength) - averageLevel);

        currentProportion += levelMode;


        // Add mana effect
        if(isManaMode == true) currentProportion += CalculateStrengthOfSpells(isAttackMode);


        // Check battle mode
        if(currentEnemyArmy.isThisASiege == true) currentProportion -= delta;


        // Add boost system effect

        CalculateResults();
    }


    private float CalculateStrengthOfSpells(bool tacticMode)
    {
        float spellDelta;
        float lowestCost = 10000;
        float commonCost = 0;
        float countOfSpells = 0;

        foreach(var spell in currentSpells)
        {
            if((tacticMode == true && spell.type == TypeOfSpell.Attack) ||
               (tacticMode == false && spell.type == TypeOfSpell.Defence)) 
            {
                countOfSpells++;
                commonCost += spell.manaCost;

                if(spell.manaCost < lowestCost) lowestCost = spell.manaCost;
            }
        }
        //Debug.Log("We have " + currentSpells.Count + " spells. " + countOfSpells + " of them are good.");
        if(mana < lowestCost || commonCost == 0) return 0f;

        float manaMultiplier = Mathf.Ceil(mana / commonCost);

        //Debug.Log(manaMultiplier);

        spellDelta = manaMultiplier * delta;
        lossesMana = (mana > commonCost) ? commonCost : mana;

        spellDelta += countOfSpells * delta;

        //Debug.Log("Mana gives you " + spellDelta + "points");

        return spellDelta;
    }


    private void CalculateResults()
    {
        result = new ResultOfAutobattle();

        float lossesResult;
        float minLosses;
        float maxLosses;

        //Debug.Log(currentProportion + ": You will lose units: " + Mathf.Round(enemiesQuantity / currentProportion) + " on " + enemiesQuantity + " enemies");
        lossesResult = Mathf.Round(enemiesQuantity / currentProportion);

        if(lossesResult >= unitsQuantity || unitsQuantity == 0) 
        {
            isVictory = false;
            result.result = isVictory;
            result.losses = unitsQuantity;
            result.healthLosses = health;
            result.manaLosses = lossesMana;
        }
        else
        {
            isVictory = true;
            result.result = isVictory;

            result.losses = lossesResult;

            minLosses = lossesResult - Mathf.Round(lossesResult * lossesGap);
            if(minLosses < 0) minLosses = 0;
            result.minLosses = minLosses;

            maxLosses = lossesResult + Mathf.Round(lossesResult * lossesGap);
            if(maxLosses >= unitsQuantity) maxLosses -= 1;
            result.maxLosses = maxLosses;

            lossesHealth = Mathf.Floor(lossesResult / unitsQuantity * health);
            result.healthLosses = lossesHealth;

            result.manaLosses = lossesMana;
        }

        if(isRecalculating == false)
            autobattleUI.ShowResult(this, result);
        else
        {
            autobattleUI.FillWindow(result);
        }
    }

    public void Recalculating()
    {
        isRecalculating = true;
        StartCalculating();

    }

    public void AcceptResult()
    {
        float finalLosses;

        if(Random.Range(0, 101) <= luck)
            finalLosses = result.minLosses;
        else
            finalLosses = Random.Range(result.minLosses, result.maxLosses);

        playersArmyScript.EscapeDamage(finalLosses);

        if(result.healthLosses != 0)
            resourcesManager.ChangeResource(ResourceType.Health, -result.healthLosses);

        if(result.manaLosses != 0)
            resourcesManager.ChangeResource(ResourceType.Mana, -result.manaLosses);


        int resultMode = 0;
        if(result.result == true) resultMode = 1;

        float percentOfReward = defenceReward;
        if(isAttackMode == true) percentOfReward = attackReward;


        battleManager.FinishTheBattle(true, resultMode, percentOfReward);
    }

}
