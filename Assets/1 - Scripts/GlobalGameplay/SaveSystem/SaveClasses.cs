using System;
using UnityEngine;
using System.Collections.Generic;
using static Enums;

[Serializable]
public class Vec3
{
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public Vec3(Vector3 oldVector)
    {
        x = oldVector.x;
        y = oldVector.y;
        z = oldVector.z;
    }
}

[Serializable]
public class fogCell
{
    public float x = 0;
    public float y = 0;

    public fogCell(Vector3Int oldVector)
    {
        x = oldVector.x;
        y = oldVector.y;
    }
}

[Serializable]
public class GMTileManagerSD
{
    public List<Vec3> arenaPoint = new List<Vec3>();

    public List<Vec3> castlesPoints = new List<Vec3>();
    public AI_SD aiData = new AI_SD();

    public List<Vec3> altarsPoints = new List<Vec3>();

    public List<Vec3> tombsPoints = new List<Vec3>();
    public List<TombsSD> tombsData = new List<TombsSD>();

    public PortalsSD portalsData = new PortalsSD();

    public List<Vec3> resourcesPoints = new List<Vec3>();
    public List<ResBuildingSD> resBuildings = new List<ResBuildingSD>();

    public List<Vec3> campsPoints = new List<Vec3>();

    public List<Vec3> boxesPoints = new List<Vec3>();
    public List<Reward> boxesRewards = new List<Reward>();

    public List<fogCell> fogFreeCells = new List<fogCell>();
}


[Serializable]
public class AI_SD
{
    public List<VassalSD> vassalsList = new List<VassalSD>();
    public List<Vec3> activeCastleList = new List<Vec3>();
}


[Serializable]
public class VassalSD
{
    public Vec3 castlePosition;
    public bool isCastleDestroyed = false;
    public bool isCastleReady = false;
    public int currentRest;

    public EnemySD vassalArmy;

    public bool isVassalActive = false;
    public bool isFlipped = false;
    public Vec3 vassalPosition;

    public bool shouldIContinueAction = false;
    public bool aggressiveMode = false;

    public int currentTriesToGetTarget;

    public Vec3 finishCell = new Vec3(Vector3.zero);

    public AITargetType currentTarget = AITargetType.Rest;
    public AIActions currentAction = AIActions.End;
    public List<AIActions> currentActionsList = new List<AIActions>();

    public Vec3 currentSiegeTargetPosition = new Vec3(Vector3.zero);
}


[Serializable]
public class PortalsSD
{
    public List<Vec3> unlockedPortals = new List<Vec3>();
    public Vec3 backPosition = new Vec3(Vector3.zero);
}

[Serializable]
public class TombsSD
{
    public Vec3 position;
    public bool status;
    public Reward reward;
    public Spells spell;
    public EnemySD enemyGarrison;
}


[Serializable]
public class ResBuildingSD
{
    public TypeOfObjectsOwner owner = TypeOfObjectsOwner.Nobody;
    public bool isVisited = false;

    public ResourceBuildings buildingType;

    public bool isSiege = false;
    public Vec3 position;

    public List<string> upgrades = new List<string>();

    public int currentSiegeDays;
    public List<UnitsTypes> garrisonTypes = new List<UnitsTypes>();
    public List<int> garrisonAmounts = new List<int>();
}


[Serializable]
public class MapBonusManagerSD
{
    public List<Vec3> heapsPoints = new List<Vec3>();
    public List<Reward> heapsRewards = new List<Reward>();
}


[Serializable]
public class EnemySD
{
    public Vec3 position = new Vec3(Vector3.zero);
    public Army army;
    public TypeOfArmy typeOfArmy;
    public bool isEnemyGarrison = false;
    public Vec3 color = new Vec3(Vector3.zero);
}


[Serializable]
public class EnemySDWrapper
{
    public List<EnemySD> enemyList = new List<EnemySD>();
}


[Serializable]
public class CalendarSD
{
    public int day;
    public int week;
    public int month;
    public int year;

    public int daysPassed;
    public int daysLeft;
    public int weeksPassed;
    public int monthsPassed;
    public int yearsPassed;

    public List<string> decadeList = new List<string>();
    public int currentDecadeIndex;
}


[Serializable]
public class CameraSD
{
    public Vec3 position;
    public Vec3 rotation;
    public float rotationAngle;
    public float zoom;
    public float cameraSize;
}


[Serializable]
public class InfirmarySD
{
    public List<UnitsTypes> units = new List<UnitsTypes>();
    public List<InjuredUnitData> quantity = new List<InjuredUnitData>();
}


[Serializable]
public class MarketSD
{
    public bool isMarketContainer = false;

    public float currentInflation = 0;
}


[Serializable]
public class UnitCenterSD
{
    public bool isGrowthContainer = false;

    public List<HiringAmount> potentialAmounts = new List<HiringAmount>();
    public List<HiringAmount> growthAmounts = new List<HiringAmount>();
}


[Serializable]
public class SanctuarySD
{
    public bool isSanctContainer = false;

    public List<ResourceType> resources = new List<ResourceType>();
    public List<float> amounts = new List<float>();
}


[Serializable]
public class HFBuildingsSD
{
    public List<CastleBuildings> buildedBuildings = new List<CastleBuildings>();
    public List<int> buildedBuildingsLevels = new List<int>();

    public List<CastleBuildings> buildingsInProgress = new List<CastleBuildings>();
    public List<ConstructionTime> constructionsTimes = new List<ConstructionTime>();

    public List<object> specialBuildingsSD = new List<object>();
}


[Serializable]
public class HeroFortressSD
{
    public int marketDays = 0;
    public int seals = 0;

    public bool isHeroInside = false;
    public bool isHeroVisitedOnThisWeek = false;

    public HFBuildingsSD specialBuildingsSD = new HFBuildingsSD();
}


[Serializable]
public class PlayerSD
{
    public PlayersMovementSD parameters;
    public PlayersArmySD army;
    public PlayersLevelUpSD abilities;
    public PlayersSpells spells;
    public PlayersRunes runes;
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
}

[Serializable]
public class PlayersMovementSD
{
    public bool flipHero = false;
    public float movementPoints = 0;
    public bool isExtraMovementWaisted = false;
    public Vec3 position;
}

[Serializable]
public class AbilityData
{
    public PlayersStats abilitySerie;
    public int level;
}

[Serializable]
public class PlayersLevelUpSD
{
    public float currentLevel;
    public float currentExp;
    public int abilityPoints;

    public List<AbilityData> openedAbilities;
}

[Serializable]
public class PlayersSpells
{
    public List<SpellData> spellsLevels = new List<SpellData>();
    public List<Spells> readySpells = new List<Spells>();
    public List<Spells> findedSpells = new List<Spells>();
    public List<Spells> spellsInStorage = new List<Spells>();
    public List<Spells> spellsForBattle = new List<Spells>();
}

[Serializable]
public class SpellData
{
    public Spells spell;
    public int level;
}