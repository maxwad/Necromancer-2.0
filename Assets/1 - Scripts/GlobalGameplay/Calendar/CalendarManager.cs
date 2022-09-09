using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarData
{
    public int daysLeft;

    public int currentDay;
    public int currentWeek;
    public int currentMonth;

    public CalendarData(int left, int day, int week, int month)
    {
        daysLeft = left;
        currentDay = day;
        currentWeek = week;
        currentMonth = month;
    }
}


public class CalendarManager : MonoBehaviour
{
    public int daysLeft = 1000;
    private int daysPassed = 0;

    private int day = 1;
    private int dayMax = 7;

    private int week = 1;
    private int weekMax = 4;

    private int month = 1;
    private int monthMax = 12;

    private int year = 0;

    private GMInterface gmInterface;
    private CalendarData calendarData;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        calendarData = new CalendarData(daysLeft, day, week, month);
        gmInterface.UpdateCalendar(calendarData);
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(MenuManager.instance.IsTherePauseOrMiniPause() == false && GlobalStorage.instance.isGlobalMode == true)
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
            week++;
            day = 1;
            //new week
        }

        if(week > weekMax)
        {
            month++;
            week = 1;
            //new month
        }

        if(month > monthMax)
        {
            year++;
            month = 1;
            //new year
        }

       calendarData = new CalendarData(daysLeft, day, week, month);
        gmInterface.UpdateCalendar(calendarData);
    }
}
