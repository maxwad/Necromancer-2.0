using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Army
{
    public List<GameObject> squadList = new List<GameObject>();
    public List<int> quantityList = new List<int>();
    public bool isThisASiege = false;
    public TypeOfArmy typeOfArmy = TypeOfArmy.OnTheMap;
    public ArmyStrength strength = ArmyStrength.Low;
    // more parameters
}

public class EnemySquadGenerator : MonoBehaviour
{
    [HideInInspector] public List<Army> allArmies = new List<Army>();
    [HideInInspector] public List<GameObject> allEnemiesPrefabList = new List<GameObject>();

    private float playerLevel;
    private float countOfSquad;
    private float sizeOfSquadMultiplier;
    private float maxCountOfSquad = 12; //because enemy UI window size allow only 12 squads
    private float enemyQuantityDivider = 2;
    private float enemySizeDivider = 5;
    private float enemyPortion = 100;
    private float percentGap = 0.2f;

    public void SetAllEnemiesList(List<GameObject> enemies)
    {
        allEnemiesPrefabList = enemies;
    }

    public Army GenerateArmy(ArmyStrength armyStrength, TypeOfArmy typeOfArmy)
    {
        //we need to separate different strenght od army

        playerLevel = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Level);

        countOfSquad = Mathf.Ceil(playerLevel / enemyQuantityDivider);
        if(countOfSquad > maxCountOfSquad) countOfSquad = maxCountOfSquad;

        sizeOfSquadMultiplier = Mathf.Ceil(playerLevel / enemySizeDivider) * (int)armyStrength;

        Army newArmy = new Army();
        newArmy.strength = armyStrength;
        newArmy.typeOfArmy = typeOfArmy;

        for(int i = 0; i < countOfSquad; i++)
        {
            int randomIndex = Random.Range(0, allEnemiesPrefabList.Count);
            GameObject randomSquad = allEnemiesPrefabList[randomIndex];
            newArmy.squadList.Add(randomSquad);

            int randomQuantity = (int)sizeOfSquadMultiplier * Mathf.RoundToInt(
                Random.Range((enemyPortion - enemyPortion * percentGap), (enemyPortion + enemyPortion * percentGap))
                );
            newArmy.quantityList.Add(randomQuantity);
        }

        allArmies.Add(newArmy);

        return newArmy;
    }

    public void RemoveArmy(Army army)
    {
        allArmies.Remove(army);
    }

    public void ClearAllArmies()
    {
        allArmies.Clear();
    }
}
