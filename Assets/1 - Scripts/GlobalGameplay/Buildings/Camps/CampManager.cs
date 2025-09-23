using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using Zenject;

public class CampManager : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private RewardManager rewardManager;
    private PlayerStats playerStats;
    private CampUI campUI;

    private Dictionary<ResourceType, Sprite> resourcesIcons;
    private Dictionary<GameObject, bool> campsRewardsDict = new Dictionary<GameObject, bool>();

    [Header("Parameters")]
    [SerializeField] private int cellsAmount = 25;
    [SerializeField] private int rewardsAmount = 10;
    [SerializeField] private int attempts = 10;
    [SerializeField] private int helps = 4;
    [SerializeField] private int runeDrawnings = 3;
    [SerializeField] private int extraAmount = 5;

    private float playerSearchLevel = 0;

    [SerializeField] private List<CampBonus> bonusesData;
    [SerializeField] private List<ResourceType> resources;

    [Inject]
    public void Construct(
        ResourcesManager resourcesManager,
        RewardManager rewardManager,
        PlayerStats playerStats,
        CampUI campUI
        )
    {
        this.resourcesManager = resourcesManager;
        this.rewardManager = rewardManager;
        this.playerStats = playerStats;
        this.campUI = campUI;
    }

    private void Start()
    {
        resourcesIcons = resourcesManager.GetAllResourcesIcons();
    }

    public void Register(GameObject building) => campsRewardsDict.Add(building, true);

    #region GETTINGS

    public bool GetCampStatus(GameObject building)
    {
        return (campsRewardsDict.ContainsKey(building) == true) ? campsRewardsDict[building] : false;
    }

    public CampGameParameters GetStartParameters()
    {
        playerSearchLevel = playerStats.GetCurrentParameter(PlayersStats.AshSpecialist); 
        CampGameParameters gameParameters = new CampGameParameters();

        gameParameters.cellsAmount = cellsAmount;
        gameParameters.rewardsAmount = rewardsAmount + ((playerSearchLevel > 0) ? extraAmount : 0);
        gameParameters.attempts = attempts + ((playerSearchLevel > 1) ? extraAmount : 0); ;
        gameParameters.helps = helps;
        gameParameters.combination = GetBonfireCombination(gameParameters.rewardsAmount);

        return gameParameters;
    }

    public List<CampBonus> GetBonfireCombination(int bonusAmount)
    {
        List<CampReward> rewardNamesList = new List<CampReward>();
        List<int> variants = new List<int>();

        for(int i = 0; i < cellsAmount; i++)
        {
            rewardNamesList.Add(CampReward.Nothing);
            variants.Add(i);
        }

        FillRandomElementBy(CampReward.AbilityPoint);
        FillRandomElementBy(CampReward.Health);
        FillRandomElementBy(CampReward.Mana);
        FillRandomElementBy(CampReward.Help);

        for(int i = 0; i < runeDrawnings; i++)
        {
            FillRandomElementBy(CampReward.RuneDrawing);
        }

        int counter = bonusAmount;
        for(int i = 0; i < counter; i++)
        {
            FillRandomElementBy(CampReward.Resource);
        }

        return CreateBonusList(); ;

        void FillRandomElementBy(CampReward reward)
        {
            int randomIndex = variants[UnityEngine.Random.Range(0, variants.Count)];            
            rewardNamesList[randomIndex] = reward;
            variants.Remove(randomIndex);

            bonusAmount--;
        }

        List<CampBonus> CreateBonusList()
        {
            List<CampBonus> list = new List<CampBonus>();

            for(int i = 0; i < rewardNamesList.Count; i++)
            {
                CampBonus currentBonus = new CampBonus();

                if(rewardNamesList[i] != CampReward.Resource)
                {
                    foreach(var bonus in bonusesData)
                    {
                        if(rewardNamesList[i] == bonus.reward)
                        {
                            currentBonus = bonus;
                            break;
                        }
                    }
                }
                else
                {
                    ResourceType resource = resources[UnityEngine.Random.Range(0, resources.Count)];

                    currentBonus.reward = rewardNamesList[i];
                    currentBonus.resource = resource;
                    currentBonus.name = resource.ToString();
                    currentBonus.icon = resourcesIcons[resource];
                    currentBonus.amount = (int)rewardManager.GetHeapReward(resource).resourcesQuantity[0];
                }

                list.Add(currentBonus);
            }

            return list;
        }
    }

    public int GetHelpsCount() => helps;

    public void CloseCamp(GameObject building) => campsRewardsDict[building] = false;

    #endregion

    public void ChangeHelpPointsAmount(int delta) => helps += delta;

    public List<Vector3> GetSaveData()
    {
        List<Vector3> emptyCamps = new List<Vector3>();

        foreach(var camp in campsRewardsDict)
        {
            if(camp.Value == false)
                emptyCamps.Add(camp.Key.transform.position);        
        }

        return emptyCamps;
    }

    public void LoadCamps(List<Vector3> emptyCamps)
    {
        foreach(var campWith in emptyCamps)
        {
            foreach(var camp in campsRewardsDict.Keys.ToList())
            {
                if(campWith == camp.transform.position)
                {
                    campsRewardsDict[camp] = false;
                    campUI.SetCampEmpty(camp);
                    break;
                }
            }
        }
    }
}
