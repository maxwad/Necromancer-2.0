using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Enums;

public class GMInterfaceCalendar : MonoBehaviour
{
    private GMInterface mainInterface;

    [Header("Calendar")]
    [SerializeField] private GameObject calendarContainer;
    [SerializeField] private TMP_Text currentDayCount;
    [SerializeField] private TMP_Text currentWeekCount;
    [SerializeField] private TMP_Text currentMonthCount;
    [SerializeField] private TMP_Text weekDescription;
    [SerializeField] private TooltipTrigger weekTooltip;

    public void UpdateCalendar(CalendarData data)
    {
        if(mainInterface == null) mainInterface = GetComponent<GMInterface>();
        mainInterface.movesPart.DaysLeft(data.daysLeft);

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

    public void ShowBlock(bool mode)
    {
        calendarContainer.SetActive(mode);
    }
}
