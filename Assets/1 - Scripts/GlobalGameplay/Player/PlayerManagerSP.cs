using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class PlayerManager : ISaveable
{
    private PlayersArmy playersArmy;
    private GMPlayerMovement playerMovement;

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

    private void Awake()
    {
        playerMovement = GetComponentInChildren<GMPlayerMovement>(true);
        playersArmy = GetComponent<PlayersArmy>();
    }

    public void SetId(int id)
    {
        if(Id >= 100) Id = id;
    }

    public int GetId() => Id;

    public void Save(SaveLoadManager manager)
    {
        PlayerSD saveData = new PlayerSD();

        saveData.parameters = playerMovement.Save();
        saveData.army = playersArmy.Save();


        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data for PLAYER");
            return;
        }

        PlayerSD saveData = TypesConverter.ConvertToRequiredType<PlayerSD>(state[Id]);

        playerMovement.Load(saveData.parameters);
        playersArmy.Load(saveData.army);

        manager.LoadDataComplete("Player are loaded");
    }


}


[Serializable]
public class PlayerSD
{
    public PlayersMovementSD parameters;
    public PlayersArmySD army;
}

[Serializable]
public class PlayersArmySD
{
    public List<PlayersArmySquadInfoSD> wholeArmy = new List<PlayersArmySquadInfoSD>();
    public int[] activeArmy = new int[4] { -1, -1, -1, -1 };
}

[Serializable]
public class PlayersArmySquadInfoSD
{
    public UnitsTypes unit;
    public UnitStatus status;
    public int quantity = 0;
    public int index = -1;
}

[Serializable]
public class PlayersMovementSD
{
    public bool flipHero = false;
    public float movementPoints = 0;
    public bool isExtraMovementWaisted = false;
    public Vec3 position;

}
