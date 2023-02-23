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
    int Id { get; set; }

    public object Save();

    public void Load(object state);
}

#endregion