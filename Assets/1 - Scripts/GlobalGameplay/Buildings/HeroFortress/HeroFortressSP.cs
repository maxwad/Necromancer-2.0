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

    [System.Serializable]
    public class HeroFortressSD
    {
        public int marketDays = 0;
        public int seals = 0;

        public bool isHeroInside = false;
        public bool isHeroVisitedOnThisWeek = false;

        public HFBuildingsSD specialBuildingsSD = new HFBuildingsSD();

        
    }

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

        HeroFortressSD saveData = manager.ConvertToRequiredType<HeroFortressSD>(state[Id]);

        marketDays = saveData.marketDays;
        seals = saveData.seals;
        isHeroInside = saveData.isHeroInside;
        isHeroVisitedOnThisWeek = saveData.isHeroVisitedOnThisWeek;

        buildings.Load(saveData.specialBuildingsSD);

        manager.LoadDataComplete("Resources are loaded");
    }
}
