using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public static class StructManager
{
    public struct AttackSettings
    {
        public UnitsAbilities ability;
        public int level;
        public GameObject weapon;
        public bool flip;
        public float size;
        public float physicDamage;
        public float magicDamage;

        public AttackSettings(UnitsAbilities ability, int level, GameObject weapon, float size, bool flip, float physicDamage, float magicDamage)
        {
            this.ability      = ability;
            this.level        = level;
            this.weapon       = weapon;
            this.flip         = flip;
            this.size         = size;
            this.physicDamage = physicDamage;
            this.magicDamage  = magicDamage;
        }

    }

    public struct WeaponSettings
    {
        public float size;
        public bool flip;
        public float speed;
        public float physicDamage;
        public float magicDamage;
        public float lifeTime;

        public WeaponSettings(float size, bool flip, float speed, float physicDamage, float magicDamage, float lifeTime)
        {
            this.size = size;
            this.flip = flip;
            this.speed = speed;
            this.physicDamage = physicDamage;
            this.magicDamage = magicDamage;
            this.lifeTime = lifeTime;
        }

    }
}
