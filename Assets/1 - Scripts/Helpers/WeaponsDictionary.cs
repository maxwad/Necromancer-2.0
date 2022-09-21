using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class WeaponsDictionary : MonoBehaviour
{
    [HideInInspector] public static WeaponsDictionary instance;

    private List<Dictionary<UnitsAbilities, string>> abilities = new List<Dictionary<UnitsAbilities, string>>();

    [Header("Whip")]
    [SerializeField] private string WhipLvl_1; 
    [SerializeField] private string WhipLvl_2; 
    [SerializeField] private string WhipLvl_3;

    [Header("Garlic")]
    [SerializeField] private string GarlicLvl_1;
    [SerializeField] private string GarlicLvl_2;
    [SerializeField] private string GarlicLvl_3;

    [Header("Axe")]
    [SerializeField] private string AxeLvl_1;
    [SerializeField] private string AxeLvl_2;
    [SerializeField] private string AxeLvl_3;

    [Header("Spear")]
    [SerializeField] private string SpearLvl_1;
    [SerializeField] private string SpearLvl_2;
    [SerializeField] private string SpearLvl_3;

    [Header("Bible")]
    [SerializeField] private string BibleLvl_1;
    [SerializeField] private string BibleLvl_2;
    [SerializeField] private string BibleLvl_3;

    [Header("Bow")]
    [SerializeField] private string BowLvl_1;
    [SerializeField] private string BowLvl_2;
    [SerializeField] private string BowLvl_3;

    [Header("Knife")]
    [SerializeField] private string KnifeLvl_1;
    [SerializeField] private string KnifeLvl_2;
    [SerializeField] private string KnifeLvl_3;

    [Header("Bottle")]
    [SerializeField] private string BottleLvl_1;
    [SerializeField] private string BottleLvl_2;
    [SerializeField] private string BottleLvl_3;


    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        abilities.Add(
            new Dictionary<UnitsAbilities, string>() { 

                [UnitsAbilities.Whip]   = WhipLvl_1,
                [UnitsAbilities.Garlic] = GarlicLvl_1,
                [UnitsAbilities.Axe]    = AxeLvl_1,
                [UnitsAbilities.Spear]  = SpearLvl_1,
                [UnitsAbilities.Bible]  = BibleLvl_1,
                [UnitsAbilities.Bow]    = BowLvl_1,
                [UnitsAbilities.Knife]  = KnifeLvl_1,
                [UnitsAbilities.Bottle] = BottleLvl_1
            });

        abilities.Add(
            new Dictionary<UnitsAbilities, string>()
            {

                [UnitsAbilities.Whip]   = WhipLvl_2,
                [UnitsAbilities.Garlic] = GarlicLvl_2,
                [UnitsAbilities.Axe]    = AxeLvl_2,
                [UnitsAbilities.Spear]  = SpearLvl_2,
                [UnitsAbilities.Bible]  = BibleLvl_2,
                [UnitsAbilities.Bow]    = BowLvl_2,
                [UnitsAbilities.Knife]  = KnifeLvl_2,
                [UnitsAbilities.Bottle] = BottleLvl_2
            });

        abilities.Add(
            new Dictionary<UnitsAbilities, string>()
            {

                [UnitsAbilities.Whip]   = WhipLvl_3,
                [UnitsAbilities.Garlic] = GarlicLvl_3,
                [UnitsAbilities.Axe]    = AxeLvl_3,
                [UnitsAbilities.Spear]  = SpearLvl_3,
                [UnitsAbilities.Bible]  = BibleLvl_3,
                [UnitsAbilities.Bow]    = BowLvl_3,
                [UnitsAbilities.Knife]  = KnifeLvl_3,
                [UnitsAbilities.Bottle] = BottleLvl_3
            });
    }

    public string GetAbilityDescription(UnitsAbilities ability, int level)
    {
        return abilities[level - 1][ability];
    }
}
