using System.Collections;
using UnityEngine;
using Zenject;
using static Enums;

public class ManningWeapon : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private ObjectsPoolManager objectsPoolManager;
    private HeroController hero;

    private EnemyWeaponParameters weaponParameters;
    private Coroutine coroutine;
    private float attackPeriod;
    private float attackDelay;
    private float timeOffset;
    private float currentTime = 0;
    private float countOfShoot = 0;
    private float currentShoot = 0;

    [Inject]
    public void Construct(
        ResourcesManager resourcesManager,
        ObjectsPoolManager objectsPoolManager,
        HeroController hero
        )
    {
        this.resourcesManager = resourcesManager;
        this.objectsPoolManager = objectsPoolManager;
        this.hero = hero;

        weaponParameters = GetComponent<EnemyWeaponParameters>();

        attackPeriod = weaponParameters.attackPeriod;
        attackDelay = weaponParameters.attackDelay;
        timeOffset = weaponParameters.timeOffset;
        countOfShoot = Mathf.Floor(attackPeriod / attackDelay);
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;

        currentTime = 0;
        currentShoot = 0;

        coroutine = StartCoroutine(Attact());
    }

    private void Stop()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        gameObject.SetActive(false);
    }

    private IEnumerator Attact()
    {
        while(currentShoot < countOfShoot)
        {
            yield return new WaitForSeconds(attackDelay - timeOffset);

            Shot();

            currentShoot++;
            currentTime += (attackDelay - timeOffset);
        }

        while(currentTime < attackPeriod)
        {
            yield return new WaitForSeconds(attackPeriod - currentTime);
            currentTime += (attackPeriod - currentTime);
        }

        gameObject.SetActive(false);
    }

    private void Shot()
    {
        resourcesManager.ChangeResource(ResourceType.Mana, -1);

        GameObject death = objectsPoolManager.GetObject(ObjectPool.ManaDeath);
        death.transform.position = hero.transform.position;
        death.SetActive(true);

        GameObject bloodSpot = objectsPoolManager.GetObject(ObjectPool.ManaSpot);
        bloodSpot.transform.position = hero.transform.position;
        bloodSpot.SetActive(true);
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;

        if(coroutine != null) StopCoroutine(coroutine);
    }
}
