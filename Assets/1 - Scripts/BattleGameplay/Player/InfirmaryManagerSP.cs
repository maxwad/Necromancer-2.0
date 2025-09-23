using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public partial class InfirmaryManager : ISaveable
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
        InfirmarySD saveData = new InfirmarySD();

        (List<UnitsTypes>, List<InjuredUnitData>) boxesData = TypesConverter.SplitDictionary(GetCurrentInjuredDict());
        saveData.units = boxesData.Item1;
        saveData.quantity = boxesData.Item2;

        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data about Infirmary");
            return;
        }

        InfirmarySD saveData = TypesConverter.ConvertToRequiredType<InfirmarySD>(state[Id]);
        injuredDict = TypesConverter.CreateDictionary(saveData.units, saveData.quantity);

        manager.LoadDataComplete("Infirmary is loaded");
    }
}
