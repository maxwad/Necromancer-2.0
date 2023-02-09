using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class CalendarManager : MonoBehaviour, IInputableKeys
{
    private BoostManager boostManager;
    private InputSystem inputSystem;
    private GameMaster gameMaster;

    public int daysLeft = 1000;

    private int day = 1;
    private int dayMax = 1;
    private int daysPassed = 0;

    private int decade = 1;
    private int decadeMax = 3;
    private int decadesPassed = 0;

    private int month = 1;
    private int monthMax = 12;
    private int monthsPassed = 0;

    private int year = 0;
    private int yearsPassed = 0;

    private GMInterface gmInterface;
    private CalendarData calendarData;

    [Header("Decades List")]
    [SerializeField] private List<DecadeSO> decadeList;

    private DecadeSO currentDecadeEffect;
    private int currentDecadeIndex = 0;

    public void Init()
    {
        boostManager = GlobalStorage.instance.boostManager;
        gmInterface = GlobalStorage.instance.gmInterface;
        gameMaster = GlobalStorage.instance.gameMaster;
        calendarData = new CalendarData(daysLeft, day, decade, month);
        gmInterface.calendarPart.UpdateCalendar(calendarData);

        //decadeList = ShuffleList(decadeList);
        StartCoroutine(SetStartDecade());
        RegisterInputKeys();

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
        inputSystem = GlobalStorage.instance.inputSystem;
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
            decade++;
            decadesPassed++;
            day = 1;

            currentDecadeIndex++;
            if(currentDecadeIndex >= decadeList.Count) currentDecadeIndex = 0;

            EventManager.OnWeekEndEvent();
            NewDecade();
            EventManager.OnNewWeekEvent(decadesPassed);

            EventManager.OnNewMonthEvent();
        }

        if(decade > decadeMax)
        {
            //new month
            month++;
            monthsPassed++;
            decade = 1;

            Debug.Log("New month");
        }

        if(month > monthMax)
        {        
            //new year
            year++;
            yearsPassed++;
            month = 1;
        }

        calendarData = new CalendarData(daysLeft, day, decade, month);
        gmInterface.calendarPart.UpdateCalendar(calendarData);

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
            boost = EnumConverter.instance.RuneToBoostType(currentDecadeEffect.effect.rune);
            value = (currentDecadeEffect.isNegative == true) ? -currentDecadeEffect.effect.value : currentDecadeEffect.effect.value;
            boostManager.DeleteBoost(boost, BoostSender.Calendar, value);
        }        

        currentDecadeEffect = decadeList[currentDecadeIndex];
        gmInterface.calendarPart.UpdateDecadeOnCalendar(currentDecadeEffect);

        boost = EnumConverter.instance.RuneToBoostType(currentDecadeEffect.effect.rune);
        value = (currentDecadeEffect.isNegative == true) ? -currentDecadeEffect.effect.value : currentDecadeEffect.effect.value;

        boostManager.SetBoost(boost, BoostSender.Calendar, currentDecadeEffect.purpose, value);
    }
}
