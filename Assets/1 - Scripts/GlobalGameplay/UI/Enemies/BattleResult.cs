using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;
using System;

public class BattleResult : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;

    [SerializeField] private GameObject slotPrefab;

    [Header("Level Info")]
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentExpText;
    [SerializeField] private TMP_Text boundExpText;
    [SerializeField] private Image levelScale;
    [SerializeField] private Image fakeLevelScale;
    private Coroutine levelGrowningCoroutine;
    private float levelFillStep = 0.01f;
    private float levelFillMultiplier = 0.5f;

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
    private List<InfotipTrigger> lossesItemSquadtipList = new List<InfotipTrigger>();
    [SerializeField] private GameObject retreatIcon;
    [SerializeField] private GameObject defeatIcon;

    private bool isDeadRegistrating = false;
    private Dictionary<UnitsTypes, int> lostUnitsDict = new Dictionary<UnitsTypes, int>();

    private List<Unit> actualUnits = new List<Unit>();

    [Space]
    [Header("Container")]
    public float fullHeight = 650f;
    public float losesHeight = 500f;
    public float minHeigth = 350f;
    private RectTransform rectContainer;

    [SerializeField] private CanvasGroup canvas;
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
    private MacroLevelUpManager macroManager;

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
            macroManager     = GlobalStorage.instance.macroLevelUpManager; 
            playersArmy      = GlobalStorage.instance.player.GetComponent<PlayersArmy>();

            CreateLists();
        }

        currentStatus = mode;
        currentPercentReward = percentOfReward / 100;
        currentEnemyArmy = currentArmy;
        currentEnemyGO = currentArmyGO;
        canvas.alpha = 0;

        UpdateActualArmy();


        BattleResultActions();

        RefactoringContainer();
        uiPanel.SetActive(true);
        Fading.instance.Fade(true, canvas, step, 10);
    }

    private void BattleResultActions()
    {
        ResetItems();

        if(currentStatus == 1)
        {
            enemyManager.DeleteArmy(currentEnemyGO, currentEnemyArmy);

            FillReward();
            FillLevelInfo();
        }

        if(currentStatus == 0) playersArmy.DefeatDamage();

        if(currentStatus == -1) playersArmy.EscapeDamage();

        FillLosses();
    }


    private void FillReward()
    {
        rewardBlock.SetActive(true);

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
    
    private void FillLevelInfo()
    {
        levelBlock.SetActive(true);
        float expReward = 0;
        LevelData currentData = macroManager.GetLevelData();

        levelText.text = currentData.level.ToString();
        currentExpText.text = currentData.currentExp.ToString();
        boundExpText.text = currentData.boundExp.ToString();
        levelScale.fillAmount = currentData.currentExp / currentData.boundExp;

        if(currentReward != null)
        {
            for(int i = 0; i < currentReward.resourcesList.Count; i++)
            {
                if(currentReward.resourcesList[i] == ResourceType.Exp)
                {
                    expReward = currentReward.resourcesQuantity[i];
                    break;
                }                
            }

            if(expReward == 0) return;

            fakeLevelScale.fillAmount = (currentData.currentExp + expReward) / currentData.boundExp;
            float goalFill = (currentData.currentExp + expReward) / currentData.boundExp;

            float difference = currentData.boundExp - (currentData.currentExp + expReward);

            float countStep = expReward / ((expReward / currentData.boundExp) / (levelFillStep * levelFillMultiplier));

            if(difference <= 0)
            {
                fakeLevelScale.fillAmount = goalFill;
                goalFill = 1;
                difference = Mathf.Abs(difference);
                countStep = (currentData.boundExp - currentData.currentExp) / (  (  (currentData.boundExp - currentData.currentExp) / currentData.boundExp)   /  (levelFillStep * levelFillMultiplier)  );
            }

            levelGrowningCoroutine = StartCoroutine(LevelGrowing(goalFill, difference, countStep));
        }
        else
        {
            return;
        }

        IEnumerator LevelGrowing(float goal, float delta, float step)
        {
            yield return new WaitForSecondsRealtime(2f);

            float currentCount = currentData.currentExp;
            float bufferCount;
            bool isNewLevel = false;

            WaitForSecondsRealtime pause = new WaitForSecondsRealtime(levelFillStep);           

            while(levelScale.fillAmount < goal)
            {
                levelScale.fillAmount += (levelFillStep * levelFillMultiplier);
                currentCount += step;
                bufferCount = Mathf.Round(currentCount);
                currentExpText.text = bufferCount.ToString();

                if(levelScale.fillAmount >= 1f)
                {
                    isNewLevel = true;
                    levelScale.fillAmount = 0;
                    currentCount = 0;

                    LevelData futureData = macroManager.GetFutureDataForBattleResult();

                    levelText.text = futureData.level.ToString();
                    currentExpText.text = "0";
                    boundExpText.text = futureData.boundExp.ToString();
                    goal = delta / futureData.boundExp;
                    step = delta / ((delta / futureData.boundExp) / (levelFillStep * levelFillMultiplier));
                    fakeLevelScale.fillAmount = goal;
                }

                if(levelScale.fillAmount > goal) levelScale.fillAmount = goal;
 
                yield return pause;
            }

            currentExpText.text = (isNewLevel == true) ? delta.ToString() : (currentData.currentExp + expReward).ToString();
        }
    }

    private void FillLosses()
    {
        if(lostUnitsDict.Count == 0) return;

        lossesBlock.SetActive(true);

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
    private void RegisterDeadUnit(UnitsTypes unitType)
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
        float height = 0;

        if(currentStatus == -1)
        {
            if(lostUnitsDict.Count == 0)
                retreatIcon.SetActive(true);
            
            height = minHeigth;
            currentCaption = retreatCaption;            
        }

        if(currentStatus == 0)
        {
            if(lostUnitsDict.Count == 0)
                defeatIcon.SetActive(true);

            height = minHeigth;
            currentCaption = defeatCaption;            
        }

        if(currentStatus == 1)
        {
            if(lostUnitsDict.Count == 0)
            {               
                height = losesHeight;
            }
            else
            {               
                height = fullHeight;
            }
            currentCaption = victoryCaption;
        }

        rectContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        caption.text = currentCaption;
    }

    private void ResetItems()
    {
        retreatIcon.SetActive(false);
        defeatIcon.SetActive(false);

        foreach(var item in rewardItemList)
            item.SetActive(false);

        foreach(var item in lossesItemList)
            item.SetActive(false);
    }

    private void UpdateActualArmy()
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
            InfotipTrigger squadtipTrigger = item.AddComponent<InfotipTrigger>();
            lossesItemSquadtipList.Add(squadtipTrigger);            
        }
    }

    public void StartCheckingUnitDeath(bool mode = false)
    {
        if(mode == false) isDeadRegistrating = true;
    }

    public void CloseCheckingUnitDeath()
    {
        // we need second function because we start reg automaticaly, but close - by command
        isDeadRegistrating = false;
    }

    #endregion

    //Button
    public void CloseWindow()
    {
        CloseCheckingUnitDeath();
        lostUnitsDict.Clear();
        currentEnemyArmy = null;
        if(levelGrowningCoroutine != null) StopCoroutine(levelGrowningCoroutine);

        if(currentStatus == 0) EventManager.OnDefeatEvent();
        if(currentStatus == 1) GetReward();

        rewardBlock.SetActive(false);
        levelBlock.SetActive(false);
        lossesBlock.SetActive(false);

        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }

    private void OnEnable()
    { 
        EventManager.SwitchPlayer += StartCheckingUnitDeath;
        EventManager.WeLostOneUnit += RegisterDeadUnit;
        EventManager.ResurrectUnit += DeleteDeadUnit;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= StartCheckingUnitDeath;
        EventManager.WeLostOneUnit -= RegisterDeadUnit;
        EventManager.ResurrectUnit -= DeleteDeadUnit;
    }
}
