using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class CalendarManager : ISaveable
{
    [HideInInspector] public int _id = -1;

    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    public void SetId(int id)
    {
        if(Id == -1) Id = id;
    }

    public int GetId() => Id;

    public void Save(SaveLoadManager manager)
    {
        CalendarSD saveData = new CalendarSD();

        saveData.day = day;
        saveData.week = week;
        saveData.month = month;
        saveData.year = year;

        saveData.daysPassed = daysPassed;
        saveData.daysLeft = daysLeft;
        saveData.weeksPassed = weeksPassed;
        saveData.monthsPassed = monthsPassed;
        saveData.yearsPassed = yearsPassed;

        saveData.currentDecadeIndex = currentDecadeIndex;

        foreach(var decade in decadeList)
            saveData.decadeList.Add(decade.decadeName);

        manager.FillSaveData(Id, saveData);
    }


    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data for Calendar");
            return;
        }

        CalendarSD saveData = manager.ConvertToRequiredType<CalendarSD>(state[Id]);

        day = saveData.day;
        week = saveData.week;
        month = saveData.month;
        year = saveData.year;

        daysPassed = saveData.daysPassed;
        daysLeft = saveData.daysLeft;
        weeksPassed = saveData.weeksPassed;
        monthsPassed = saveData.monthsPassed;
        yearsPassed = saveData.yearsPassed;

        currentDecadeIndex = saveData.currentDecadeIndex;

        List<DecadeSO> loadedDecadeList = new List<DecadeSO>();

        foreach(var decade in saveData.decadeList)
            loadedDecadeList.Add(decadeList.Where(item => item.decadeName == decade).First());

        decadeList = loadedDecadeList;

        NewDecade();
        UpdateCalendar();

        manager.LoadDataComplete("Calendar is loaded");
    }
}
