using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;
using System;

public class BattleUIManager : MonoBehaviour
{
    public Canvas uiCanvas;
    private PlayerStats playerStats;
    private MacroLevelUpManager levelManager;
    private ResourcesManager resourcesManager;
    private BattleBoostManager boostManager;
    private ObjectsPoolManager objectsPool;
    private bool isBattleOver = false;

    #region VALUES

    [Header("Left Column Exp")]
    //[SerializeField] private RectTransform currentScaleValue;
    [SerializeField] private Image leftExpScale;
    [SerializeField] private Color levelUpColor;
    [SerializeField] private Color normalLevelColor;

    [Header("Rigth Column Exp")]
    [SerializeField] private GameObject tempLevelGO;
    [SerializeField] private RectTransform currentTempLevelWrapper;
    //[SerializeField] private RectTransform currentScaleTempLevelValue;
    [SerializeField] private Image rightExpScale;
    private float heigthOneLevel;
    private float currentMaxLevel;
    private float currenLevel;
    [SerializeField] private Color activeTempLevelColor;
    [SerializeField] private Color inactiveTempLevelColor;
    private List<Image> levelList = new List<Image>();

    [Header("Exp Effects")]
    private float blinkTime = 0.005f;

    [Header("Infirmary")]
    [SerializeField] private Image infirmaryScale;
    [SerializeField] private TMP_Text infirmaryInfo;
    private float currentMaxInfirmaryCount;
    private float currentInfirmaryCount;
    [SerializeField] private Color infirmaryUpColor;
    [SerializeField] private Color infirmaryDownColor;
    [SerializeField] private Color normalInfirmaryColor;

    [Header("Mana")]
    //[SerializeField] private RectTransform manaValue;
    [SerializeField] private TMP_Text manaInfo;
    [SerializeField] private Image manaScale;
    private float currentMaxManaCount;
    private float currentManaCount;
    [SerializeField] private Color manaUpColor;
    [SerializeField] private Color manaDownColor;
    [SerializeField] private Color normalManaColor;

    [Header("Health")]
    //[SerializeField] private RectTransform healthValue;
    [SerializeField] private TMP_Text healthInfo;
    [SerializeField] private Image healthScale;
    private float currentMaxHealthCount;
    private float currentHealthCount;
    [SerializeField] private Color healthUpColor;
    [SerializeField] private Color healthDownColor;
    [SerializeField] private Color normalHealthColor;

    [Header("Gold")]
    [SerializeField] private TMP_Text goldInfo;
    private float startGold;

    [Header("Spells")]
    [SerializeField] private Button buttonSpell;
    private List<SpellStat> currentSpells = new List<SpellStat>();
    [SerializeField] private GameObject spellButtonContainer;
    private List<Button> currentSpellsButtons = new List<Button>();
    private int countOfActiveSpells = 6;
    private int currentSpellIndex = -1;

    [Header("Enemy")]
    [SerializeField] private RectTransform bossWrapper;
    [SerializeField] private Image enemiesValue;
    [SerializeField] private Image spawnValue;
    [SerializeField] private TMP_Text enemiesInfo;
    private float maxEnemiesCount = 0;
    private float currentEnemiesCount = 0;
    private float bossIndex = 0;
    [SerializeField] private GameObject bossMark;
    private List<GameObject> bossMarkList = new List<GameObject>();
    private BossData[] bosses;

    [Header("End of Battle")]
    [SerializeField] private GameObject leaveBlock;
    private bool isLeaveBlockOpened = false;
    [SerializeField] private GameObject victoryBlock;
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private GameObject defeatBlock;
    [SerializeField] private CanvasGroup defeatCanvasGroup;

    #endregion

    [Header("Boost")]
    [SerializeField] private RectTransform boostWrapper;
    [SerializeField] private RectTransform tempBoostWrapper;
    [SerializeField] private GameObject boostItem;
    private Dictionary<BoostType, float> boostDict = new Dictionary<BoostType, float>();
    private List<GameObject> boostItemList = new List<GameObject>();


    private void Start()
    {
        objectsPool = GlobalStorage.instance.objectsPoolManager;
        playerStats = GlobalStorage.instance.playerStats;
        levelManager = GlobalStorage.instance.macroLevelUpManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        boostManager = GlobalStorage.instance.unitBoostManager;
    }

    #region Helpers

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if(GlobalStorage.instance.isGlobalMode == false) OpenLeaveBlock(!isLeaveBlockOpened);
        }

        if(Input.GetKeyDown(KeyCode.End))
        {
            if(GlobalStorage.instance.isGlobalMode == false) Victory();
        }
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            if(GlobalStorage.instance.isGlobalMode == false) Defeat();
        }
        if(Input.GetKeyDown(KeyCode.Home))
        {
            if(GlobalStorage.instance.isGlobalMode == false) LeaveTheBattle();
        }

        if(GlobalStorage.instance.isGlobalMode == false) Spelling();
    }

    private void Spelling()
    {
        switch(Input.inputString)
        {
            case "1":
                currentSpellIndex = 0;
                break;
            case "2":
                currentSpellIndex = 1;
                break;
            case "3":
                currentSpellIndex = 2;
                break;
            case "4":
                currentSpellIndex = 3;
                break;
            case "5":
                currentSpellIndex = 4;
                break;
            case "6":
                currentSpellIndex = 5;
                break;
            case "7":
                currentSpellIndex = 6;
                break;
            case "8":
                currentSpellIndex = 7;
                break;
            case "9":
                currentSpellIndex = 8;
                break;
            case "0":
                currentSpellIndex = 9;
                break;

            default:
                currentSpellIndex = -1;
                break;
        }

        if(currentSpellIndex != -1 && currentHealthCount != 0) 
            currentSpellsButtons[currentSpellIndex].GetComponent<SpellButtonController>().ActivateSpell();
    }

    public void Inizialize(bool mode)
    {   
        uiCanvas.gameObject.SetActive(!mode);
        isBattleOver = false;

        if(mode == false) ResetCanvas();
    }
    public void UpgradeResourceUI(ResourceType type, float value)
    {
        GetMaxManaHealth();

        if(type == ResourceType.Health) FillHealth(value);
        if(type == ResourceType.Mana) FillMana(value);
        if(type == ResourceType.Gold) FillGold(value);
    }

    private void GetMaxManaHealth()
    {
        currentMaxManaCount = resourcesManager.GetMaxMana();
        currentMaxHealthCount = resourcesManager.GetMaxHealth();
    }


    private void ResetCanvas()
    {
        FillRigthTempLevelScale();

        FillInfirmary();

        GetMaxManaHealth();
        FillHealth();
        FillMana();
        FillGold();

        FillSpells(-1);

        FillDeadEnemiesBar(null);

        FillPlayerBoost();
    }

    private void Blink(Image panel, Color effectColor, Color normalColor, float divider = 5)
    {
        panel.color = effectColor;
        StartCoroutine(ColorBack(panel, normalColor, divider));
    }

    private IEnumerator ColorBack(Image panel, Color normalColor, float divider)
    {
        //the bigger divider the slower animation

        float time = 0;

        while(panel.color != normalColor)
        {
            time += Time.deltaTime;
            panel.color = Color.Lerp(panel.color, normalColor, time / divider);
            yield return new WaitForSeconds(blinkTime);
        }
    }

    public void OpenLeaveBlock(bool mode)
    {
        if(isBattleOver == true) return;

        isLeaveBlockOpened = mode;
        GlobalStorage.instance.ModalWindowOpen(mode);
        MenuManager.instance.MiniPause(mode);
        leaveBlock.SetActive(mode);
    }

    public void LeaveTheBattle()
    {
        OpenLeaveBlock(false);
        GlobalStorage.instance.battleManager.FinishTheBattle(false, -1);
    }

    public void ShowVictoryBlock()
    {
        isBattleOver = true;
        EventManager.OnVictoryEvent();
        StartCoroutine(VictoryBlockAppearing());

        IEnumerator VictoryBlockAppearing()
        {
            victoryBlock.SetActive(true);
            victoryCanvasGroup.alpha = 0;

            while(victoryCanvasGroup.alpha < 1)
            {
                victoryCanvasGroup.alpha += 0.01f;
                yield return null;
            }
        }
    }

    public void Victory()
    {
        victoryBlock.SetActive(false);
        GlobalStorage.instance.battleManager.FinishTheBattle(false, 1);
    }

    public void ShowDefeatBlock()
    {
        isBattleOver = true;
        StartCoroutine(DefeatBlockAppearing());

        IEnumerator DefeatBlockAppearing()
        {
            defeatBlock.SetActive(true);
            defeatCanvasGroup.alpha = 0;

            while(defeatCanvasGroup.alpha < 1)
            {
                defeatCanvasGroup.alpha += 0.01f;
                yield return null;
            }
        }
    }

    public void Defeat()
    {
        defeatBlock.SetActive(false);
        GlobalStorage.instance.battleManager.FinishTheBattle(false, 0);
    }

    #endregion

    #region BOOST

    private void FillPlayerBoost()
    {
        foreach(var item in boostItemList)
            item.SetActive(false);
        
        boostItemList.Clear();
        boostDict.Clear();

        Dictionary<BoostType, List<Boost>> boosts = boostManager.GetBoostDict();
        foreach(var item in boosts)
        {
            List<Boost> tempList = item.Value;

            foreach(var boost in tempList)
            {
                if(boost.effect == BoostEffect.PlayerBattle)
                {
                    if(boostDict.ContainsKey(item.Key))
                        boostDict[item.Key] += boost.value;
                    else
                        boostDict[item.Key] = boost.value;
                }
                else
                {
                    break;
                }
            }
        }

        foreach(var boost in boostDict)
        {
            CreateEffect(boostWrapper, boost.Key, boost.Value, true);
        }
    }

    private void UpgradeBoostes(BoostType boost, float boostValue)
    {
        FillPlayerBoost();
    }

    private void CreateEffect(Transform parent, BoostType type, float boost, bool constMode = true)
    {
        GameObject boostItemUI = objectsPool.GetObject(ObjectPool.BattleEffect);
        boostItemUI.transform.SetParent(parent, false);
        boostItemUI.SetActive(true);
        if(parent == boostWrapper) boostItemList.Add(boostItemUI);

        RunesType runeType = BoostConverter.instance.BoostTypeToRune(type);
        float value = boost;
        BoostInBattleUI item = boostItemUI.GetComponent<BoostInBattleUI>();
        item.Init(runeType, value, constMode);
    }

    private void ShowBoostEffect(BoostType type, float value)
    {
        CreateEffect(tempBoostWrapper, type, value, false);
    }

    #endregion

    #region TempExp
    private void FillRigthTempLevelScale()
    {
        levelList.Clear();

        foreach(Transform child in currentTempLevelWrapper.transform) 
        {
            if(child.transform.localScale.z == -1) Destroy(child.gameObject);
        }

        leftExpScale.fillAmount = 0;
        rightExpScale.fillAmount = 0;
        currentMaxLevel = levelManager.GetCurrentLevel();

        heigthOneLevel = currentTempLevelWrapper.rect.height / currentMaxLevel;

        for(int i = 0; i < currentMaxLevel; i++) 
        {
            GameObject levelPart = Instantiate(tempLevelGO);
            RectTransform rectLevel = levelPart.GetComponent<RectTransform>();
            levelPart.transform.SetParent(currentTempLevelWrapper.transform, false);

            rectLevel.anchoredPosition = new Vector2(0, heigthOneLevel * (i + 1));

            levelPart.GetComponent<Image>().color = inactiveTempLevelColor;
            levelPart.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();

            levelList.Add(levelPart.GetComponent<Image>());
        }
    }

    public void TempLevelUp(float oldLevel)
    {
        levelList[(int)oldLevel].color = activeTempLevelColor;
        currenLevel = oldLevel;

        if(oldLevel + 1 < levelList.Count)
            leftExpScale.fillAmount = 0;

        rightExpScale.fillAmount = heigthOneLevel * (oldLevel + 1) / currentTempLevelWrapper.rect.height;

        Blink(rightExpScale, levelUpColor, normalLevelColor, 100);
    }

    public void UpgradeScale(float scale, float value)
    {
        leftExpScale.fillAmount = value / scale;
        Blink(leftExpScale, levelUpColor, normalLevelColor);
    }

    #endregion

    #region Infirmary

    private void FillInfirmary(float max = 0, float current = 0)
    {
        Color blinkColor = infirmaryUpColor;
        if(max == 0)
        {
            currentMaxInfirmaryCount = playerStats.GetCurrentParameter(PlayersStats.Infirmary);
            currentInfirmaryCount = GlobalStorage.instance.infirmaryManager.injuredList.Count;
        }
        else
        {
            if(currentInfirmaryCount > current) blinkColor = infirmaryDownColor;
            currentMaxInfirmaryCount = max;            
            currentInfirmaryCount = current;
        }

        float widthInfirmary = currentInfirmaryCount / currentMaxInfirmaryCount;

        infirmaryScale.fillAmount = widthInfirmary;
        infirmaryInfo.text = currentInfirmaryCount.ToString() + "/" + currentMaxInfirmaryCount.ToString();
        Blink(infirmaryScale, blinkColor, normalInfirmaryColor);
    }

    private void UpdateInfirmaryUI(float quantity, float capacity)
    {
        FillInfirmary(capacity, quantity);        
    }

    #endregion

    #region Mana

    private void FillMana(float value = 0)
    {
        Color blinkColor = manaUpColor;
        float current = (value == 0) ? resourcesManager.GetResource(ResourceType.Mana) : value;

        if(currentManaCount > current) blinkColor = manaDownColor;
        currentManaCount = current;

        float widthMana = currentManaCount / currentMaxManaCount;

        manaScale.fillAmount = widthMana;
        manaInfo.text = Mathf.Round(currentManaCount) + "/" + currentMaxManaCount;

        Blink(manaScale, blinkColor, normalManaColor, 10);
    }


    #endregion

    #region Health

    public void FillHealth(float value = 0)
    {   
        Color blinkColor = healthUpColor;
        float current = (value == 0) ? resourcesManager.GetResource(ResourceType.Health) : value;
        if(currentHealthCount > current) blinkColor = healthDownColor;
        currentHealthCount = current;

        float widthHealth = currentHealthCount / currentMaxHealthCount;

        healthScale.fillAmount = widthHealth;

        if(currentHealthCount > 0 && currentHealthCount < 1) currentHealthCount = 1f;

        healthInfo.text = Mathf.Round(currentHealthCount) + "/" + currentMaxHealthCount;

        Blink(healthScale, blinkColor, normalHealthColor, 10);
    }

    #endregion

    #region Gold
    private void FillGold(float value = 0)
    {
        float current = 0;

        if(value == 0) 
            startGold = resourcesManager.GetResource(ResourceType.Gold);
        else
            current = value - startGold;

        goldInfo.text = Mathf.Round(current).ToString();
    }

    #endregion

    #region Spells 

    private void FillSpells(int numberOfSpell)
    {
        currentSpells = GlobalStorage.instance.spellManager.GetCurrentSpells();

        if(numberOfSpell == -1)
        {
            foreach(Transform child in spellButtonContainer.transform)
                Destroy(child.gameObject);

            currentSpellsButtons.Clear();

            for(int i = 0; i < countOfActiveSpells; i++)
            {
                int slotNumber = -1;
                if(i < countOfActiveSpells)
                {
                    slotNumber = i + 1;
                    if(i + 1 == 10) slotNumber = 0;
                }

                Button button = Instantiate(buttonSpell);
                button.GetComponent<SpellButtonController>().InitializeButton(currentSpells[i], slotNumber);
                currentSpellsButtons.Add(button);
                button.transform.SetParent(spellButtonContainer.transform, false);

            }
        }
        else
        {
            for(int i = 0; i < countOfActiveSpells; i++)
            {
                if(i == numberOfSpell)
                {
                    Button button = Instantiate(buttonSpell);
                    button.GetComponent<SpellButtonController>().InitializeButton(currentSpells[i]);
                    currentSpellsButtons.Add(button);
                    button.transform.SetParent(spellButtonContainer.transform, false);

                    break;
                }
            }
        }        
    }

    #endregion

    #region EnemiesBar
    public void SetStartEnemiesParameters(int count, BossData[] bossesData)
    {        
        maxEnemiesCount = count;
        currentEnemiesCount = count;

        bosses = bossesData;
        bossIndex = 0;
        bossDict.Clear();

        foreach(GameObject child in bossMarkList)
            Destroy(child);

        bossMarkList.Clear();
        float wrapperWidth = bossWrapper.rect.width;

        for(int i = 0; i < bosses.Length; i++)
        {
            GameObject bossItem = Instantiate(bossMark);
            bossItem.transform.SetParent(bossWrapper.transform, false);

            float wPosition = (maxEnemiesCount - bosses[i].bound) / maxEnemiesCount;
            bossItem.GetComponent<RectTransform>().localPosition = new Vector3(wPosition * wrapperWidth, 0, 0);

            bossMarkList.Add(bossItem);
        }
    }

    public Image ActivateBossMark()
    {
        Image mark = null;

        if(bossIndex < bossMarkList.Count) 
        {
            mark = bossMarkList[(int)bossIndex].GetComponent<Image>();
            mark.color = Color.white;
        } 

        bossIndex++;

        return mark;
    }

    public void FillSpawnEnemiesBar(int currentQuantity, bool isBoss)
    {
        float widthEnemyScale = (maxEnemiesCount - currentQuantity) / maxEnemiesCount;
        spawnValue.fillAmount = widthEnemyScale;

        //if(isBoss == true) ActivateBossMark();
    }

    public void FillDeadEnemiesBar(GameObject enemy)
    {
        if(enemy != null) currentEnemiesCount--;

        float widthEnemyScale = (maxEnemiesCount - currentEnemiesCount) / maxEnemiesCount;

        enemiesValue.fillAmount = widthEnemyScale;
        enemiesInfo.text = currentEnemiesCount.ToString();
    }

    public class BossUIData 
    {
        public BossController boss;
        public Image bossMark;
        public RuneSO rune;
    }

    private Dictionary<BossController, BossUIData> bossDict = new Dictionary<BossController, BossUIData>();

    public void RegisterBoss(float health, BossController bossC)
    {
        Image mark = ActivateBossMark();

        bossDict[bossC] = new BossUIData
        {
            boss = bossC,
            bossMark = mark,
            rune = bossC.rune
        };
    }

    public void UnRegisterBoss(BossController boss)
    {

    }

    public void UpdateBossHealth(float health, BossController boss)
    {

    }


    #endregion

    private void OnEnable()
    {
        EventManager.SwitchPlayer             += Inizialize;
        EventManager.UpdateInfirmaryUI        += UpdateInfirmaryUI;
        EventManager.UpgradeResource          += UpgradeResourceUI;
        //EventManager.EnemiesCount             += GetStartCountEnemies;
        EventManager.EnemyDestroyed           += FillDeadEnemiesBar;
        EventManager.SetBattleBoost           += UpgradeBoostes;
        EventManager.ShowBoostEffect          += ShowBoostEffect;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer             -= Inizialize;
        EventManager.UpdateInfirmaryUI        -= UpdateInfirmaryUI;
        EventManager.UpgradeResource          -= UpgradeResourceUI;
        //EventManager.EnemiesCount             -= GetStartCountEnemies;
        EventManager.EnemyDestroyed           -= FillDeadEnemiesBar;
        EventManager.SetBattleBoost           -= UpgradeBoostes;
        EventManager.ShowBoostEffect          -= ShowBoostEffect;
    }
}
