using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

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

    public void Save(SaveLoadManager manager)
    {
        GMTileManagerSD saveData = new GMTileManagerSD();

        saveData.arenaPoint = arenaBuilder.GetPointsList().ToVec3List();
        saveData.castlesPoints = castleBuilder.GetPointsList().ToVec3List();
        saveData.tombsPoints = tombBuilder.GetPointsList().ToVec3List();
        saveData.resourcesPoints = resourceBuilder.GetPointsList().ToVec3List();

        //GMTileManagerSDLarge saveData = new GMTileManagerSDLarge();

        //saveData.arenaPoint = arenaBuilder.GetPointsList();
        //saveData.castlesPoints = castleBuilder.GetPointsList();
        //saveData.tombsPoints = tombBuilder.GetPointsList();
        //saveData.resourcesPoints = resourceBuilder.GetPointsList();

        (List<Vector3>, List<Reward>) boxesData = TypesConverter.SplitDictionary(boxesBuilder.SaveBoxes());
        saveData.boxesPoints = boxesData.Item1.ToVec3List();
        //saveData.boxesPoints = boxesData.Item1;
        saveData.boxesRewards = boxesData.Item2;

        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            Debug.Log("There is no data for loading GMTileManager!");
            return;            
        }

        GMTileManagerSD saveData = manager.ConvertToRequiredType<GMTileManagerSD>(state[Id]);
        Dictionary<Vector3, Reward> boxesData = TypesConverter.CreateDictionary(saveData.boxesPoints.ToVector3List(), saveData.boxesRewards);

        arenaBuilder.Build(this, saveData.arenaPoint.ToVector3List());
        castleBuilder.Build(this, saveData.castlesPoints.ToVector3List());
        tombBuilder.Build(this, saveData.tombsPoints.ToVector3List());
        resourceBuilder.Build(this, saveData.resourcesPoints.ToVector3List());

        //GMTileManagerSDLarge saveData = manager.ConvertToRequiredType<GMTileManagerSDLarge>(state[Id]);
        //Dictionary<Vector3, Reward> boxesData = TypesConverter.CreateDictionary(saveData.boxesPoints, saveData.boxesRewards);

        //arenaBuilder.Build(this, saveData.arenaPoint);
        //castleBuilder.Build(this, saveData.castlesPoints);
        //tombBuilder.Build(this, saveData.tombsPoints);
        //resourceBuilder.Build(this, saveData.resourcesPoints);
        campBuilder.Build(this);
        boxesBuilder.Build(this, boxesData);

        environmentRegister.Registration(this);
        CreateEnterPointsForAllBuildings();

        manager.LoadDataComplete("GMTileManager is loaded");
    }
}
