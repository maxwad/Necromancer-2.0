using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class CalendarData
{
    public int daysLeft;

    public int currentDay;
    public int currentDecade;
    public int currentMonth;

    public CalendarData(int left, int day, int decade, int month)
    {
        daysLeft = left;
        currentDay = day;
        currentDecade = decade;
        currentMonth = month;
    }
}


public class CalendarManager : MonoBehaviour
{
    public int daysLeft = 1000;

    private int day = 1;
    private int dayMax = 10;
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
    private BoostManager boostManager;

    public void Init()
    {
        boostManager = GlobalStorage.instance.boostManager;
        gmInterface = GlobalStorage.instance.gmInterface;
        calendarData = new CalendarData(daysLeft, day, decade, month);
        gmInterface.UpdateCalendar(calendarData);

        //decadeList = ShuffleList(decadeList);
        StartCoroutine(SetStartDecade());

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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && GlobalStorage.instance.isGlobalMode == true)
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

        }

        if(decade > decadeMax)
        {
            //new month
            month++;
            monthsPassed++;
            decade = 1;

            Debug.Log("New month");
            EventManager.OnNewMonthEvent();
        }

        if(month > monthMax)
        {        
            //new year
            year++;
            yearsPassed++;
            month = 1;
        }

        calendarData = new CalendarData(daysLeft, day, decade, month);
        gmInterface.UpdateCalendar(calendarData);
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
        gmInterface.UpdateDecadeOnCalendar(currentDecadeEffect);

        boost = EnumConverter.instance.RuneToBoostType(currentDecadeEffect.effect.rune);
        value = (currentDecadeEffect.isNegative == true) ? -currentDecadeEffect.effect.value : currentDecadeEffect.effect.value;
        boostManager.SetBoost(boost, BoostSender.Calendar, currentDecadeEffect.purpose, value);
    }
}
