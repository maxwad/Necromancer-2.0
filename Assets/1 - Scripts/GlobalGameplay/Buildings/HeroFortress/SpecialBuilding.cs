using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public abstract class SpecialBuilding : MonoBehaviour
{
    //Common Class for inspector
    public abstract GameObject Init(CastleBuildings building);

    // public abstract void Load(List<ISpecialSaveData> saveData);

    public abstract void Load(List<ISpecialSaveData> saveData);

    public abstract ISpecialSaveData Save();
}
