using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static NameManager;

public class Autobattle : MonoBehaviour
{
    private AutobattleUI autobattleUI;
    private PlayersArmy playersArmyScript;
    private ResourcesManager resourcesManager;
    private SpellManager spellManager;

    [Header("Start Parameters")]
    private UnitController[] playersArmy = new UnitController[4];
    private Army currentEnemyArmy;
    private List<EnemyController> enemyControllers = new List<EnemyController>();
    private List<SpellStat> currentSpells = new List<SpellStat>();

    private bool isAttackMode = false;
    private bool isManaMode = false;

    private float mana;
    private float health;

    [Header("Count Settings")]
    [SerializeField] private float enemiesPortionPerUnit = 50f;
    private float currentProportion;
    private bool isMagicEnemy = false;
    private bool isMagicUnits = false;

    private void Start()
    {
        autobattleUI = GlobalStorage.instance.autobattleUI;
        playersArmyScript = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
        spellManager = GlobalStorage.instance.spellManager;
    }

    public void Preview(Army enemyArmy)
    {
        if(IsCalculateNedeed() == false) return;

        currentEnemyArmy = enemyArmy;

        Init();

        CalculateStrengthOfSides();

        autobattleUI.ShowWindow(this);
    }

    private void Init()
    {
        playersArmy = playersArmyScript.GetPlayersArmyForAutobattle();
        currentSpells = spellManager.GetCurrentSpells();

        isAttackMode = autobattleUI.GetAttackMode();
        isManaMode = autobattleUI.GetManaMode();

        mana = resourcesManager.GetResource(ResourceType.Mana);
        health = resourcesManager.GetResource(ResourceType.Health);

        currentProportion = enemiesPortionPerUnit;
    }

    private void CalculateStrengthOfSides()
    {
        float magicAttackUnits = 0;
        float physicAttackUnits = 0;
        float magicDefenceUnits = 0;
        float physicDefenceUnits = 0;
        float commonQuantityUnits = 0;

        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null)
            {
                magicAttackUnits += playersArmy[i].magicAttack * playersArmy[i].quantity;
                physicAttackUnits += playersArmy[i].physicAttack * playersArmy[i].quantity;

                magicDefenceUnits += playersArmy[i].magicDefence * playersArmy[i].quantity;
                physicDefenceUnits += playersArmy[i].physicDefence * playersArmy[i].quantity;

                commonQuantityUnits += playersArmy[i].quantity;
            }
        }

        magicAttackUnits /= commonQuantityUnits;
        physicAttackUnits /= commonQuantityUnits;
        magicDefenceUnits /= commonQuantityUnits;
        physicDefenceUnits /= commonQuantityUnits;

        Debug.Log(magicAttackUnits + " , " + physicAttackUnits + " | " + magicDefenceUnits + " , " + physicDefenceUnits);

        float magicAttackEnemy = 0;
        float physicAttackEnemy = 0;
        float magicDefenceEnemy = 0;
        float physicDefenceEnemy = 0;
        float commonQuantityEnemies = 0;

        for(int i = 0; i < currentEnemyArmy.squadList.Count; i++)
        {
            EnemyController enemy = currentEnemyArmy.squadList[i].GetComponent<EnemyController>();
            int quantity = currentEnemyArmy.quantityList[i];

            magicAttackEnemy += enemy.magicAttack * quantity;
            physicAttackEnemy += enemy.physicAttack * quantity;

            magicDefenceEnemy += enemy.magicDefence * quantity;
            physicDefenceEnemy += enemy.physicDefence * quantity;

            commonQuantityEnemies += quantity;
        }

        magicAttackEnemy /= commonQuantityEnemies;
        physicAttackEnemy /= commonQuantityEnemies;
        magicDefenceEnemy /= commonQuantityEnemies;
        physicDefenceEnemy /= commonQuantityEnemies;

        Debug.Log(magicAttackEnemy + " , " + physicAttackEnemy + " | " + magicDefenceEnemy + " , " + physicDefenceEnemy);
    }

    private bool IsCalculateNedeed()
    {


        return true;
    }

}
