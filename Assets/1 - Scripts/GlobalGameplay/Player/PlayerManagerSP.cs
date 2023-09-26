using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static NameManager;

public partial class PlayerManager : ISaveable
{
    private PlayersArmy playersArmy;
    private GMPlayerMovement playerMovement;
    private MacroLevelUpManager levelUpManager;
    private SpellManager spellManager;
    private RunesSystem runesSystem;

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

    [Inject]
    public void Construct(
        PlayersArmy playersArmy,
        GMPlayerMovement playerMovement,
        MacroLevelUpManager levelUpManager,
        SpellManager spellManager,
        RunesSystem runesSystem
        )
    {
        this.playersArmy = playersArmy;
        this.playerMovement = playerMovement;
        this.levelUpManager = levelUpManager;
        this.spellManager = spellManager;
        this.runesSystem = runesSystem;
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
        saveData.abilities = levelUpManager.Save();
        saveData.spells = spellManager.Save();
        saveData.runes = runesSystem.Save();

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

        playersArmy.Load(saveData.army);
        levelUpManager.Load(saveData.abilities);
        spellManager.Load(saveData.spells);
        runesSystem.Load(saveData.runes);

        playerMovement.Load(saveData.parameters);

        manager.LoadDataComplete("Player are loaded");
    }
}

