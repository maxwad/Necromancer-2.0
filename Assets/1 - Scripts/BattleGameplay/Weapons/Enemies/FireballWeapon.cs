using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Enums;

public class FireballWeapon : MonoBehaviour
{
    private ObjectsPoolManager objectsPool;
    private GameObject player;

    private EnemyWeaponParameters weaponParameters;
    private Coroutine coroutine;
    private Coroutine appearing;
    private SpriteRenderer spriteRenderer;

    private bool isWatching = false;

    private float attackPeriod;
    private float attackDelay;
    private float timeOffset;
    private float currentTime = 0;
    private float countOfShoot = 0;
    private float currentShoot = 0;

    private float currentAngle = 0f;
    public int bulletCount = 3;
    public float wideSector = 81;

    private ObjectPool bullet;

    [Inject]
    public void Construct(ObjectsPoolManager objectsPool, HeroController hero)
    {
        this.objectsPool = objectsPool;
        player = hero.gameObject;

        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponParameters = GetComponent<EnemyWeaponParameters>();

        bullet = weaponParameters.bullet;
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

        isWatching = true;

        appearing = StartCoroutine(Fade(false));
        coroutine = StartCoroutine(Attact());

    }

    private void Update()
    {
        if(isWatching == true)
        {
            Vector3 targ = player.transform.position;
            targ.z = 0f;

            Vector3 objectPos = transform.position;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;

            currentAngle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(45, 0, currentAngle));
        }
    }

    private IEnumerator Fade(bool fadeMode)
    {
        float step = 0.01f;
        float startValue = 0f;
        float endValue = 0.7f;

        if(fadeMode == true)
        {
            startValue = 1f;
            endValue = 0f;
        }

        float currentValue = startValue;

        Color currentColor = spriteRenderer.color;

        if(fadeMode == false)
        {
            while(currentValue <= endValue)
            {
                currentValue += step;
                spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);
                yield return new WaitForSeconds(step);
            }
        }
        else
        {
            while(currentValue >= endValue)
            {
                currentValue -= step;
                spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);
                yield return new WaitForSeconds(step);
            }

            gameObject.SetActive(false);
        }
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
        float startAngle = (currentAngle + wideSector / 2) - 180f;
        float angleStep = wideSector / (bulletCount - 1);

        for(int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            GameObject currentBullet = objectsPool.GetObject(bullet);
            currentBullet.transform.position = transform.position;
            currentBullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            currentBullet.SetActive(true);
        }
    }

    private void Stop()
    {
        isWatching = false;
        if(appearing != null) StopCoroutine(appearing);
        if(coroutine != null) StopCoroutine(coroutine);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;

        isWatching = false;
        if(appearing != null) StopCoroutine(appearing);
        if(coroutine != null) StopCoroutine(coroutine);
    }
}
