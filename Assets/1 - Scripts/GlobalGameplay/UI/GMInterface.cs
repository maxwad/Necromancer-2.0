using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class GMInterface : MonoBehaviour
{
    private PlayerStats playerStats;
    [SerializeField] private GameObject uiPanel;
    private ObjectsPoolManager poolManager;
    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, float> resourcesDict = new Dictionary<ResourceType, float>();

    [Header("Mana")]
    [SerializeField] private TMP_Text manaCount;
    private float currentMaxManaCount;
    private float currentManaCount;

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

    [Header("Moves")]
    [SerializeField] private TMP_Text currentMovesCount;
    [SerializeField] private TMP_Text leftDaysCount;
    [SerializeField] private TMP_Text currentDayCount;
    [SerializeField] private TMP_Text currentWeekCount;
    [SerializeField] private TMP_Text currentMonthCount;
    [SerializeField] private TooltipTrigger decadeTooltip;


    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        poolManager = GlobalStorage.instance.objectsPoolManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;

        resourceCounters = new Dictionary<ResourceType, TMP_Text>()
        {
            [ResourceType.Gold] = goldCount,
            [ResourceType.Food] = foodCount,
            [ResourceType.Stone] = stoneCount,
            [ResourceType.Wood] = woodCount,
            [ResourceType.Iron] = ironCount,
            [ResourceType.Magic] = magicCount
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

        FillMana();
        FillResources();
    }

    private void EnableUI(bool mode)
    {
        uiPanel.SetActive(mode);
    }

    private void FillMana(float max = 0, float current = 0)
    {
        if(max == 0)
        {
            currentMaxManaCount = playerStats.GetStartParameter(PlayersStats.Mana);
            currentManaCount = playerStats.GetCurrentParameter(PlayersStats.Mana);
        }
        else
        {
            currentMaxManaCount = max;
            currentManaCount = current;
        }

        manaCount.text = currentManaCount.ToString();
    }

    private void UpdateManaUI(PlayersStats stat, float maxValue, float currentValue)
    {
        if(stat == PlayersStats.Mana) FillMana(maxValue, currentValue);        
    }

    public void ShowDelta(ResourceType resType, float value)
    {
        if(GlobalStorage.instance.isGlobalMode == false) return;

        GameObject delta = poolManager.GetObjectFromPool(ObjectPool.DeltaCost);

        delta.transform.SetParent(deltaContainers[resType].transform);
        delta.SetActive(true);
        delta.GetComponent<DeltaCost>().ShowDelta(value);
    }

    private void FillResources()
    {
        resourcesDict = resourcesManager.GetAllResources();

        foreach(var resource in resourceCounters)
        {
            resource.Value.text = resourcesDict[resource.Key].ToString();

        }

        //foreach(var resource in resourcesDict)
        //{

        //    switch(resource.Key)
        //    {
        //        case ResourceType.Gold:
        //            goldCount.text = resource.Value.ToString();
        //            break;

        //        case ResourceType.Food:
        //            foodCount.text = resource.Value.ToString();
        //            break;

        //        case ResourceType.Stone:
        //            stoneCount.text = resource.Value.ToString();
        //            break;

        //        case ResourceType.Wood:
        //            woodCount.text = resource.Value.ToString();
        //            break;

        //        case ResourceType.Iron:
        //            ironCount.text = resource.Value.ToString();
        //            break;

        //        case ResourceType.Magic:
        //            magicCount.text = resource.Value.ToString();
        //            break;

        //        default:
        //            break;
        //    }
        //}
    }

    private void UpdateCurrentMoves(PlayersStats stat, float maxValue, float currentValue)
    {
        if(stat == PlayersStats.MovementDistance) currentMovesCount.text = currentValue.ToString();
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
        decadeTooltip.header = decade.decadeName;
        decadeTooltip.content = decade.decadeDescription;
    }

    private void OnEnable()
    {
        EventManager.UpgradeStatCurrentValue += UpdateManaUI;
        EventManager.UpgradeResources += FillResources;
        EventManager.UpgradeStatCurrentValue += UpdateCurrentMoves;
        EventManager.ChangePlayer += EnableUI;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer -= EnableUI;
        EventManager.UpgradeResources -= FillResources;
        EventManager.UpgradeStatCurrentValue -= UpdateCurrentMoves;
        EventManager.UpgradeStatCurrentValue -= UpdateManaUI;
    }
}
