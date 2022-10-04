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
    public Dictionary<ResourceType, float> resourcesDict = new Dictionary<ResourceType, float>();

    [Header("Mana")]
    [SerializeField] private TMP_Text manaCount;

    [Header("Resources")]
    [SerializeField] private TMP_Text goldCount;
    [SerializeField] private TMP_Text foodCount;
    [SerializeField] private TMP_Text woodCount;
    [SerializeField] private TMP_Text stoneCount;
    [SerializeField] private TMP_Text ironCount;
    [SerializeField] private TMP_Text magicCount;
    private Dictionary<ResourceType, TMP_Text> resourceCounters = new Dictionary<ResourceType, TMP_Text>();

    [Header("Deltas")]
    [SerializeField] private GameObject goldContainer;
    [SerializeField] private GameObject foodContainer;
    [SerializeField] private GameObject woodContainer;
    [SerializeField] private GameObject stoneContainer;
    [SerializeField] private GameObject ironContainer;
    [SerializeField] private GameObject magicContainer;
    [SerializeField] private GameObject manaContainer;
    private Dictionary<ResourceType, GameObject> deltaContainers = new Dictionary<ResourceType, GameObject>();

    [Header("Calendar")]
    [SerializeField] private TMP_Text currentDayCount;
    [SerializeField] private TMP_Text currentWeekCount;
    [SerializeField] private TMP_Text currentMonthCount;
    [SerializeField] private TMP_Text weekDescription;
    [SerializeField] private TooltipTrigger weekTooltip;


    [Header("Moves")]
    [SerializeField] private TMP_Text currentMovesCount;
    [SerializeField] private TMP_Text leftDaysCount;
    [SerializeField] private Button nextDayButton;
    [SerializeField] private Animator nextDayBtnAnimator;



    private void Awake()
    {
        poolManager = GlobalStorage.instance.objectsPoolManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        calendarManager = GlobalStorage.instance.calendarManager;
        nextDayBtnAnimator = nextDayButton.GetComponent<Animator>();

        resourceCounters = new Dictionary<ResourceType, TMP_Text>()
        {
            [ResourceType.Gold] = goldCount,
            [ResourceType.Food] = foodCount,
            [ResourceType.Stone] = stoneCount,
            [ResourceType.Wood] = woodCount,
            [ResourceType.Iron] = ironCount,
            [ResourceType.Magic] = magicCount,
            [ResourceType.Mana] = manaCount
        };

        deltaContainers = new Dictionary<ResourceType, GameObject>()
        {
            [ResourceType.Gold] = goldContainer,
            [ResourceType.Food] = foodContainer,
            [ResourceType.Stone] = stoneContainer,
            [ResourceType.Wood] = woodContainer,
            [ResourceType.Iron] = ironContainer,
            [ResourceType.Magic] = magicContainer,
            [ResourceType.Mana] = manaContainer
        };
    }

    private void Start()
    {
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
            resource.Value.text = resourcesDict[resource.Key].ToString();
        }
    }

    private void FillResource(ResourceType type, float value)
    {
        if(type == ResourceType.Health || type == ResourceType.Exp) return;

        resourceCounters[type].text = value.ToString();
    }

    public void UpdateCurrentMoves( float currentValue)
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

    public void UpdateDecadeOnCalendar(Decade decade)
    {
        weekDescription.text = decade.decadeName;
        weekTooltip.content = decade.decadeDescription;
    }

    public void NextDay()
    {
        calendarManager.NextDay();
    }

    private void OnEnable()
    {
        EventManager.UpgradeResource += FillResource;
        //EventManager.UpgradeStatCurrentValue += UpdateCurrentMoves;
        EventManager.SwitchPlayer += EnableUI;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= EnableUI;
        EventManager.UpgradeResource -= FillResource;
        //EventManager.UpgradeStatCurrentValue -= UpdateCurrentMoves;
    }
}
