using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class GMInterface : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    private ObjectsPoolManager poolManager;
    private ResourcesManager resourcesManager;
    private CalendarManager calendarManager;
    private PlayerPersonalWindow personalWindow;
    public Dictionary<ResourceType, float> resourcesDict = new Dictionary<ResourceType, float>();

    [Header("Resources")]
    [SerializeField] private GameObject resourcesContainer;
    [SerializeField] private TMP_Text goldCount;
    [SerializeField] private TMP_Text foodCount;
    [SerializeField] private TMP_Text woodCount;
    [SerializeField] private TMP_Text stoneCount;
    [SerializeField] private TMP_Text ironCount;
    private Dictionary<ResourceType, TMP_Text> resourceCounters = new Dictionary<ResourceType, TMP_Text>();

    [Header("Deltas")]
    [SerializeField] private GameObject goldContainer;
    [SerializeField] private GameObject foodContainer;
    [SerializeField] private GameObject woodContainer;
    [SerializeField] private GameObject stoneContainer;
    [SerializeField] private GameObject ironContainer;
    private Dictionary<ResourceType, GameObject> deltaContainers = new Dictionary<ResourceType, GameObject>();

    [Header("Calendar")]
    [SerializeField] private GameObject calendarContainer;
    [SerializeField] private TMP_Text currentDayCount;
    [SerializeField] private TMP_Text currentWeekCount;
    [SerializeField] private TMP_Text currentMonthCount;
    [SerializeField] private TMP_Text weekDescription;
    [SerializeField] private TooltipTrigger weekTooltip;


    [Header("Moves")]
    [SerializeField] private GameObject movesContainer;
    [SerializeField] private TMP_Text currentMovesCount;
    [SerializeField] private TMP_Text leftDaysCount;
    [SerializeField] private Button nextDayButton;
    private Animator nextDayBtnAnimator;

    [Header("Hero")]
    [SerializeField] private GameObject heroContainer;
    [SerializeField] private TMP_Text manaAmount;
    [SerializeField] private Image manaScale;
    private float manaMax;
    [SerializeField] private TMP_Text healthAmount;
    [SerializeField] private Image healthScale;
    private float healthMax;
    [SerializeField] private Image levelScale;
    [SerializeField] private TMP_Text levelCount;
    [SerializeField] private TooltipTrigger levelTooltip;
    [SerializeField] private Button heroButton;
    [SerializeField] private Animator skillsButton;

    [Header("ShortCuts")]
    public KeyCode armyKey;
    public KeyCode skillsKey;
    public KeyCode microKey;
    public KeyCode spellsKey;


    private void Awake()
    {
        poolManager = GlobalStorage.instance.objectsPoolManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        calendarManager = GlobalStorage.instance.calendarManager;
        personalWindow = GlobalStorage.instance.playerMilitaryWindow;
        nextDayBtnAnimator = nextDayButton.GetComponent<Animator>();

        resourceCounters = new Dictionary<ResourceType, TMP_Text>()
        {
            [ResourceType.Gold] = goldCount,
            [ResourceType.Food] = foodCount,
            [ResourceType.Stone] = stoneCount,
            [ResourceType.Wood] = woodCount,
            [ResourceType.Iron] = ironCount,
            [ResourceType.Mana] = manaAmount,
            [ResourceType.Health] = healthAmount
        };

        deltaContainers = new Dictionary<ResourceType, GameObject>()
        {
            [ResourceType.Gold] = goldContainer,
            [ResourceType.Food] = foodContainer,
            [ResourceType.Stone] = stoneContainer,
            [ResourceType.Wood] = woodContainer,
            [ResourceType.Iron] = ironContainer
        };

    }

    private void Start()
    {
        healthMax = resourcesManager.GetMaxHealth();
        manaMax = resourcesManager.GetMaxMana();
        uiPanel.SetActive(true);

        FillStartResources();
    }

    private void EnableUI(bool mode)
    {
        uiPanel.SetActive(mode);
    }

    public void ShowDelta(ResourceType resType, float value)
    {
        if(GlobalStorage.instance.isGlobalMode == false) return;

        GameObject delta = poolManager.GetObject(ObjectPool.DeltaCost);

        delta.transform.SetParent(deltaContainers[resType].transform, false);
        delta.SetActive(true);
        delta.GetComponent<DeltaCost>().ShowDelta(value);
    }

    private void FillStartResources()
    {
        resourcesDict = resourcesManager.GetAllResources();

        foreach(var resource in resourceCounters)
        {
            if(resource.Key == ResourceType.Mana || resource.Key == ResourceType.Health)
            {
                //Debug.Log("HERE Current = " + currentAmount + "max = " + maxAmount);
                UpgrateManaHealthUI(resource.Key, resourcesDict[resource.Key]);
            }
            else
            {
                resource.Value.text = Mathf.Round(resourcesDict[resource.Key]).ToString();
            }
        }
    }

    private void FillResource(ResourceType type, float value)
    {
        if(type == ResourceType.Exp) return;

        resourceCounters[type].text = Mathf.Round(value).ToString();

        if(type == ResourceType.Health || type == ResourceType.Mana) UpgrateManaHealthUI(type, value);

    }

    private void UpgrateMaxManaHealth(PlayersStats stat, float maxValue)
    {
        ResourceType type = ResourceType.Mana;

        if(stat == PlayersStats.Mana) 
        {
            manaMax = maxValue;
        }

        if(stat == PlayersStats.Health)
        {             
            healthMax = maxValue;
            type = ResourceType.Health;
        }

        UpgrateManaHealthUI(type, resourcesDict[type]);
    }

    private void UpgrateManaHealthUI(ResourceType stat, float value)
    {
        float maxAmount;
        TMP_Text currentText;
        Image scale;

        float currentAmount = value;

        if(stat == ResourceType.Mana)
        {
            maxAmount = manaMax;
            currentText = manaAmount;
            scale = manaScale;
        }
        else
        {
            maxAmount = healthMax;
            currentText = healthAmount;
            scale = healthScale;

            if(currentAmount > 0 && currentAmount < 1) currentAmount = 1f;
        }

        scale.fillAmount = currentAmount / maxAmount;
        currentText.text = Mathf.Round(currentAmount) + "/" + maxAmount;
    }

    public void UpgradeLevel(LevelData data)
    {
        levelScale.fillAmount = data.currentExp / data.boundExp;
        levelCount.text = data.level.ToString();
        levelTooltip.content = data.currentExp + "/" + data.boundExp;
    }

    public void UpgradeAbilityPoints(int amount)
    {
        bool mode = (amount == 0) ? false : true;
        skillsButton.GetComponent<Animator>().SetBool(TagManager.A_BLINK, mode);
    }

    public void UpdateCurrentMoves(float currentValue)
    {
        currentMovesCount.text = currentValue.ToString();

        bool mode = (currentValue == 0) ? true : false;
        nextDayBtnAnimator.SetBool(TagManager.A_BLINK, mode);         
    }

    public void UpdateCalendar(CalendarData data)
    {
        leftDaysCount.text = data.daysLeft.ToString();
        currentDayCount.text = data.currentDay.ToString();
        currentWeekCount.text = data.currentDecade.ToString();
        currentMonthCount.text = data.currentMonth.ToString();
    }

    public void UpdateDecadeOnCalendar(DecadeSO decade)
    {
        weekDescription.text = decade.decadeName;

        string description = (decade.isNegative == true) ? decade.effect.negativeDescription : decade.effect.positiveDescription;
        description = description.Replace("$", decade.effect.value.ToString());

        weekTooltip.content = description;
    }

    public void ShowInterfaceElements(bool mode)
    {
        calendarContainer.SetActive(mode);
        movesContainer.SetActive(mode);
        heroContainer.SetActive(mode);
    }


    #region Buttons

    //BUTTON
    public void NextDay()
    {
        calendarManager.NextDay();
    }

    public void OpenArmyWindow()
    {
        personalWindow.PressButton(armyKey);
    }

    public void OpenSkillsWindow()
    {
        personalWindow.PressButton(skillsKey);
    }

    public void OpenMicroWindow()
    {
        personalWindow.PressButton(microKey);
    }

    public void OpenSpellsWindow()
    {
        personalWindow.PressButton(spellsKey);
    }

    #endregion

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgrateMaxManaHealth;
        EventManager.UpgradeResource += FillResource;
        EventManager.SwitchPlayer += EnableUI;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgrateMaxManaHealth;
        EventManager.SwitchPlayer -= EnableUI;
        EventManager.UpgradeResource -= FillResource;
    }
}
