using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SpellLibrary : MonoBehaviour
{
    private BoostManager boostManager;
    private ObjectsPoolManager objectsPool;
    private PlayersArmy playersArmy;

    private void Start()
    {
        boostManager = GlobalStorage.instance.boostManager;
        objectsPool = GlobalStorage.instance.objectsPoolManager;
        playersArmy = GlobalStorage.instance.playersArmy;
    }

    public void ActivateSpell(SpellSO spell, bool mode)
    {
        switch(spell.spell)
        {
            case Spells.SpeedUp:
                SpeedUp(mode, spell);
                break;

            case Spells.AttackUp:
                AttackUp(mode, spell);
                break;

            case Spells.DoubleCrit:
                DoubleCrit(mode, spell);
                break;

            case Spells.Shurikens:
                Shurikens(mode, spell);
                break;

            case Spells.GoAway:
                GoAway(mode, spell);
                break;

            case Spells.AllBonuses:
                AllBonuses(mode, spell);
                break;

            case Spells.Healing:
                Healing(mode, spell);
                break;

            case Spells.DoubleBonuses:
                DoubleBonuses(mode, spell);
                break;

            case Spells.WeaponSize:
                WeaponSize(mode, spell);
                break;

            case Spells.Maning:
                Maning(mode, spell);
                break;

            case Spells.Immortal:
                Immortal(mode, spell);
                break;

            case Spells.EnemiesStop:
                EnemiesStop(mode, spell);
                break;

            case Spells.DestroyEnemies:
                DestroyEnemies(mode, spell);
                break;

            case Spells.ExpToGold:
                ExpToGold(mode, spell);
                break;

            case Spells.ResurrectUnit:
                ResurrectUnit(mode, spell);
                break;

            default:
                break;
        }

        //if(mode == true)
        //{
        //    float realDuration = spell.actionTime + spell.actionTime * boostManager.GetBoost(BoostType.SpellActionTime);
        //    StartCoroutine(DeactivateSpell(spell, realDuration));
        //}
    }

    #region Helpers

    //private IEnumerator DeactivateSpell(SpellSO spell, float duration)
    //{
    //    yield return new WaitForSeconds(duration);

    //    if(GlobalStorage.instance.IsGlobalMode() == false)
    //        ActivateSpell(spell, false);
    //}

    #endregion

    #region SPELLS

    //Increases the hero's movement speed by 20% for 30 seconds.
    private void SpeedUp(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            boostManager.SetBoost(BoostType.MovementSpeed, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
        }
        //else
        //{
        //    boostManager.DeleteBoost(BoostType.MovementSpeed, BoostSender.Spell, value);
        //}        
    }


    //Increase attack power by 20% for 30 seconds.
    private void AttackUp(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            boostManager.SetBoost(BoostType.MagicAttack, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
            boostManager.SetBoost(BoostType.PhysicAttack, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
        }
        //else
        //{
        //    boostManager.DeleteBoost(BoostType.MagicAttack, BoostSender.Spell, value);
        //    boostManager.DeleteBoost(BoostType.PhysicAttack, BoostSender.Spell, value);
        //}
    }


    //All damage becomes critical for 5 seconds.
    private void DoubleCrit (bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            boostManager.SetBoost(BoostType.CriticalDamage, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
        }
        //else
        //{
        //    boostManager.DeleteBoost(BoostType.CriticalDamage, BoostSender.Spell, value);
        //}
    }


    //Increases weapon size by 20% for 30 seconds.
    private void WeaponSize(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            boostManager.SetBoost(BoostType.WeaponSize, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
        }
        //else
        //{
        //    boostManager.DeleteBoost(BoostType.WeaponSize, BoostSender.Spell, value);
        //}
    }


    //Hero throws 8 shurikens that kill everyone in their path.
    private void Shurikens(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            float angleItem = 360 / spell.value;

            for(int i = 0; i < spell.value; i++)
            {
                GameObject item = objectsPool.GetObject(ObjectPool.Shuriken);
                item.transform.position = GlobalStorage.instance.hero.transform.position;
                item.transform.rotation = Quaternion.Euler(0f, 0f, angleItem * i);
                item.SetActive(true);
            }
        }
    }


    //Knock back all enemies within a radius of 10 meters.
    private void GoAway(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            GameObject circle = objectsPool.GetObject(ObjectPool.PushCircle);
            circle.transform.position = GlobalStorage.instance.hero.transform.position;
            circle.SetActive(true);
            circle.GetComponent<PushCircleController>().Init(spell.radius * 2);
        }
    }


    //Pull all the bonuses in the field.
    private void AllBonuses(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            List<GameObject> bonusList = GlobalStorage.instance.bonusManager.bonusesOnTheMap;            
            foreach(var bonus in bonusList)
            {
                bonus.GetComponent<BonusController>().ActivatateBonus();
            }
        }
    }


    //Heal the hero by 10.
    private void Healing(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            EventManager.OnBonusPickedUpEvent(BonusType.Health, spell.value);
        }
    }


    //Restore 10 mana.
    private void Maning(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            EventManager.OnBonusPickedUpEvent(BonusType.Mana, spell.value);
        }
    }


    //Double all bonuses for 30 seconds.
    private void DoubleBonuses(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            boostManager.SetBoost(BoostType.BonusAmount, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
        }
        //else
        //{
        //    boostManager.DeleteBoost(BoostType.BonusAmount, BoostSender.Spell, value);
        //}
    }


    //Make units immortal for 30 seconds.
    private void Immortal(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            boostManager.SetBoost(BoostType.MagicDefence, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
            boostManager.SetBoost(BoostType.PhysicDefence, BoostSender.Spell, BoostEffect.PlayerBattle, spell.value);
        }

        //EventManager.OnSpellImmortalEvent(mode);
    }


    //Stop all enemies for 10 seconds.
    private void EnemiesStop(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            boostManager.SetBoost(BoostType.EnemyMovementSpeed, BoostSender.Spell, BoostEffect.EnemiesBattle, spell.value);
        }
        //List<GameObject> enemies = GlobalStorage.instance.battleMap.GetComponent<EnemySpawner>().enemiesOnTheMap;

        //foreach(var enemy in enemies)
        //    enemy.GetComponent<EnemyMovement>().MakeMeFixed(mode);
        // do we need stop spawn enemy while they freeze?
    }


    //Destroy all enemies within a radius of 15 meters.
    private void DestroyEnemies(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            List<GameObject> enemies = GlobalStorage.instance.battleMap.GetComponent<EnemySpawner>().enemiesOnTheMap;
            GameObject hero = GlobalStorage.instance.hero.gameObject;

            int count = enemies.Count - 1;

            for(int i = count; i >= 0; i--)
            {
                if(Vector2.Distance(enemies[i].transform.position, hero.transform.position) <= spell.radius)
                {
                    enemies[i].GetComponent<EnemyController>().Kill(spell.value);
                }
            }

            Camera.main.GetComponent<BattleCamera>()?.ShakeCamera();
        }
    }


    //Turn all experience bonuses on field into gold.
    private void ExpToGold(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            List<GameObject> allBonuses = GlobalStorage.instance.bonusManager.bonusesOnTheMap;

            List<GameObject> bonuses = new List<GameObject>();

            foreach(var item in allBonuses)
            {
                if(Vector2.Distance(item.transform.position, GlobalStorage.instance.hero.gameObject.transform.position) <= spell.radius)
                {
                    bonuses.Add(item);
                } 
            }

            int count = bonuses.Count - 1;

            for(int i = count; i >= 0; i--)
            {
                BonusController bonus = bonuses[i].GetComponent<BonusController>();
                float amount = bonus.baseValue;
                if(bonus.bonusType == BonusType.TempExp)
                {
                    GlobalStorage.instance.bonusManager.CreateBonus(false, BonusType.Gold, bonus.transform.position, amount);
                    bonus.DestroyMe();
                }
            }
        }
    }


    //Resurrect one last killed unit from the infirmary.
    private void ResurrectUnit(bool mode, SpellSO spell)
    {
        if(mode == true)
        {
            playersArmy.ResurrectionFewUnitInTheBattle(spell.value);
        }
    }

    #endregion

    private void AllStop()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += AllStop;
    }


    private void OnDisable()
    {
        EventManager.EndOfBattle -= AllStop;
    }
}
