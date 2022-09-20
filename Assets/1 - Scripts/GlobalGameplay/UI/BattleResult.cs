using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class BattleResult : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;

    [SerializeField] private GameObject slotPrefab;

    [Header("Reward slots")]
    [SerializeField] private GameObject rewardBlock;
    [SerializeField] private List<GameObject> rewardItemList;
    private List<Image> rewardItemImageList = new List<Image>();
    private List<TMP_Text> rewardItemTextList = new List<TMP_Text>();
    private List<TooltipTrigger> rewardItemTooltipList = new List<TooltipTrigger>();

    [Header("Losses slots")]
    [SerializeField] private GameObject lossesBlock;
    [SerializeField] private List<GameObject> lossesItemList;
    private List<Image> lossesItemImageList = new List<Image>();
    private List<TMP_Text> lossesItemTextList = new List<TMP_Text>();
    private List<TooltipTrigger> lossesItemTooltipList = new List<TooltipTrigger>();

    private bool isDeadRegistrating = false;
    private Dictionary<UnitsTypes, Sprite> allUnitsIconsDict = new Dictionary<UnitsTypes, Sprite>();
    private Dictionary<UnitsTypes, string> allUnitsNamesDict = new Dictionary<UnitsTypes, string>();
    private Dictionary<UnitsTypes, int> lostUnitsDict = new Dictionary<UnitsTypes, int>();

    private List<Unit> actualUnits = new List<Unit>();

    [Space]
    [Header("Container")]
    public float fullHeight;
    public float losesHeight;
    private RectTransform rectContainer;

    [SerializeField] private CanvasGroup canvas;
    private float currentAlfa = 0;
    private float step = 0.1f;

    public TMP_Text caption;
    private string currentCaption;
    private string retreatCaption = "Retreat!";
    private string defeatCaption = "Defeat!";
    private string victoryCaption = "Victory!";

    //0 - defeat, 1 - victory, -1 - stepback
    private int currentStatus;
    private Army currentEnemyArmy;
    private EnemyArmyOnTheMap currentEnemyGO;

    [Header("Managers")]
    private Reward currentReward;
    private RewardManager rewardManager;
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;
    private EnemyManager enemyManager;

    private PlayersArmy playersArmy;

    public void Init(int mode, EnemyArmyOnTheMap currentArmyGO, Army currentArmy)
    {
        GlobalStorage.instance.ModalWindowOpen(true);

        if(rectContainer == null)
        {
            rectContainer    = canvas.GetComponent<RectTransform>();
            rewardManager    = GlobalStorage.instance.rewardManager;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons   = resourcesManager.GetAllResourcesIcons();
            enemyManager     = GlobalStorage.instance.enemyManager;
            playersArmy      = GlobalStorage.instance.player.GetComponent<PlayersArmy>();

            CreateLists();
        }

        currentStatus = mode;
        currentEnemyArmy = currentArmy;
        currentEnemyGO = currentArmyGO;
        canvas.alpha = 0;

        CreateDicts();

        RefactoringContainer();

        BattleResultActions();

        uiPanel.SetActive(true);
        StartCoroutine(ShowUI());
    }

    private void BattleResultActions()
    {
        if(currentStatus == 1)
        {
            enemyManager.DeleteArmy(currentEnemyGO, currentEnemyArmy);
            FillReward();
        }

        if(currentStatus == 0) playersArmy.DefeatDamage();

        if(currentStatus == -1) playersArmy.EscapeDamage();

        FillLosses();
    }

    private IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(step * 10);

        WaitForSeconds delay = new WaitForSeconds(step * 0.25f);

        currentAlfa = 0;

        while(currentAlfa < 1)
        {
            currentAlfa += step * 2;
            canvas.alpha = currentAlfa;
            yield return delay;
        }
    }

    private void FillReward()
    {
        currentReward = rewardManager.GetBattleReward(currentEnemyArmy);

        for(int i = 0; i < currentReward.resourcesList.Count; i++)
        {
            rewardItemList[i].SetActive(true);
            rewardItemImageList[i].sprite = resourcesIcons[currentReward.resourcesList[i]];
            rewardItemTextList[i].text = currentReward.resourcesQuantity[i].ToString();
            rewardItemTooltipList[i].content = currentReward.resourcesList[i].ToString();
        }
    }

    private void FillLosses()
    {
        int counter = 0;

        foreach(var unit in lostUnitsDict)
        {
            lossesItemList[counter].SetActive(true);
            lossesItemImageList[counter].sprite = allUnitsIconsDict[unit.Key];
            lossesItemTextList[counter].text = unit.Value.ToString();
            lossesItemTooltipList[counter].content = allUnitsNamesDict[unit.Key];

            counter++;
        }
    }

    private void GetReward()
    {
        for(int i = 0; i < currentReward.resourcesList.Count; i++)
            EventManager.OnResourcePickedUpEvent(currentReward.resourcesList[i], currentReward.resourcesQuantity[i]);

        currentReward = null;
    }

    #region HELPERS
    private void RegisterDeadUnit(UnitsTypes unitType, int quantity)
    {
        if(isDeadRegistrating == true)
        {
            if(lostUnitsDict.ContainsKey(unitType) == true)
                lostUnitsDict[unitType]++;
            else
                lostUnitsDict.Add(unitType, 1);
        }
    }

    private void DeleteDeadUnit(UnitsTypes unitType)
    {
        if(isDeadRegistrating == true)
        {
            if(lostUnitsDict.ContainsKey(unitType) == true)
            {
                lostUnitsDict[unitType]--;

                if(lostUnitsDict[unitType] == 0) lostUnitsDict.Remove(unitType);
            }
        }
    }

    private void RefactoringContainer()
    {
        ResetItems();

        float height = 0;

        if(currentStatus == -1)
        {
            height = losesHeight;
            currentCaption = retreatCaption;
            rewardBlock.SetActive(false);
        }

        if(currentStatus == 0)
        {
            height = losesHeight;
            currentCaption = defeatCaption;
            rewardBlock.SetActive(false);
        }

        if(currentStatus == 1)
        {
            height = fullHeight;
            currentCaption = victoryCaption;
            rewardBlock.SetActive(true);
        }

        rectContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        caption.text = currentCaption;
    }

    private void ResetItems()
    {
        foreach(var item in rewardItemList)
            item.SetActive(false);

        foreach(var item in lossesItemList)
            item.SetActive(false);
    }

    private void CreateDicts()
    {
        actualUnits = GlobalStorage.instance.unitManager.GetActualArmy();

        allUnitsIconsDict.Clear();
        allUnitsNamesDict.Clear();

        foreach(var unit in actualUnits)
        {
            allUnitsIconsDict.Add(unit.unitType, unit.unitIcon);
            allUnitsNamesDict.Add(unit.unitType, unit.unitName);
        }
    }

    private void CreateLists()
    {
        foreach(var item in rewardItemList)
        {
            rewardItemImageList.Add(item.GetComponentInChildren<Image>());
            rewardItemTextList.Add(item.GetComponentInChildren<TMP_Text>());
            rewardItemTooltipList.Add(item.GetComponentInChildren<TooltipTrigger>());
        }

        foreach(var item in lossesItemList)
        {
            lossesItemImageList.Add(item.GetComponentInChildren<Image>());
            lossesItemTextList.Add(item.GetComponentInChildren<TMP_Text>());
            lossesItemTooltipList.Add(item.GetComponentInChildren<TooltipTrigger>());
        }
    }

    private void StartCheckingUnitDeath(bool mode)
    {
        if(mode == false) isDeadRegistrating = true;
    }

    #endregion

    public void CloseWindow()
    {        
        isDeadRegistrating = false;
        lostUnitsDict.Clear();
        currentEnemyArmy = null;

        if(currentStatus == 0) EventManager.OnDefeatEvent();
        if(currentStatus == 1) GetReward();

        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }

    private void OnEnable()
    { 
        EventManager.ChangePlayer += StartCheckingUnitDeath;
        EventManager.WeLostOneUnit += RegisterDeadUnit;
        EventManager.ResurrectUnit += DeleteDeadUnit;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer -= StartCheckingUnitDeath;
        EventManager.WeLostOneUnit -= RegisterDeadUnit;
        EventManager.ResurrectUnit -= DeleteDeadUnit;
    }
}
