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

//public interface ISpecialBuilding 
//{
//    public void Init(CastleBuildings building);
//}
