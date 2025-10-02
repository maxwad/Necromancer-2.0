using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class WeaponsDictionary : MonoBehaviour
{
    [HideInInspector] public static WeaponsDictionary instance;

    private List<Dictionary<UnitsWeapon, string>> abilities = new List<Dictionary<UnitsWeapon, string>>();

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
            new Dictionary<UnitsWeapon, string>() { 

                [UnitsWeapon.Whip]   = WhipLvl_1,
                [UnitsWeapon.Garlic] = GarlicLvl_1,
                [UnitsWeapon.Axe]    = AxeLvl_1,
                [UnitsWeapon.Spear]  = SpearLvl_1,
                [UnitsWeapon.Bible]  = BibleLvl_1,
                [UnitsWeapon.Arrow]    = BowLvl_1,
                [UnitsWeapon.Knife]  = KnifeLvl_1,
                [UnitsWeapon.Bottle] = BottleLvl_1
            });

        abilities.Add(
            new Dictionary<UnitsWeapon, string>()
            {

                [UnitsWeapon.Whip]   = WhipLvl_2,
                [UnitsWeapon.Garlic] = GarlicLvl_2,
                [UnitsWeapon.Axe]    = AxeLvl_2,
                [UnitsWeapon.Spear]  = SpearLvl_2,
                [UnitsWeapon.Bible]  = BibleLvl_2,
                [UnitsWeapon.Arrow]    = BowLvl_2,
                [UnitsWeapon.Knife]  = KnifeLvl_2,
                [UnitsWeapon.Bottle] = BottleLvl_2
            });

        abilities.Add(
            new Dictionary<UnitsWeapon, string>()
            {

                [UnitsWeapon.Whip]   = WhipLvl_3,
                [UnitsWeapon.Garlic] = GarlicLvl_3,
                [UnitsWeapon.Axe]    = AxeLvl_3,
                [UnitsWeapon.Spear]  = SpearLvl_3,
                [UnitsWeapon.Bible]  = BibleLvl_3,
                [UnitsWeapon.Arrow]    = BowLvl_3,
                [UnitsWeapon.Knife]  = KnifeLvl_3,
                [UnitsWeapon.Bottle] = BottleLvl_3
            });
    }

    public string GetAbilityDescription(UnitsWeapon ability, int level)
    {
        return abilities[level - 1][ability];
    }
}
