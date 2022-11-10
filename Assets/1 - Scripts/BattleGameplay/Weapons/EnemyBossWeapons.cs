using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyBossWeapons : MonoBehaviour
{
    private List<BossSpells> currentBossSpellList = new List<BossSpells>();
    private Coroutine bossCoroutine;

    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject manningLessPrefab;

    private GameObject currentWeapon;

    public void ActivateBossWeapon(BossSpells bossSpell, bool mode)
    {
        switch(bossSpell)
        {
            case BossSpells.InvertMovement:
                InvertMovement(mode);
                break;

            case BossSpells.Lightning:
                Lightning(mode);
                break;

            case BossSpells.ManningLess:
                ManningLess(mode);
                break;

            default:
                break;
        }

        //if(mode == true)
        //{
        //    currentBossSpellList.Add(bossSpell);
        //    StartCoroutine(DeactivateBossSpell(bossSpell, duration));
        //}
        //else
        //{
        //    currentBossSpellList.Remove(bossSpell);
        //    if(bossCoroutine != null) StopCoroutine(bossCoroutine);
        //}
    }

    //private IEnumerator DeactivateBossSpell(BossSpells spell, float duration)
    //{
    //    yield return new WaitForSeconds(duration);

    //    ActivateBossWeapon(spell, false);
    //}

    //private void DeactivateAllBossSpells(bool mode)
    //{
    //    if(mode == true)
    //    {
    //        foreach(var item in currentBossSpellList)
    //        {
    //            StartCoroutine(DeactivateBossSpell(item, 0));
    //        }
    //    }
    //}

    //public void AbortBossSpell()
    //{
    //    DeactivateAllBossSpells(true);
    //}

    private void InvertMovement(bool mode)
    {
        GlobalStorage.instance.battlePlayer.MovementInverting(mode);
    }

    private void ManningLess(bool mode)
    {
        if(mode == true) bossCoroutine = StartCoroutine(StealMana(-1));

        IEnumerator StealMana(float quantity)
        {
            while(true)
            {
                yield return new WaitForSeconds(2f);
                GlobalStorage.instance.resourcesManager.ChangeResource(ResourceType.Mana, quantity);
            }
        }

        if(mode == true)
        {
            GlobalStorage.instance.resourcesManager.ChangeResource(ResourceType.Mana, -1);
            //GameObject ray = Instantiate(lightningPrefab, GlobalStorage.instance.hero.transform.position, Quaternion.identity);
            //ray.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
            //currentWeapon = ray;
        }
        else
        {
            //if(currentWeapon != null) Destroy(currentWeapon);
        }
    }
    private void Lightning(bool mode)
    {
        //if(mode == true) bossCoroutine = StartCoroutine(StartLightning());

        //IEnumerator StartLightning()
        //{
        //    while(true)
        //    {
        //        yield return new WaitForSeconds(2f);
        //        GameObject ray = Instantiate(lightningPrefab, GlobalStorage.instance.hero.transform.position, Quaternion.identity);
        //        ray.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
        //    }
        //}

        if(mode == true)
        {

            Debug.Log("Start");
            GameObject ray = GlobalStorage.instance.objectsPoolManager.GetWeapon(UnitsAbilities.EnemyLigthning);
            ray.transform.position = GlobalStorage.instance.hero.transform.position;
            ray.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
            currentWeapon = ray;
            currentWeapon.SetActive(true);
        }
        else
        {
            Debug.Log("End");
            if(currentWeapon != null) currentWeapon.SetActive(false);
        }
    }


    //private void OnEnable()
    //{
    //    EventManager.SwitchPlayer += DeactivateAllBossSpells;
    //}

    //private void OnDisable()
    //{
    //    EventManager.SwitchPlayer += DeactivateAllBossSpells;
    //}
}
