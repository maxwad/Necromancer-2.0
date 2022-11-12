using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[Serializable]
public class BossSpell
{
    public BossSpells spell;
    public float attackPeriod;
}

public class BossesArsenal : MonoBehaviour
{
    public List<BossSpell> arsenal;


    public BossSpell GetBossSpell()
    {
        if(arsenal.Count > 0)
        {
            //return arsenal[UnityEngine.Random.Range(0, arsenal.Count)];
            return arsenal[3];
        }
        else
        {
            return null;
        } 
    }
}
