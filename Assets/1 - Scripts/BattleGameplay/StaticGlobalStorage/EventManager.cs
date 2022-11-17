using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public static class EventManager
{
    #region UNITS

    //calls when we had finished to create all units
    //
    //SUBSCRIBERS:
    // - PlayersArmy
    //
    //ACTIVATION:
    // - UnitManager
    //
    public delegate void AllUnitsIsReadyEvent();
    public static event AllUnitsIsReadyEvent AllUnitsIsReady;
    public static void OnAllUnitsIsReadyEvent() => AllUnitsIsReady?.Invoke();



    ////calls when we switch unit from army to reserve or back
    ////
    ////SUBSCRIBERS:
    //// - PlayersArmy
    ////
    ////ACTIVATION:
    //// - ArmySlot
    ////
    //public delegate void SwitchUnitEvent(bool mode, Unit unit);
    //public static event SwitchUnitEvent SwitchUnit;
    //public static void OnSwitchUnitEvent(bool mode, Unit unit) => SwitchUnit?.Invoke(mode, unit);



    //calls when we lose 1 unit from squad
    //
    //SUBSCRIBERS:
    // - PlayersArmy
    // - BattleArmyController
    // - InfirmaryManager
    // 
    //ACTIVATION:
    // - UnitController
    //
    public delegate void WeLostOneUnitEvent(UnitsTypes unitType);
    public static event WeLostOneUnitEvent WeLostOneUnit;
    public static void OnWeLostOneUnitEvent(UnitsTypes unitType) => WeLostOneUnit?.Invoke(unitType);



    //calls when we need to update UI from Infirmary
    //
    //SUBSCRIBERS:
    // - BattleUIManager
    // 
    //ACTIVATION:
    // - InfirmaryManager
    //
    public delegate void UpdateInfirmaryUIEvent( float quantity, float capacity);
    public static event UpdateInfirmaryUIEvent UpdateInfirmaryUI;
    public static void OnUpdateInfirmaryUIEvent(float quantity, float capacity) => UpdateInfirmaryUI?.Invoke(quantity, capacity);



    //calls when need to resurrect unit from Infirmary
    //
    //SUBSCRIBERS:
    // - InfirmaryManager
    // 
    //ACTIVATION:
    // - SpellLibrary
    //
    public delegate void RemoveUnitFromInfirmaryEvent(bool mode, bool order, float quantity);
    public static event RemoveUnitFromInfirmaryEvent RemoveUnitFromInfirmary;
    public static void OnRemoveUnitFromInfirmaryEvent(bool mode, bool order, float quantity) => RemoveUnitFromInfirmary?.Invoke(mode, order, quantity);



    //calls when need to add resurrected unit to hero's army
    //
    //SUBSCRIBERS:
    // - ...
    // 
    //ACTIVATION:
    // - InfirmaryManager
    //
    public delegate void ResurrectUnitEvent(UnitsTypes unitType);
    public static event ResurrectUnitEvent ResurrectUnit;
    public static void OnResurrectUnitEvent(UnitsTypes unitType) => ResurrectUnit?.Invoke(unitType);



    //// 1 unit boost system: calls when we boost some stat of unit from anywhere
    ////
    ////SUBSCRIBERS:
    //// - UnitBoostManager
    ////
    ////ACTIVATION:
    //// - SpellLibrary
    ////
    //public delegate void BoostUnitStatEvent(bool boostAll, bool addBoost, BoostSender sender, UnitStats stat, float value, UnitsTypes types = UnitsTypes.Militias);
    //public static event BoostUnitStatEvent BoostUnitStat;
    //public static void OnBoostUnitStatEvent(bool boostAll, bool addBoost, BoostSender sender, UnitStats stat, float value, UnitsTypes types = UnitsTypes.Militias) => BoostUnitStat?.Invoke(boostAll, addBoost, sender, stat, value, types);


    //calls when need to add resurrected unit to hero's army
    //
    //SUBSCRIBERS:
    // - ...
    // 
    //ACTIVATION:
    // - PlayersArmy
    //
    public delegate void UpdateSquadEvent(UnitsTypes type);
    public static event UpdateSquadEvent UpdateSquad;
    public static void OnUpdateSquadEvent(UnitsTypes type) => UpdateSquad?.Invoke(type);


    //calls when want to switch squads in Army
    //
    //SUBSCRIBERS:
    // - SquadSlotPlacing
    // 
    //ACTIVATION:
    // - SquadSlotPlacing
    //
    public delegate void SwitchSlotsEvent(int index, UnitStatus place, GameObject slot);
    public static event SwitchSlotsEvent SwitchSlots;
    public static void OnSwitchSlotsEvent(int index, UnitStatus place, GameObject slot) => SwitchSlots?.Invoke(index, place, slot);
    #endregion


    #region PLAYER
    //calls when we should switch between globalPlayer and battlePlayer
    //
    //SUBSCRIBERS:
    // - BattleMap
    // - PlayerManager
    // - PlayerStats
    // - PlayersArmy
    // - BattleUIManager
    // - HeroController
    // - SpellLibrary
    // - BonusManager
    // - WeaponMovement
    // - GMInterface
    // 
    //ACTIVATION:
    // - CameraManager
    //
    public delegate void SwitchPlayerEvent(bool mode);
    public static event SwitchPlayerEvent SwitchPlayer;
    public static void OnSwitchPlayerEvent(bool mode) => SwitchPlayer?.Invoke(mode);



    //// 1 player boost system: calls when we boost some stat from anywhere
    ////
    ////SUBSCRIBERS:
    //// - PlayerBoostManager
    ////
    ////ACTIVATION:
    //// - SpellLibrary
    ////
    //public delegate void BoostStatEvent(bool mode, BoostSender sender, PlayersStats stat, float value);
    //public static event BoostStatEvent BoostStat;
    //public static void OnBoostStatEvent(bool mode, BoostSender sender, PlayersStats stat, float value) => BoostStat?.Invoke(mode, sender, stat, value);



    //// 2 player boost system: give common boost to PlayerStats
    ////
    ////SUBSCRIBERS:
    //// - PlayerStats
    ////
    ////ACTIVATION:
    //// - PlayerBoostManager
    ////
    //public delegate void UpgrateStatEvent(PlayersStats stats, float value);
    //public static event UpgrateStatEvent UpgrateStat;
    //public static void OnUpgrateStatEvent(PlayersStats stats, float value) => UpgrateStat?.Invoke(stats, value);


    // universal battle boost system: give common boost to Hero and units
    //
    //SUBSCRIBERS:
    // - BattleArmyController
    //
    //ACTIVATION:
    // - BattleBoostManager
    //
    public delegate void SetBattleBoostEvent(BoostType boost, float value);
    public static event SetBattleBoostEvent SetBattleBoost;
    public static void OnSetBattleBoostEvent(BoostType boost, float value) => SetBattleBoost?.Invoke(boost, value);


    // send to BattleUI one boost effect
    //
    //SUBSCRIBERS:
    // - BattleUIManager
    //
    //ACTIVATION:
    // - BattleBoostManager
    //
    public delegate void ShowBoostEffectEvent(BoostSender sender, BoostType boost, float value);
    public static event ShowBoostEffectEvent ShowBoostEffect;
    public static void OnShowBoostEffectEvent(BoostSender sender, BoostType boost, float value) => ShowBoostEffect?.Invoke(sender, boost, value);

    //// 3 player boost system: give new boosted stats to listeners
    ////
    ////SUBSCRIBERS:
    //// - BattleArmyController
    //// - GMPlayerMovement
    ////
    ////ACTIVATION:
    //// - PlayerStats
    ////
    //public delegate void NewBoostedStatEvent(PlayersStats stats, float value);
    //public static event NewBoostedStatEvent NewBoostedStat;
    //public static void OnNewBoostedStatEvent(PlayersStats stats, float value) => NewBoostedStat?.Invoke(stats, value);

    //calls when we need update some stat of player
    //
    //SUBSCRIBERS:
    // - HeroController
    // - ResourcesManager
    // - 
    //ACTIVATION:
    // - PlayerStats
    //
    public delegate void SetNewPlayerStatEvent(PlayersStats stats, float value);
    public static event SetNewPlayerStatEvent SetNewPlayerStat;
    public static void OnSetNewPlayerStatEvent(PlayersStats stats, float value) => SetNewPlayerStat?.Invoke(stats, value);



    //calls when we get new level
    //
    //SUBSCRIBERS:
    // - RunesWindow
    //ACTIVATION:
    // - MacroLevelUpManager
    //
    public delegate void UpgradeLevelEvent(float value);
    public static event UpgradeLevelEvent UpgradeLevel;
    public static void OnUpgradeLevelEvent(float value) => UpgradeLevel?.Invoke(value);



    ////calls when we change Mana
    ////
    ////SUBSCRIBERS:
    //// - gmInterface
    ////
    ////ACTIVATION:
    //// - HeroController
    //// - GMPlayerMovement
    ////
    //public delegate void UpgradeStatCurrentValueEvent(PlayersStats stat, float maxValue, float currentValue);
    //public static event UpgradeStatCurrentValueEvent UpgradeStatCurrentValue;
    //public static void OnUpgradeStatCurrentValueEvent(PlayersStats stat, float maxValue, float currentValue) => UpgradeStatCurrentValue?.Invoke(stat, maxValue, currentValue);



    //calls when we change Gold
    //
    //SUBSCRIBERS:
    // - BattleUIManager
    // - GMInterface
    //
    //ACTIVATION:
    // - ResourcesManager
    //
    public delegate void UpgradeResourceEvent(ResourceType resource, float value);
    public static event UpgradeResourceEvent UpgradeResource;
    public static void OnUpgradeResourceEvent(ResourceType resource, float value) => UpgradeResource?.Invoke(resource, value);
    #endregion


    #region Battle
    //calls when we finish or crash the battle
    //
    //SUBSCRIBERS:
    // - BonusController
    // - All objectpool objects
    //ACTIVATION:
    // - BattleMap
    //
    public delegate void EndOfBattleEvent(); 
    public static event EndOfBattleEvent EndOfBattle;
    public static void OnEndOfBattleEvent() => EndOfBattle?.Invoke();



    //calls when we destroy obstacle
    //
    //SUBSCRIBERS:
    // - BattleMap
    //
    //ACTIVATION:
    // - HealthObjectStats
    //
    public delegate void ObstacleDestroyedEvent(GameObject objectOnMap);
    public static event ObstacleDestroyedEvent ObstacleDestroyed;
    public static void OnObstacleDestroyedEvent(GameObject objectOnMap) => ObstacleDestroyed?.Invoke(objectOnMap);


    //calls when we destroy enemy
    //
    //SUBSCRIBERS:
    // - BattleMap
    // - BattleUIManager
    //
    //ACTIVATION:
    // - EnemyController
    //
    public delegate void EnemyDestroyedEvent(GameObject objectOnMap);
    public static event EnemyDestroyedEvent EnemyDestroyed;
    public static void OnEnemyDestroyedEvent(GameObject objectOnMap) => EnemyDestroyed?.Invoke(objectOnMap);


    //calls when we destroy enemy
    //
    //SUBSCRIBERS:
    // - UnitController
    //
    //ACTIVATION:
    // - EnemyController
    //
    public delegate void KillEnemyByEvent(UnitsAbilities weapon);
    public static event KillEnemyByEvent KillEnemyBy;
    public static void OnKillEnemyByEvent(UnitsAbilities weapon) => KillEnemyBy?.Invoke(weapon);


    //calls when we rich max level in the battle
    //
    //SUBSCRIBERS:
    // - BonusManager
    //
    //ACTIVATION:
    // - HeroController
    //
    public delegate void ExpEnoughEvent(bool mode);
    public static event ExpEnoughEvent ExpEnough;
    public static void OnExpEnoughEvent(bool mode) => ExpEnough?.Invoke(mode);



    ////calls when battle starts, we send common enemies quantity
    ////
    ////SUBSCRIBERS:
    //// - BattleUIManager
    ////
    ////ACTIVATION:
    //// - EnemySpawner
    ////
    //public delegate void EnemiesCountEvent(int count, BossData[] bosses);
    //public static event EnemiesCountEvent EnemiesCount;
    //public static void OnEnemiesCountEvent(int count, BossData[] bosses) => EnemiesCount?.Invoke(count, bosses);


    //calls when player win the battle
    //
    //SUBSCRIBERS:
    // - BonusController
    //
    //ACTIVATION:
    // - BattleUIManager
    //
    public delegate void VictoryEvent();
    public static event VictoryEvent Victory;
    public static void OnVictoryEvent() => Victory?.Invoke();


    //calls when player lose the battle
    //
    //SUBSCRIBERS:
    // - BonusController
    //
    //ACTIVATION:
    // - BattleManager
    //
    public delegate void DefeatEvent();
    public static event DefeatEvent Defeat;
    public static void OnDefeatEvent() => Defeat?.Invoke();

    #endregion


    #region Bonuses

    //calls when we pick up some bonus
    //
    //SUBSCRIBERS:
    // - HeroController
    // - ResourcesManager
    //
    //ACTIVATION:
    // - BonusController
    // - SpellLibrary
    //
    public delegate void BonusPickedUpEvent(BonusType type, float value);
    public static event BonusPickedUpEvent BonusPickedUp;
    public static void OnBonusPickedUpEvent(BonusType type, float value) => BonusPickedUp?.Invoke(type, value);

    #endregion


    #region Resourses

    //calls when we pick up some resourses
    //
    //SUBSCRIBERS:
    // - ResourcesManager
    //
    //ACTIVATION:
    // - 
    //
    public delegate void ResourcePickedUpEvent(ResourceType type, float value);
    public static event ResourcePickedUpEvent ResourcePickedUp;
    public static void OnResourcePickedUpEvent(ResourceType type, float value) => ResourcePickedUp?.Invoke(type, value);
    #endregion


    #region OTHER
    ////calls when we switch between global mode and battle mode
    ////IT USES ONCE! Don't subscribe any script more!
    ////
    ////SUBSCRIBERS:
    //// - CameraManager
    ////
    ////ACTIVATION:
    //// - GlobalStorage
    ////
    //public delegate void ChangePlayModeEvent(bool mode);
    //public static event ChangePlayModeEvent ChangePlayMode;
    //public static void OnChangePlayModeEvent(bool mode) => ChangePlayMode?.Invoke(mode);


    //calls when we create spell Immortal
    //
    //SUBSCRIBERS:
    // - UnitController
    //
    //ACTIVATION:
    // - SpellLibrary
    //
    public delegate void SpellImmortalEvent(bool mode);
    public static event SpellImmortalEvent SpellImmortal;
    public static void OnSpellImmortalEvent(bool mode) => SpellImmortal?.Invoke(mode);


    //calls when we end day on Global Map
    //
    //SUBSCRIBERS:
    // - GMPlayerMovement
    //
    //ACTIVATION:
    // - CalendarManager
    //
    public delegate void NewMoveEvent();
    public static event NewMoveEvent NewMove;
    public static void OnNewMoveEvent() => NewMove?.Invoke();


    //calls when start new month on Global Map
    //
    //SUBSCRIBERS:
    // - EnemyManager
    //
    //ACTIVATION:
    // - CalendarManager
    //
    public delegate void NewMonthEvent();
    public static event NewMonthEvent NewMonth;
    public static void OnNewMonthEvent() => NewMonth?.Invoke();

    //calls when start new week on Global Map
    //
    //SUBSCRIBERS:
    // - EnemyManager
    //
    //ACTIVATION:
    // - CalendarManager
    //
    public delegate void NewWeekEvent(int counter);
    public static event NewWeekEvent NewWeek;
    public static void OnNewWeekEvent(int counter) => NewWeek?.Invoke(counter);

    #endregion
}
