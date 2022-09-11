using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int daysPassed = 0;

    private int day = 1;
    private int dayMax = 10;

    private int decade = 1;
    private int decadeMax = 3;

    private int month = 1;
    private int monthMax = 12;

    private int year = 0;

    private GMInterface gmInterface;
    private CalendarData calendarData;
    private CalendarBoostActivator activator;

    [Header("Decades List")]
    [SerializeField] private List<DecadeSO> decadeSOList;
    [SerializeField] private List<Decade> decadeList = new List<Decade>();

    private Decade currentDecadeEffect;
    private int currentDecadeIndex = 0;


    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        activator = GetComponent<CalendarBoostActivator>();
        calendarData = new CalendarData(daysLeft, day, decade, month);
        gmInterface.UpdateCalendar(calendarData);

        for(int i = 0; i < decadeSOList.Count; i++)
        {
            decadeList.Add(new Decade(decadeSOList[i], i));
        }

        decadeList = ShuffleList(decadeList);
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

    private void NextDay()
    {
        EventManager.OnNewMoveEvent();

        daysPassed++;
        daysLeft--;

        day++;
        if(day > dayMax)
        {
            //new decade
            decade++;
            day = 1;

            currentDecadeIndex++;
            if(currentDecadeIndex >= decadeList.Count) currentDecadeIndex = 0;          

            NewDecade();
            EventManager.OnNewMonthEvent();
        }

        if(decade > decadeMax)
        {
            //new month
            month++;
            decade = 1;

            Debug.Log("New month");
           // EventManager.OnNewMonthEvent();
        }

        if(month > monthMax)
        {        
            //new year
            year++;
            month = 1;
        }

        calendarData = new CalendarData(daysLeft, day, decade, month);
        gmInterface.UpdateCalendar(calendarData);
    }

    private List<Decade> ShuffleList(List<Decade> oldList)
    {
        List<Decade> newList = new List<Decade>();

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
        currentDecadeEffect = decadeList[currentDecadeIndex];
        gmInterface.UpdateDecadeOnCalendar(currentDecadeEffect);
        activator.ActivateBoost(currentDecadeEffect);
    }
}
