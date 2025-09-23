using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using Zenject;

public partial class CalendarManager : MonoBehaviour, IInputableKeys
{
    private BoostManager boostManager;
    private InputSystem inputSystem;
    private GameMaster gameMaster;
    private GMInterface gmInterface;

    public int daysLeft = 1000;

    private int day = 1;
    private int dayMax = 10;
    private int daysPassed = 0;

    private int week = 1;
    private int weekMax = 3;
    private int weeksPassed = 0;

    private int month = 1;
    private int monthMax = 12;
    private int monthsPassed = 0;

    private int year = 0;
    private int yearsPassed = 0;

    private CalendarData calendarData;

    [Header("Decades List")]
    [SerializeField] private List<DecadeSO> decadeList;

    private DecadeSO currentDecadeEffect;
    private int currentDecadeIndex = 0;

    [Inject]
    private void Construct(
        GameMaster gameMaster,
        InputSystem inputSystem,
        BoostManager boostManager,
        GMInterface gmInterface)
    {
        this.gameMaster   = gameMaster;
        this.inputSystem  = inputSystem;
        this.boostManager = boostManager;
        this.gmInterface  = gmInterface;
    }

    private void Start()
    {
        RegisterInputKeys();        
    }

    public void Init(bool createMode)
    {
        UpdateCalendar();

        if(createMode == true)
        {
            decadeList = ShuffleList(decadeList);
            StartCoroutine(SetStartDecade());
        }

        GlobalStorage.instance.LoadNextPart();
    }

    private IEnumerator SetStartDecade()
    {
        while(GlobalStorage.instance.isGameLoaded == false)
        {
            yield return null;
        }

        NewDecade();
    }

    public void RegisterInputKeys()
    {
        inputSystem.RegisterInputKeys(KeyActions.NewDay, this);
    }

    public void InputHandling(KeyActions keyAction)
    {
        if(GlobalStorage.instance.isGlobalMode == true)
        {
            if(MenuManager.instance.IsTherePauseOrMiniPause() == false && GlobalStorage.instance.isModalWindowOpen == false)
            {
                NextDay();
            }
        }
    }


    public void NextDay()
    {
        EventManager.OnNewMoveEvent();

        daysPassed++;
        daysLeft--;

        day++;
        if(day > dayMax)
        {
            //new decade
            week++;
            weeksPassed++;
            day = 1;

            currentDecadeIndex++;
            if(currentDecadeIndex >= decadeList.Count) currentDecadeIndex = 0;

            EventManager.OnWeekEndEvent();
            NewDecade();
            EventManager.OnNewWeekEvent(weeksPassed);

        }

        if(week > weekMax)
        {
            //new month
            month++;
            monthsPassed++;
            week = 1;

            EventManager.OnNewMonthEvent();
            Debug.Log("New month");
        }

        if(month > monthMax)
        {        
            //new year
            year++;
            yearsPassed++;
            month = 1;
        }

        UpdateCalendar();

        gameMaster.EnemyTurn();
    }

    private List<DecadeSO> ShuffleList(List<DecadeSO> oldList)
    {
        List<DecadeSO> newList = new List<DecadeSO>();

        int counter = oldList.Count;
        for(int i = 0; i < counter; i++)
        {
            int randomElementInList = Random.Range(0, oldList.Count);
            newList.Add(oldList[randomElementInList]);
            oldList.Remove(oldList[randomElementInList]);
        }

        return newList;
    }

    private void NewDecade()
    {
        BoostType boost;
        float value;

        if(currentDecadeEffect != null)
        {
            boost = TypesConverter.RuneToBoostType(currentDecadeEffect.effect.rune);
            value = (currentDecadeEffect.isNegative == true) ? -currentDecadeEffect.effect.value : currentDecadeEffect.effect.value;
            boostManager.DeleteBoost(boost, BoostSender.Calendar, value);
        }

        currentDecadeEffect = decadeList[currentDecadeIndex];
        gmInterface.calendarPart.UpdateDecadeOnCalendar(currentDecadeEffect);

        boost = TypesConverter.RuneToBoostType(currentDecadeEffect.effect.rune);
        value = (currentDecadeEffect.isNegative == true) ? -currentDecadeEffect.effect.value : currentDecadeEffect.effect.value;

        boostManager.SetBoost(boost, BoostSender.Calendar, currentDecadeEffect.purpose, value);
    }

    public void UpdateCalendar()
    {
        calendarData = new CalendarData(daysLeft, day, week, month);
        gmInterface.calendarPart.UpdateCalendar(calendarData);
    }
}
