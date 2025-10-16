using Enums;
using System.Collections;
using UnityEngine;
using Zenject;

public class EnemyWeapon : MonoBehaviour
{
    public float AttackPeriod => attackPeriod;

    [SerializeField] protected BossWeapons bullet;
    [SerializeField] protected float attackPeriod;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float timeOffset;
    [SerializeField] protected float currentTime = 0;
    [SerializeField] protected float countOfShoot = 0;
    [SerializeField] protected float currentShoot = 0;

    protected Coroutine coroutine;

    protected ResourcesManager resourcesManager;
    protected ObjectsPoolManager objectsPool;
    protected HeroController hero;
    protected GameObject effectsContainer;

    [Inject]
    public void Construct(
        ResourcesManager resourcesManager,
        ObjectsPoolManager objectsPoolManager,
        HeroController hero,
        [Inject(Id = Constants.EFFECTS_CONTAINER)] GameObject effectsContainer
        )
    {
        this.resourcesManager = resourcesManager;
        this.objectsPool = objectsPoolManager;
        this.hero = hero;
        this.effectsContainer = effectsContainer;

        countOfShoot = Mathf.Floor(attackPeriod / attackDelay);
    }

    protected virtual IEnumerator Attack()
    {
        yield return null;
    }

    protected virtual void Stop()
    {

    }

    protected virtual void Shot()
    {

    }

    protected virtual void SpecialDisableEffect()
    {

    }

    protected virtual void SpecialEnableEffect()
    {

    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;

        currentTime = 0;
        currentShoot = 0;

        SpecialEnableEffect();
        coroutine = StartCoroutine(Attack());
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;

        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            SpecialDisableEffect();
        }
    }
}
