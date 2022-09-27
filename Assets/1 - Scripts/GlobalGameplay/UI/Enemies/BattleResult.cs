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
    private List<SquadtipTrigger> lossesItemSquadtipList = new List<SquadtipTrigger>();

    private bool isDeadRegistrating = false;
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
    private float currentPercentReward;
    private Army currentEnemyArmy;
    private EnemyArmyOnTheMap currentEnemyGO;

    [Header("Managers")]
    private Reward currentReward;
    private RewardManager rewardManager;
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;
    private EnemyManager enemyManager;

    private PlayersArmy playersArmy;

    public void Init(int mode, float percentOfReward, EnemyArmyOnTheMap currentArmyGO, Army currentArmy)
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
        currentPercentReward = percentOfReward / 100;
        currentEnemyArmy = currentArmy;
        currentEnemyGO = currentArmyGO;
        canvas.alpha = 0;

        UpdateActualarmy();

        RefactoringContainer();

        BattleResultActions();

        uiPanel.SetActive(true);
        //StartCoroutine(ShowUI());
        Fading.instance.Fade(true, canvas, step, step * 5);
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

    //private IEnumerator ShowUI()
    //{
    //    yield return new WaitForSeconds(step * 10);

    //    WaitForSeconds delay = new WaitForSeconds(step * 0.25f);

    //    currentAlfa = 0;

    //    while(currentAlfa < 1)
    //    {
    //        currentAlfa += step * 2;
    //        canvas.alpha = currentAlfa;
    //        yield return delay;
    //    }
    //}

    private void FillReward()
    {
        currentReward = rewardManager.GetBattleReward(currentEnemyArmy);        

        for(int i = 0; i < currentReward.resourcesList.Count; i++)
        {
            if(currentPercentReward != 1f)
            {
                currentReward.resourcesQuantity[i] = Mathf.Ceil(currentReward.resourcesQuantity[i] * currentPercentReward);
            }

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
            Unit neededUnit = null;

            foreach(var item in actualUnits)
            {
                if(item.unitType == unit.Key)
                {
                    neededUnit = item;
                    break;
                }
            }

            lossesItemList[counter].SetActive(true);
            lossesItemImageList[counter].sprite = neededUnit.unitIcon;
            lossesItemTextList[counter].text = unit.Value.ToString();
            lossesItemSquadtipList[counter].SetUnit(neededUnit);

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

    private void UpdateActualarmy()
    {
        actualUnits = GlobalStorage.instance.unitManager.GetActualArmy();
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
            Destroy(item.GetComponentInChildren<TooltipTrigger>());
            SquadtipTrigger squadtipTrigger = item.AddComponent<SquadtipTrigger>();
            lossesItemSquadtipList.Add(squadtipTrigger);            
        }
    }

    public void StartCheckingUnitDeath(bool mode = false)
    {
        if(mode == false) isDeadRegistrating = true;
    }

    public void CloseCheckingUnitDeath()
    {
        // we need second function becouse we start reg automaticaly, but close - by command
        isDeadRegistrating = false;
    }

    #endregion

    public void CloseWindow()
    {
        CloseCheckingUnitDeath();
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
