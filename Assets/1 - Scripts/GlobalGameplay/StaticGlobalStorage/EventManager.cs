using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

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


    //calls when we update garrisons
    //
    //SUBSCRIBERS:
    // - ResourceBuildingUI
    //
    //ACTIVATION:
    // - Garrison
    //
    public delegate void UpdateSiegeTermEvent(Garrison garrison);
    public static event UpdateSiegeTermEvent UpdateSiegeTerm;
    public static void OnUpdateSiegeTermEvent(Garrison garrison) => UpdateSiegeTerm?.Invoke(garrison);
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
    public delegate void EnemyDestroyedEvent(EnemyController deadEnemy);
    public static event EnemyDestroyedEvent EnemyDestroyed;
    public static void OnEnemyDestroyedEvent(EnemyController deadEnemy) => EnemyDestroyed?.Invoke(deadEnemy);


    //calls when we destroy enemy
    //
    //SUBSCRIBERS:
    // - UnitController
    //
    //ACTIVATION:
    // - EnemyController
    //
    public delegate void KillEnemyByEvent(UnitsWeapon weapon);
    public static event KillEnemyByEvent KillEnemyBy;
    public static void OnKillEnemyByEvent(UnitsWeapon weapon) => KillEnemyBy?.Invoke(weapon);


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


    //calls when weapon is destroyed
    //
    //SUBSCRIBERS:
    // - WeaponStorage
    //
    //ACTIVATION:
    // - Weapon
    //
    public delegate void WeaponDestroyedEvent(Weapon weapon);
    public static event WeaponDestroyedEvent WeaponDestroyed;
    public static void OnWeaponDestroyedEvent(Weapon weapon) => WeaponDestroyed?.Invoke(weapon);

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


    //calls when week ends on Global Map
    //
    //SUBSCRIBERS:
    // Garrison
    //
    //ACTIVATION:
    // - CalendarManager
    //
    public delegate void WeekEndEvent();
    public static event WeekEndEvent WeekEnd;
    public static void OnWeekEndEvent() => WeekEnd?.Invoke();


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


    //calls when start we need Reset Garrisons
    //
    //SUBSCRIBERS:
    // - EnemyArmyOnTheMap
    //
    //ACTIVATION:
    // - EnemyArragement
    //
    public delegate void ResetGarrisonsEvent();
    public static event ResetGarrisonsEvent ResetGarrisons;
    public static void OnResetGarrisonsEvent() => ResetGarrisons?.Invoke();
    #endregion
}
