using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class AbilitiesStorage : MonoBehaviour
{
    [SerializeField] private List<MacroAbilitySO> allAbilities;
    private Dictionary<PlayersStats, List<MacroAbilitySO>> availableDict = new Dictionary<PlayersStats, List<MacroAbilitySO>>();
    private Dictionary<PlayersStats, List<MacroAbilitySO>> unlockedDict = new Dictionary<PlayersStats, List<MacroAbilitySO>>();
    private Dictionary<PlayersStats, List<MacroAbilitySO>> sortingDict = new Dictionary<PlayersStats, List<MacroAbilitySO>>();

    private void Awake()
    {        
        foreach(var card in allAbilities)
        {
            List<MacroAbilitySO> currentList = new List<MacroAbilitySO>();

            if(availableDict.ContainsKey(card.abilitySeries) == true)
            {
                currentList = availableDict[card.abilitySeries];
                currentList.Add(card);
                availableDict[card.abilitySeries] = currentList;
            }
            else
            {
                currentList.Add(card);
                availableDict[card.abilitySeries] = currentList;
            }
        }

        foreach(var item in availableDict)
        {
            List<MacroAbilitySO> currentList = SortingByLevel(item.Value);

            sortingDict[item.Key] = currentList;

            //foreach(var a in currentList)
            //{
            //    Debug.Log(a.abilitySeries + " = " + a.level);
            //}
        }

        availableDict = sortingDict;
        sortingDict = null;

        //Debug.Log("Count dict = " + availableDict.Count);
    }

    private List<MacroAbilitySO> SortingByLevel(List<MacroAbilitySO> oldList)
    {
        List<MacroAbilitySO> newList = new List<MacroAbilitySO>();
        MacroAbilitySO minLevelAbility;

        while(oldList.Count != 0)
        {
            minLevelAbility = oldList[0];

            foreach(var item in oldList)
            {
                if(item.level < minLevelAbility.level)
                {
                    minLevelAbility = item;
                }
            }

            newList.Add(minLevelAbility);
            oldList.Remove(minLevelAbility);
        }

        return newList;
    }


    public List<MacroAbilitySO> GetAbilitiesForNewLevel(int countOfVariants)
    {
        List<MacroAbilitySO> newList = new List<MacroAbilitySO>();
        List<PlayersStats> abilitiesList = new List<PlayersStats>();

        foreach(var item in availableDict)
        {
            abilitiesList.Add(item.Key);
        }

        for(int i = 0; i < countOfVariants; i++)
        {
            if(abilitiesList.Count == 0) break;
 
            int randomIndex = Random.Range(0, abilitiesList.Count);
            PlayersStats randomAbility = abilitiesList[randomIndex];
            newList.Add(availableDict[randomAbility][0]);
            abilitiesList.Remove(randomAbility);

            //Debug.Log("Take " + randomAbility + " level " + availableDict[randomAbility][0].level);
        }

        return newList;
    }

    public void UnlockAbility(PlayersStats abilityType, int level)
    {

    }

    

}
