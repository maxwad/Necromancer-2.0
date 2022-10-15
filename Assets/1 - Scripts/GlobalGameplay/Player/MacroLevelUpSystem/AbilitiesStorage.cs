using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class AbilitiesStorage : MonoBehaviour
{
    [SerializeField] private List<MacroAbilitySO> allAbilities;
    private AbilitiesPlacing abilitiesPlacing;
    private Dictionary<PlayersStats, List<MacroAbilitySO>> availableAbilitiesDict = new Dictionary<PlayersStats, List<MacroAbilitySO>>();
    private Dictionary<PlayersStats, List<MacroAbilitySO>> openedAbilitiesDict = new Dictionary<PlayersStats, List<MacroAbilitySO>>();
    private Dictionary<PlayersStats, List<MacroAbilitySO>> sortingDict = new Dictionary<PlayersStats, List<MacroAbilitySO>>();

    public int abilitiesCount = 0;
    private int abilitiesOpened = 0;
    private int abilitiesLeft = 0;

    public void Init()
    {        
        foreach(var card in allAbilities)
        {
            List<MacroAbilitySO> currentList = new List<MacroAbilitySO>();

            if(availableAbilitiesDict.ContainsKey(card.abilitySeries) == true)
            {
                currentList = availableAbilitiesDict[card.abilitySeries];
                currentList.Add(card);
                availableAbilitiesDict[card.abilitySeries] = currentList;
            }
            else
            {
                currentList.Add(card);
                availableAbilitiesDict[card.abilitySeries] = currentList;
            }
        }

        foreach(var item in availableAbilitiesDict)
        {
            List<MacroAbilitySO> currentList = SortingByLevel(item.Value);

            sortingDict[item.Key] = currentList;

            //foreach(var a in currentList)
            //{
            //    Debug.Log(a.abilitySeries + " = " + a.level);
            //}
        }

        availableAbilitiesDict = sortingDict;
        sortingDict = null;

        abilitiesCount = CountAbilities();

        abilitiesPlacing = GlobalStorage.instance.playerMilitaryWindow.gameObject.GetComponentInChildren<AbilitiesPlacing>();
        abilitiesPlacing.Init(availableAbilitiesDict);
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

        foreach(var item in availableAbilitiesDict)
        {
            abilitiesList.Add(item.Key);
        }

        for(int i = 0; i < countOfVariants; i++)
        {
            if(abilitiesList.Count == 0) break;
 
            int randomIndex = Random.Range(0, abilitiesList.Count);
            PlayersStats randomAbility = abilitiesList[randomIndex];
            newList.Add(availableAbilitiesDict[randomAbility][0]);
            abilitiesList.Remove(randomAbility);

            //Debug.Log("Take " + randomAbility + " level " + availableDict[randomAbility][0].level);
        }

        return newList;
    }

    public void ApplyAbility(MacroAbilitySO ability)
    {
        if(availableAbilitiesDict.ContainsKey(ability.abilitySeries))
        {
            foreach(var skill in availableAbilitiesDict[ability.abilitySeries])
            {
                if(skill.level == ability.level)
                {
                    MoveToOpenedDict(skill);
                    availableAbilitiesDict[ability.abilitySeries].Remove(skill);

                    if(availableAbilitiesDict[ability.abilitySeries].Count == 0)
                        availableAbilitiesDict.Remove(ability.abilitySeries);

                    break;
                }
            }
        }
        else
        {
            Debug.Log("ERROR: We don't have this ability serie");
            return;
        }

        abilitiesPlacing.MarkSkill(ability);
        CountAbilities();
    }

    private void MoveToOpenedDict(MacroAbilitySO ability)
    {
        if(openedAbilitiesDict.ContainsKey(ability.abilitySeries) == false)
            openedAbilitiesDict[ability.abilitySeries] = new List<MacroAbilitySO>();

        openedAbilitiesDict[ability.abilitySeries].Add(ability);
    }

    public bool CheckSkill(MacroAbilitySO ability)
    {
        bool result = false;

        if(openedAbilitiesDict.ContainsKey(ability.abilitySeries))
        {
            List<MacroAbilitySO> checkList = openedAbilitiesDict[ability.abilitySeries];

            for(int i = 0; i < checkList.Count; i++)
            {
                if(checkList[i].level == ability.level)                 
                { 
                    result = true;
                    break;
                }
            }
        }        

        return result;
    }

    public bool CanIGetNewAbility()
    {
        return abilitiesCount > abilitiesOpened;
    }

    private int CountAbilities()
    {
        abilitiesLeft = 0;
        abilitiesOpened = 0;


        foreach(var serie in availableAbilitiesDict)
        {
            for(int i = 0; i < serie.Value.Count; i++)
            {
                abilitiesLeft++;
            }
        }

        foreach(var serie in openedAbilitiesDict)
        {
            for(int i = 0; i < serie.Value.Count; i++)
            {
                abilitiesOpened++;
            }
        }

        //Debug.Log("Left: " + abilitiesLeft + "; Opened: " + abilitiesOpened);

        return abilitiesLeft + abilitiesOpened;
    }

}
