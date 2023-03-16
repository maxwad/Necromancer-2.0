using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

#region CASTLE

public interface IUpgradable
{
    public void TryToBuild();

    public BuildingsRequirements GetRequirements();
}

public interface IGarrison
{
    public void StartExchange(bool isCastlesSquad, UnitsTypes unitType);

}

#endregion


#region INPUT SYSTEM

public interface IInputableKeys
{
    public void RegisterInputKeys();

    public void InputHandling(KeyActions keyAction);
}

public interface IInputableAxies
{
    public void RegisterInputAxies();

    public void InputHandling(AxiesData axiesData, MouseData mouseData);
}

#endregion


#region INPUT SYSTEM

public interface ISaveable
{
    public int Id { get; set; }

    public void SetId(int id);

    public int GetId();


    public void Save(SaveLoadManager manager);

    public void Load(SaveLoadManager manager, Dictionary<int, object> state);
}

public interface ISpecialSavable
{
    public ISpecialSaveData Save();

    public void Load(List<ISpecialSaveData> saveData);
}

public interface ISpecialSaveData { }

#endregion