using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public interface IUpgradable
{
    public void TryToBuild();

    public BuildingsRequirements GetRequirements();
}

public interface IGarrison
{
    public void StartExchange(bool isCastlesSquad, UnitsTypes unitType);

}

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

public interface IInputableMouse
{
    public void RegisterInputMouse();

    public void InputHandling(KeyActions keyAction);
}

