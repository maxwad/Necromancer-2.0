using System.Collections.Generic;
using UnityEngine;

public partial class GlobalMapTileManager : ISaveable
{
    private int _id = 0;

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
        GMTileManagerSD saveData = new GMTileManagerSD();

        saveData.arenaPoint      = arenaBuilder.GetPointsList().ToVec3List();
        saveData.castlesPoints   = castleBuilder.GetPointsList().ToVec3List();
        saveData.altarsPoints    = altarBuilder.GetPointsList().ToVec3List();
        saveData.aiData          = castleBuilder.GetSaveData();
        saveData.tombsPoints     = tombBuilder.GetPoints().ToVec3List();
        saveData.tombsData       = tombBuilder.GetSaveData();
        saveData.resourcesPoints = resourceBuilder.GetPointsList().ToVec3List();
        saveData.resBuildings    = resourceBuilder.GetSaveData();
        saveData.campsPoints     = campBuilder.GetPointsList().ToVec3List();
        (List<Vector3>, List<Reward>) boxesData = TypesConverter.SplitDictionary(boxesBuilder.SaveBoxes());
        saveData.boxesPoints = boxesData.Item1.ToVec3List();
        saveData.boxesRewards = boxesData.Item2;

        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data for GMTileManager");
            return;            
        }

        GMTileManagerSD saveData = manager.ConvertToRequiredType<GMTileManagerSD>(state[Id]);

        Dictionary<Vector3, Reward> boxesData = TypesConverter.CreateDictionary(saveData.boxesPoints.ToVector3List(), saveData.boxesRewards);
        arenaBuilder.Build(this, saveData.arenaPoint.ToVector3List());

        altarBuilder.LoadData(saveData.altarsPoints);

        tombBuilder.Build(this, saveData.tombsPoints.ToVector3List());
        tombBuilder.LoadData(saveData.tombsData);

        resourceBuilder.Build(this, saveData.resourcesPoints.ToVector3List());
        resourceBuilder.LoadData(saveData.resBuildings);
        //WARNING: it's should be here because vassals need ResBuildingRegistrationList
        castleBuilder.Build(this, saveData.castlesPoints.ToVector3List());
        castleBuilder.LoadData(saveData.aiData);

        campBuilder.Build(this);
        campBuilder.LoadData(saveData.campsPoints.ToVector3List());

        boxesBuilder.Build(this, boxesData);

        environmentRegister.Registration(this);
        CreateEnterPointsForAllBuildings();

        manager.LoadDataComplete("GMTileManager is loaded");
    }
}
