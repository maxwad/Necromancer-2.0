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
        EnemyDeath,
        EnemyOnTheMap,
        ResourceOnTheMap,
        BoxOnTheMap,
        BonusText,
        DeltaCost

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
        Exp, 
        Mana,
        Health
    }

    public enum PlayersStats
    {
        Level, 
        Health, 
        Mana, 
        Speed,//later
        SearchRadius,//later
        Defence,
        Infirmary,
        Luck,

        MovementDistance,
        HealthRegeneration,
        RadiusView,
        ExtraResourcesProduce,//later
        ExtraBoxReward,
        ExtraExpReward,

        Spell,//later
        NegativeCell,//later
        MedicAltar,//later
        DoubleBonusFromBox,
        ExtraMovementPoints,
        Fog,
        HealthBigRegeneration,
        ManaBigRegeneration,
        Learning, 

        MedicTry,//later
        Curiosity,
        Portal,
        ExtraAfterBattleReward,
        ManaRegeneration,
        HeroArmyToEnemy,//later
        InfirmaryTime
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

    public enum TypeOfSpell
    {
        Attack, Defence, Bonus
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

    public enum BoostFromType
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
        Boost,
        Tool,
        Warning
    }

    #endregion
}
