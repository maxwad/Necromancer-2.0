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

    public enum UnitStatus
    {
        Army, Store, Nowhere, Infirmary
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
        Bottle,
        Notning
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
        BattleBonus,
        Torch,
        BonusGold,
        BloodSpot,
        EnemyDeath,
        EnemyOnTheMap,
        ResourceOnTheMap,
        BoxOnTheMap,
        BonusText,
        DeltaCost,
        Rune,
        BattleEffect,
        ManaSpot,
        ManaDeath,
        Shuriken,
        Fireball,
        Ligthning,
        Cannonball,
        SquadSlot,
        SpellEffect,
        PushCircle

    }

    public enum BossWeapons
    {
        Ligthning, Fireball, ManningLess, Inverting
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
        Exp, 
        Mana,
        Health
    }

    public enum PlayersStats
    {
        Level = 0, 
        Health = 1001, 
        Mana = 1002, 
        Speed = 7,
        SearchRadius = 1003,
        Defence = 15,
        Infirmary = 1004,
        Luck = 5,

        MovementDistance = 30,
        HealthRegeneration = 27,
        RadiusView = 1005,
        ExtraResourcesProduce = 25,//later
        ExtraBoxReward = 1006,
        ExtraExpReward = 31,

        Spell = 1007,//later
        NegativeCell = 1008,
        MedicAltar = 26,//later
        DoubleBonusFromBox = 1009,
        ExtraMovementPoints = 1010,
        Fog = 1011,
        HealthBigRegeneration = 1012,
        ManaBigRegeneration = 1013,
        Learning = 1014,

        MedicTry = 1015,//later
        Curiosity = 1016,
        Portal = 1017,
        ExtraAfterBattleReward = 1018,
        ManaRegeneration = 29,
        HeroArmyToEnemy = 1019,//later
        InfirmaryTime = 1020
    }

    public enum RunesType
    {
        PhysicAttack = 1,
        MagicAttack = 2,
        PhysicDefence = 3,
        MagicDefence = 4,
        CriticalDamage = 5,
        BossDamade = 6,
        MovementSpeed = 7,
        BonusAmount = 8,
        BonusRadius = 9,
        BonusOpportunity = 10,
        WeaponSpeed = 11,
        WeaponSize = 12,
        CoolDown = 13,
        Exp = 14,
        EnemyPhysicAttack = 16,
        EnemyMagicAttack = 17,
        EnemyPhysicDefence = 18,
        EnemyMagicDefence = 19,
        EnemyMovementSpeed = 20,
        EnemyCoolDown = 21,
        EnemyHealth = 22,
        SpellReloading = 23,
        SpellActionTime = 24,
        ExtraResourcesProduce = 25,
        Altar = 26,
        HealthRegeneration = 27,
        Hiring = 28,
        ManaRegeneration = 29,
        MovementPoints = 30,
        ExpAfterBattle = 31
    }

    public enum BoostType
    {
        PhysicAttack = 1,
        MagicAttack = 2,
        PhysicDefence = 3,
        MagicDefence = 4,
        CriticalDamage = 5,
        BossDamade = 6,
        MovementSpeed = 7,
        BonusAmount = 8,
        BonusRadius = 9,
        BonusOpportunity = 10,
        WeaponSpeed = 11,
        WeaponSize = 12,
        CoolDown = 13,
        Exp = 14,
        HeroDefence = 15,
        Nothing = 0,
        SpellReloading = 23,
        SpellActionTime = 24,

        EnemyPhysicAttack = 16,
        EnemyMagicAttack = 17,
        EnemyPhysicDefence = 18,
        EnemyMagicDefence = 19,
        EnemyMovementSpeed = 20,
        EnemyCoolDown = 21,
        EnemyHealth  = 22,

        ExtraResourcesProduce = 25,
        Altar = 26,
        HealthRegeneration = 27,
        Hiring = 28,
        ManaRegeneration = 29,
        MovementPoints = 30,
        ExpAfterBattle = 31
    }

    public enum Spells
    {
        SpeedUp = 0,
        AttackUp = 1,
        DoubleCrit = 2,
        Shurikens = 3,
        GoAway = 4,
        AllBonuses = 5,
        Healing = 6,
        DoubleBonuses = 7,
        WeaponSize = 8,
        Maning = 9,
        Immortal = 10,
        EnemiesStop = 11,
        DestroyEnemies = 12,
        ExpToGold = 13,
        ResurrectUnit = 14
    }

    public enum PreSpells
    {
        Shurikens = 3,
        GoAway = 4,
        DestroyEnemies = 12,
        ExpToGold = 13
    }

    public enum TypeOfSpell
    {
        Attack, Defence, Bonus
    }

    public enum BossSpells
    {
        InvertMovement = 0,
        Lightning = 1,
        ManningLess = 2,
        Fireball = 3
    }

    public enum BoostSender
    {
        Spell, Calendar, Rune, EnemySystem
    }

    public enum BoostEffect
    {
        PlayerBattle, Global, EnemiesBattle
    }

    public enum EffectType
    {
        Rune, Spell, Enemy
    }

    public enum UISlotTypes
    {
        Army, Store
    }


    public enum BattleVisualEffects
    {
        Death, Resurection, LevelUp
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
        Enemy,
        BoxBonus,
        Player
    }

    public enum TypeOfObjectsOwner 
    { 
        Player, Enemy, Nobody
    }

    public enum ArmyStrength
    {
        Low = 1, Middle = 2, High = 3, Extremely = 4
    }

    public enum TypeOfArmy
    {
        OnTheMap, InCastle, InTomb, InOutpost
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

    public enum StatBoostType
    {
        Bool, Step, Percent, Value
    }

    public enum PlayersWindow
    {
        PlayersArmy,
        Battle,
        Tomb,
        Spells,
        MicroLevelUp,
        MacroLevelUp,
        Nothing
    }

    public enum TipsType
    {
        Unit,
        Enemy,
        Hero,
        Skill,
        Spell,
        Rune,
        Tool,
        Warning
    }

    #endregion
}
