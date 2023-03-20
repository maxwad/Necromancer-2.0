using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class HeroFortress : ISaveable
{
    [SerializeField] private int _id = 101;

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
        if(Id >= 100) Id = id;
    }

    public int GetId() => Id;

    public void Save(SaveLoadManager manager)
    {
        HeroFortressSD saveData = new HeroFortressSD();

        saveData.marketDays              = marketDays;
        saveData.seals                   = seals;
        saveData.isHeroInside            = isHeroInside;
        saveData.isHeroVisitedOnThisWeek = isHeroVisitedOnThisWeek;

        saveData.specialBuildingsSD = buildings.Save();

        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data for Calendar");
            return;
        }

        HeroFortressSD saveData = TypesConverter.ConvertToRequiredType<HeroFortressSD>(state[Id]);

        marketDays = saveData.marketDays;
        seals = saveData.seals;
        isHeroInside = saveData.isHeroInside;
        isHeroVisitedOnThisWeek = saveData.isHeroVisitedOnThisWeek;

        buildings.Load(saveData.specialBuildingsSD);

        manager.LoadDataComplete("Resources are loaded");
    }
}
