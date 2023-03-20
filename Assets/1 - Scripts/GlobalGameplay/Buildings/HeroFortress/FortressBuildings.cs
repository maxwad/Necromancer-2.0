using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class FortressBuildings : MonoBehaviour
{
    private GMInterface gmInterface;
    private ResourceBuilding mint;
    private GameObject fortressGO;

    [SerializeField] private TMP_Text fortressLevelText;
    private int fortressLevel = 0;

    [SerializeField] private List<GameObject> buildingsGOList;
    private Dictionary<CastleBuildings, FBuilding> buildingsComponentDict = new Dictionary<CastleBuildings, FBuilding>();

    public Dictionary<CastleBuildings, int> buildingsLevels = new Dictionary<CastleBuildings, int>();
    public Dictionary<CastleBuildings, FBuilding> allReadyBuildings = new Dictionary<CastleBuildings, FBuilding>();

    public List<FortressUpgradeSO> upgradesList;
    public Dictionary<CastleBuildings, List<FortressUpgradeSO>> upgradesDict = new Dictionary<CastleBuildings, List<FortressUpgradeSO>>();

    public Dictionary<CastleBuildingsBonuses, float> buildingsBonuses = new Dictionary<CastleBuildingsBonuses, float>();
   
    [Space]
    [Header("Descriptions")]
    [SerializeField] private GameObject buildingDescription;
    private CanvasGroup buildingCanvas;
    [SerializeField] private List<GameObject> descriptionLevels;
    [SerializeField] private TMP_Text descriptionCaption;
    [SerializeField] private TMP_Text descriptionText;

    [Space]
    [Header("BuildingProgress")]
    private int maxCountOfProgress = 3;
    private Dictionary<CastleBuildings, ConstructionTime> buildingsInProgress = new Dictionary<CastleBuildings, ConstructionTime>();
    [SerializeField] private TMP_Text countOfProgressText;
    [SerializeField] private Image progressBG;
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color warningColor;

    [Space]
    [Header("Params")]
    public int maxLevel = 3;
    public int destroyBuildings = 3;

    private void Awake()
    {
        foreach(var item in buildingsGOList)
        {
            FBuilding buildingComponent = item.GetComponent<FBuilding>();
            buildingsComponentDict.Add(buildingComponent.building, buildingComponent);
        }

        foreach(CastleBuildings item in Enum.GetValues(typeof(CastleBuildings)))
            buildingsLevels.Add(item, 0);

        foreach(CastleBuildingsBonuses item in Enum.GetValues(typeof(CastleBuildingsBonuses)))
            buildingsBonuses.Add(item, 0);

        foreach(CastleBuildings item in Enum.GetValues(typeof(CastleBuildings)))
        {
            List<FortressUpgradeSO> upgrades = new List<FortressUpgradeSO>();
            for(int i = 0; i < upgradesList.Count; i++)
            {
                if(upgradesList[i].building == item)
                {
                    upgrades.Add(upgradesList[i]);
                }
            }

            upgradesDict.Add(item, upgrades);
        }

        fortressGO = GlobalStorage.instance.fortressGO;
        mint = fortressGO.GetComponent<ResourceBuilding>();
        buildingCanvas = buildingDescription.GetComponent<CanvasGroup>();

        UpgradeFortressLevel();
    }

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        UpdateBuildProgressUI();
    }

    //public void Test()
    //{
    //    foreach(var bonus in buildingsBonuses)
    //    {
    //        Debug.Log(bonus.Key + " = " + bonus.Value);
    //    }
    //}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            DestroyBuildings();
        }
    }

    #region UPGRADE ACTIONS

    public void ShowAllBuildings(bool showMode)
    {
        foreach(var building in buildingsGOList)
        {
            building.SetActive(showMode);
        }
    }

    public void UpgradeBuilding(CastleBuildings building, bool levelUpMode)
    {
        if(buildingsLevels[building] == 0)
        {
            RegisterBuilding(building);
        }

        int deltaLevel = (levelUpMode == true) ? 1 : -1;
        buildingsLevels[building] += deltaLevel;

        //if(newLevel <= maxLevel && newLevel >= 0)
        //    buildingsLevels[building] = newLevel;

        UpgradeFortressLevel();
        if(buildingsLevels[building] == 0) 
            allReadyBuildings.Remove(building);

        buildingsInProgress.Remove(building);
        UpdateBuildProgressUI();
        SetBonus(building, buildingsComponentDict[building].buildingBonus);

        gmInterface.castlePart.UpdateCastleStatus();
    }

    public void UpgradeFortressLevel()
    {
        int level = 0;
        foreach(var item in buildingsLevels)
            level += item.Value;

        fortressLevel = level;

        fortressLevelText.text = level.ToString();
        mint.SetResourceMultiplier(fortressLevel);
    }

    public void DestroyBuildings()
    {
        for(int i = 0; i < destroyBuildings; i++)
        {
            List<CastleBuildings> buildings = new List<CastleBuildings>();

            foreach(var buildingItem in buildingsLevels)
            {
                if(buildingItem.Value > 0)
                {
                    buildings.Add(buildingItem.Key);
                }
            }

            if(buildings.Count == 0)
            {
                Debug.Log("GAMEOVER");
                break;
            }
            else
            {
                CastleBuildings randomBuilding = buildings[UnityEngine.Random.Range(0, buildings.Count)];
                FBuilding building = allReadyBuildings[randomBuilding];
                building.Downgrade();
            }
        }

        gmInterface.castlePart.UpdateCastleStatus();
    }

    public void RegisterBuilding(CastleBuildings building)
    {
        if(allReadyBuildings.ContainsKey(building) == false)
            allReadyBuildings.Add(building, buildingsComponentDict[building]);
    }

    #endregion

    #region GETTINGS

    public FortressUpgradeSO GetBuildingBonus(CastleBuildings building, int checkLevel = 0)
    {
        int level = (checkLevel == 0) ? 0 : checkLevel;
        return upgradesDict[building][level]; 
    }

    private void SetBonus(CastleBuildings building, CastleBuildingsBonuses buildingBonus)
    {
        float bonusAmount = 0;
        int level = buildingsLevels[building];

        if(level >= 1)
        {
            FortressUpgradeSO bonus = upgradesDict[building][level - 1];
            bonusAmount = (bonus.isInverted == true) ? -bonus.value : bonus.value;
            bonusAmount = (bonus.percentValueType == true) ? bonusAmount * 0.01f : bonusAmount;
        }

        buildingsBonuses[buildingBonus] = bonusAmount;
        //Test();
    }

    public float GetBonusAmount(CastleBuildingsBonuses buildingBonus)
    {
        return buildingsBonuses[buildingBonus];
    }

    public List<Cost> GetBuildingCost(CastleBuildings building)
    {
        if(GetBuildingsLevel(building) >= GetMaxLevel()) return null;

        float discount = GetBonusAmount(CastleBuildingsBonuses.BuildingDiscount);

        List<Cost> defaultCosts;

        int level = buildingsLevels[building];
        defaultCosts = upgradesDict[building][level].cost;

        List<Cost> costs = new List<Cost>();

        for(int i = 0; i < defaultCosts.Count; i++)
        {
            Cost costItem = new Cost();
            costItem.type = defaultCosts[i].type;
            costItem.amount = Mathf.Round(defaultCosts[i].amount + defaultCosts[i].amount * discount) * (level + 1);
            costs.Add(costItem);
        }

        return costs;
    }

    public int GetMaxLevel()
    {
        return maxLevel;
    }

    public int GetBuildingsLevel(CastleBuildings building)
    {
        return buildingsLevels[building];
    }

    public int GetFortressLevel()
    {
        return fortressLevel;
    }

    public List<CastleBuildings> GetMilitary()
    {
        List<CastleBuildings> buildings = new List<CastleBuildings>();

        foreach(var building in allReadyBuildings)
        {
            if(building.Value.isMilitarySource == true)
            {
                if(buildingsLevels[building.Key] != 0)
                {
                    buildings.Add(building.Key);
                }
            }
        }
        return buildings;
    }

    public bool GetConstructionStatus(CastleBuildings building)
    {
        return buildingsInProgress.ContainsKey(building);
    }

    public CastleDataForUI GetDataForUI()
    {
        CastleDataForUI newData = new CastleDataForUI();
        newData.level = fortressLevel;

        newData.canIBuild = false;
        if(buildingsInProgress.Count < maxCountOfProgress)
        {
            newData.canIBuild = (CheckBuildingsStatus() > 0) ? true : false;
        }

        List<ConstractionData> constractions = new List<ConstractionData>();

        foreach(var building in buildingsInProgress)
        {
            ConstractionData constractionData = new ConstractionData();
            constractionData.constractionName = buildingsComponentDict[building.Key].buildingName;
            constractionData.icon = upgradesDict[building.Key][0].activeIcon;
            constractionData.daysLeft = building.Value.daysLeft;

            constractions.Add(constractionData);
        }

        newData.constractions = constractions;

        return newData;
    }

    public int GetRequiredLevel(CastleBuildings building)
    { 
        return upgradesDict[building][buildingsLevels[building]].fortressLevel;
    }

    public bool GetSiegeStatus()
    {
        return mint.CheckSiegeStatus();
    }

    #endregion

    #region BUILDING PROCESS

    public bool CheckNeededLevel(CastleBuildings building)
    {
        int neededLevel = upgradesDict[building][buildingsLevels[building]].fortressLevel;

        return fortressLevel >= neededLevel;
    }

    public int CheckBuildingsStatus()
    {
        int quantity = 0;

        foreach(var building in buildingsComponentDict)
        {
            if(buildingsLevels[building.Key] < maxLevel)
            {
                int neededLevel = upgradesDict[building.Key][buildingsLevels[building.Key]].fortressLevel;
                if(fortressLevel >= neededLevel)
                {
                    building.Value.UpdateStatus(true);
                    quantity++;
                }
                else
                {
                    building.Value.UpdateStatus(false);
                }
            }
        }

        return quantity;
    }

    public void UpdateBuildingsStatus()
    {        
        foreach(var building in buildingsComponentDict)
        {

            building.Value.CheckRequirements();            
        }
    }

    public bool CanIBuild()
    {
        return buildingsInProgress.Count < maxCountOfProgress;
    }

    public ConstructionTime StartBuildingBuilding(CastleBuildings building)
    {
        ConstructionTime constructionTime = new ConstructionTime();

        float discountDays;
        int originalDays = upgradesDict[building][buildingsLevels[building]].constructuionTime;
        constructionTime.originalTerm = originalDays;

        if(buildingsLevels[CastleBuildings.Manufactory] == 0)
        {
            discountDays = 0f;
            maxCountOfProgress = 3;
        }
        else
        {
            discountDays = buildingsBonuses[CastleBuildingsBonuses.BuildingTime];
            maxCountOfProgress = 3;
            //maxCountOfProgress = buildingsLevels[CastleBuildings.Manufactory];
        }
        constructionTime.term = originalDays + Mathf.FloorToInt(originalDays * discountDays);
        constructionTime.daysLeft = constructionTime.term;
        buildingsInProgress.Add(building, constructionTime);
        UpdateBuildProgressUI();

        return constructionTime;
    }

    #endregion

    #region UI

    private void NewDay()
    {
        if(mint.CheckSiegeStatus() == true) return;

        List<CastleBuildings> tempList = new List<CastleBuildings>(buildingsInProgress.Keys);
        foreach(var item in tempList)
        {
            buildingsInProgress[item].daysLeft--;
            buildingsComponentDict[item].UpdateBuildingProcess(buildingsInProgress[item]);
        }

        gmInterface.castlePart.UpdateCastleStatus();
    }

    private void UpdateBuildProgressUI()
    {
        if(buildingsInProgress.Count == 0)
            progressBG.color = emptyColor;
        else if(buildingsInProgress.Count == maxCountOfProgress)
            progressBG.color = warningColor;
        else
            progressBG.color = normalColor;

        countOfProgressText.text = buildingsInProgress.Count + "/" + maxCountOfProgress;
    }

    public void ShowDescription(CastleBuildings building)
    {
        descriptionCaption.text = upgradesDict[building][0].buildingName;
        descriptionText.text = upgradesDict[building][0].buildingDescription;

        int currentLevel = buildingsLevels[building];
        for(int i = 0; i < descriptionLevels.Count; i++)
        {
            FortressUpgradeSO bonus = GetBuildingBonus(building, i);

            descriptionLevels[i].transform.GetChild(0).gameObject.SetActive((i + 1 == currentLevel));

            string levelDescription = bonus.description;
            levelDescription = levelDescription.Replace("$V", bonus.value.ToString());
            TMP_Text[] texts = descriptionLevels[i].GetComponentsInChildren<TMP_Text>();
            texts[texts.Length - 1].text = levelDescription;
        }

        buildingDescription.SetActive(true);
        Fading.instance.FadeWhilePause(true, buildingCanvas);
    }

    public void CloseDescription()
    {
        buildingDescription.SetActive(false);
    }

    public void CloseAnotherConfirm()
    {
        foreach(var building in buildingsComponentDict)
        {
            building.Value.CloseConfirm();
        }
    }

    public void BuildStartBuilding(CastleBuildings building)
    {
        UpgradeBuilding(building, true);
    }

    #endregion

    #region SAVE/LOAD

    public HFBuildingsSD Save()
    {
        HFBuildingsSD saveData = new HFBuildingsSD();

        foreach(var building in buildingsLevels)
        {
            if(building.Value != 0)
            {
                saveData.buildedBuildings.Add(building.Key);
                saveData.buildedBuildingsLevels.Add(building.Value);                
            }
        }

        foreach(var building in buildingsInProgress)
        {
            saveData.buildingsInProgress.Add(building.Key);
            saveData.constructionsTimes.Add(building.Value);            
        }

        foreach(var building in allReadyBuildings)
            saveData.specialBuildingsSD.Add(building.Value.SaveData());

        return saveData;
    }

    public void Load(HFBuildingsSD saveData)
    {
        for(int i = 0; i < saveData.buildedBuildings.Count; i++)
        {
            for(int j = 0; j < saveData.buildedBuildingsLevels[i]; j++)
                BuildStartBuilding(saveData.buildedBuildings[i]);
        }

        for(int i = 0; i < saveData.buildingsInProgress.Count; i++)
        {
            CastleBuildings building = saveData.buildingsInProgress[i];
            StartBuildingBuilding(building);
            buildingsInProgress[building] = saveData.constructionsTimes[i];
            buildingsComponentDict[building].UpdateBuildingProcess(saveData.constructionsTimes[i]);
        }

        foreach(var building in allReadyBuildings)
            building.Value.Load(saveData.specialBuildingsSD);

        gmInterface.castlePart.UpdateCastleStatus();
    }

    #endregion


    private void OnEnable()
    {
        EventManager.NewMove += NewDay;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= NewDay;
    }
}