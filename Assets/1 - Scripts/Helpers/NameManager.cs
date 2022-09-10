using System;
using System.Collections.Generic;
using UnityEngine;

public static class NameManager
{
    public enum UnitsTypes
    {
        Militias,
        Rangers,
        Barbarians,
        Spearmen,
        Monks,
        Priests,
        Mercenaries,
        Paladins
    }

    public enum UnitsHouses
    {
        Kasarma,
        Avanpost,
        Tower,
        Sklep,
        Church
    }

    public enum UnitStats
    {
        Health,
        MagicAttack,
        PhysicAttack,
        MagicDefence,
        PhysicDefence,
        SpeedAttack,
        Size,

        CoinsPrice,
        FoodPrice,
        WoodPrice,
        IronPrice,
        StonePrice,
        MagicPrice,

    }

    public enum UnitsAbilities
    {
        Whip,  
        Garlic,
        Axe,
        Spear,
        Bible,
        Bow,
        Knife,
        Bottle
    }

    public enum EnemiesTypes
    {
        Bat,
        Zombie,
        Monster,
        Banshi,
        Fish,
        Werewolf,
        Mummy,
        Skeleton,
        Crab,
        Mantis
    }

    public enum EnemyAbilities 
    { 
        Empty
    }

    public enum ObjectPool
    {
        Enemy,
        DamageText,
        BonusExp,
        Torch,
        BonusGold,
        BloodSpot,
        EnemyDeath
    }

    public enum Animations
    {
        Idle, Walk, Attack
    }

    public enum AfterAnimation
    {
        Nothing, Destroy, SetDisable
    }

    public enum ObstacleTypes
    {
        Enemy,
        EnemyBoss,
        Obstacle,
        Torch,
        Tower
    }

    public enum BonusType
    {
        Health,
        Mana,
        Gold,
        TempExp,
        Exp,
        Other,
        Nothing
    }

    public enum ResourceType
    {
        Gold,
        Food,
        Stone,
        Wood,
        Iron,
        Magic,
        Nothing
    }

    public enum PlayersStats
    {
        Level,
        Health,
        Mana,
        Speed,
        SearchRadius,
        Defence,
        Infirmary,
        Luck,

        MovementDistance,
        HealthRegeneration,
        RadiusView,
        ExtraResourcesProduce,
        ExtraReward,

        Spell,
        NegativeCell,
        MedicAltar,

        MedicTry,
        Curiosity,
        Portal,
        ExtraResourcesReward,
        ManaRegeneration,
        HeroArmyToEnemy,

        GlobalExp
    }

    public enum Spells
    {
        SpeedUp,
        AttackUp,
        DoubleCrit,
        Shurikens,
        GoAway,
        AllBonuses,
        Healing,
        DoubleBonuses,
        WeaponSize,
        Maning,
        Immortal,
        EnemiesStop,
        DestroyEnemies,
        ExpToGold,
        ResurrectUnit
    }

    public enum BossSpells
    {
        InvertMovement = 0,
        Lightning = 1,
        ManningLess = 2
    }

    public enum BoostSender
    {
        Spell, Calendar
    }

    public enum BoostType
    {
        Hiring,
        ResourcesBilding,
        BattleExpirience,
        Altar,
        Luck,
        EnemyHealth,
        EnemyDefence,
        Movement,
        Regeneration,
        Mana
    }

    public enum UISlotTypes
    {
        Army, Reserve
    }


    #region GlobalMap

    public enum NeighborsDirection
    {
        NE, E, SE, SW, W, NW
    }

    public enum TypeOfObjectOnTheMap
    {
        PlayersCastle,
        NecromancerCastle,
        Castle,
        ResoursesFarm,
        ResoursesQuarry,
        ResoursesMine,
        ResoursesSawmill,
        Outpost,
        Camp,
        Altar,
        Portal,
        RoadPointer,
        Arena,
        Tomb,
        Resource,
        Enemy
    }

    public enum TypeOfObjectsOwner 
    { 
        Player, Enemy, Nobody
    }

    public enum ArmyStrength
    {
        Low, Middle, High, Extremely
    }

    #endregion

    #region Helpers

    public enum CursorView
    {
        Default,
        Battle,
        Movement,
        Action,
        Enter
    }


    #endregion
}
