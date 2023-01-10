using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SpellWorkroom : SpecialBuilding
{
    [SerializeField] private GameObject warning;

    public override GameObject Init(CastleBuildings building)
    {

        gameObject.SetActive(true);

        return gameObject;
    }
}
