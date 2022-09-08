using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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



    //calls when we switch unit from army to reserve or back
    //
    //SUBSCRIBERS:
    // - PlayersArmy
    //
    //ACTIVATION:
    // - ArmySlot
    //
    public delegate void SwitchUnitEvent(bool mode, Unit unit);
    public static event SwitchUnitEvent SwitchUnit;
    public static void OnSwitchUnitEvent(bool mode, Unit unit) => SwitchUnit?.Invoke(mode, unit);



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
    public delegate void WeLostOneUnitEvent(UnitsTypes unitType, int quantity);
    public static event WeLostOneUnitEvent WeLostOneUnit;
    public static void OnWeLostOneUnitEvent(UnitsTypes unitType, int quantity) => WeLostOneUnit?.Invoke(unitType, quantity);



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



    // 1 unit boost system: calls when we boost some stat of unit from anywhere
    //
    //SUBSCRIBERS:
    // - UnitBoostManager
    //
    //ACTIVATION:
    // - SpellLibrary
    //
    public delegate void BoostUnitStatEvent(bool boostAll, bool addBoost, BoostSender sender, UnitStats stat, float value, UnitsTypes types = UnitsTypes.Militias);
    public static event BoostUnitStatEvent BoostUnitStat;
    public static void OnBoostUnitStatEvent(bool boostAll, bool addBoost, BoostSender sender, UnitStats stat, float value, UnitsTypes types = UnitsTypes.Militias) => BoostUnitStat?.Invoke(boostAll, addBoost, sender, stat, value, types);
    #endregion


    #region PLAYER
    //calls when we should switch between globalPlayer and battlePlayer
    //
    //SUBSCRIBERS:
    // - BattleMap
    // - PlayerManager
    // - PlayerStats
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
    public delegate void ChangePlayerEvent(bool mode);
    public static event ChangePlayerEvent ChangePlayer;
    public static void OnChangePlayerEvent(bool mode) => ChangePlayer?.Invoke(mode);



    // 1 player boost system: calls when we boost some stat from anywhere
    //
    //SUBSCRIBERS:
    // - PlayerBoostManager
    //
    //ACTIVATION:
    // - SpellLibrary
    //
    public delegate void BoostStatEvent(bool mode, BoostSender sender, PlayersStats stat, float value);
    public static event BoostStatEvent BoostStat;
    public static void OnBoostStatEvent(bool mode, BoostSender sender, PlayersStats stat, float value) => BoostStat?.Invoke(mode, sender, stat, value);



    // 2 player boost system: give common boost to PlayerStats
    //
    //SUBSCRIBERS:
    // - PlayerStats
    //
    //ACTIVATION:
    // - PlayerBoostManager
    //
    public delegate void SetBoostToStatEvent(PlayersStats stats, float value);
    public static event SetBoostToStatEvent SetBoostToStat;
    public static void OnSetBoostToStatEvent(PlayersStats stats, float value) => SetBoostToStat?.Invoke(stats, value);


    // 3 player boost system: give new boosted stats to listeners
    //
    //SUBSCRIBERS:
    // - BattleArmyController
    // - GMPlayerMovement
    //
    //ACTIVATION:
    // - PlayerStats
    //
    public delegate void NewBoostedStatEvent(PlayersStats stats, float value);
    public static event NewBoostedStatEvent NewBoostedStat;
    public static void OnNewBoostedStatEvent(PlayersStats stats, float value) => NewBoostedStat?.Invoke(stats, value);

    //calls when we need update some stat of player
    //
    //SUBSCRIBERS:
    // - HeroController
    // - 
    //ACTIVATION:
    // - PlayerStats
    //
    public delegate void SetStartPlayerStatEvent(PlayersStats stats, float value);
    public static event SetStartPlayerStatEvent SetStartPlayerStat;
    public static void OnSetStartPlayerStatEvent(PlayersStats stats, float value) => SetStartPlayerStat?.Invoke(stats, value);



    //calls when we get new temp level
    //
    //SUBSCRIBERS:
    //
    //ACTIVATION:
    // - HeroController
    //
    public delegate void UpgradeTempLevelEvent(float value);
    public static event UpgradeTempLevelEvent UpgradeTempLevel;
    public static void OnUpgradeTempLevelEvent(float value) => UpgradeTempLevel?.Invoke(value);



    //calls when we change Mana
    //
    //SUBSCRIBERS:
    // - BattleUIManager
    // - gmInterface
    //
    //ACTIVATION:
    // - HeroController
    //
    public delegate void UpgradeStatCurrentValueEvent(PlayersStats stat, float maxValue, float currentValue);
    public static event UpgradeStatCurrentValueEvent UpgradeStatCurrentValue;
    public static void OnUpgradeStatCurrentValueEvent(PlayersStats stat, float maxValue, float currentValue) => UpgradeStatCurrentValue?.Invoke(stat, maxValue, currentValue);



    //calls when we change Gold
    //
    //SUBSCRIBERS:
    // - BattleUIManager
    // - GMInterface
    //
    //ACTIVATION:
    // - ResourcesManager
    //
    public delegate void UpgradeResourcesEvent();
    public static event UpgradeResourcesEvent UpgradeResources;
    public static void OnUpgradeResourcesEvent() => UpgradeResources?.Invoke();
    #endregion


    #region Battle
    //calls when we finish or crash the battle
    //
    //SUBSCRIBERS:
    // - BonusController
    // - ObjectsPoolManager
    //
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



    //calls when battle starts, we send common enemies quantity
    //
    //SUBSCRIBERS:
    // - BattleUIManager
    //
    //ACTIVATION:
    // - EnemySpawner
    //
    public delegate void EnemiesCountEvent(int count);
    public static event EnemiesCountEvent EnemiesCount;
    public static void OnEnemiesCountEvent(int count) => EnemiesCount?.Invoke(count);


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
    //calls when we switch between global mode and battle mode
    //IT USES ONCE! Don't subscribe any script more!
    //
    //SUBSCRIBERS:
    // - CameraManager
    //
    //ACTIVATION:
    // - GlobalStorage
    //
    public delegate void ChangePlayModeEvent(bool mode);
    public static event ChangePlayModeEvent ChangePlayMode;
    public static void OnChangePlayModeEvent(bool mode) => ChangePlayMode?.Invoke(mode);


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

    #endregion
}
