using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static NameManager;

public class WeaponMovement : MonoBehaviour
{
    Unit unit;
    private bool isReadyToWork = false;

    private Rigidbody2D rbWeapon;

    private SpriteRenderer bible;
    private int bibleLevel;

    [SerializeField] private GameObject bottleDeath;
    [SerializeField] private GameObject bottleShadowPrefab;
    [SerializeField] private Vector3 bottleShadowStartScale = new Vector3(1f, 1f, 1f);
    private GameObject bottleShadow;
    private Vector3 groundVelocity;
    private float verticalVelocity;
    private float gravity = -10;


    public float speed = 1;
    private SpriteRenderer unitSprite;

    public float lifeTime = 0.1f;
    private float currentLifeTime = 0;

    private Coroutine coroutine;
    private WeaponStorage weaponStorage;
    private WeaponDamage weaponDamage;
    private BoostManager boostManager;
    private GameObject effectsContainer;
    private ObjectsPoolManager poolManager;

    [Inject]
    public void Construct(
        [Inject(Id = Constants.EFFECTS_CONTAINER)] GameObject effectsContainer,
        WeaponStorage weaponStorage, 
        BoostManager boostManager,
        ObjectsPoolManager poolManager
        )
    {
        this.effectsContainer = effectsContainer;
        this.weaponStorage = weaponStorage;
        this.boostManager = boostManager;
        this.poolManager = poolManager;;

        weaponDamage = GetComponent<WeaponDamage>();
    }

    private void Update()
    {
        if(lifeTime != 0)
        {
            currentLifeTime += Time.deltaTime;

            if(currentLifeTime >= lifeTime) gameObject.SetActive(false);
        }

        if(isReadyToWork == true) 
            WeaponMoving();
    }

    private void WeaponMoving()
    {
        switch(unit.unitAbility)
        {
            case UnitsAbilities.Spear:
                SpearMovement();
                break;

            case UnitsAbilities.Bible:
                BibleMovement();
                break;

            case UnitsAbilities.Bow:
                BowMovement();
                break;

            case UnitsAbilities.Knife:
                KnifeMovement();
                break;

            case UnitsAbilities.Bottle:
                BottleMovement();
                break;
            default:
                break;
        }
    }

    #region Helpers
    public void SetSettings(Unit unitSource)
    {
        unit = unitSource;
        unitSprite = unitSource.unitController.unitSprite;
    }

    public void ActivateWeapon(Unit unit, int index = 0)
    {
        weaponDamage.ClearEnemyList();

        switch(unit.unitAbility)
        {
            case UnitsAbilities.Garlic:
                ActivateGarlic();
                break;

            case UnitsAbilities.Axe:
                ActivateAxe(index);
                break;

            case UnitsAbilities.Spear:
                ActivateSpear();
                break;

            case UnitsAbilities.Bible:
                ActivateBible(unit);
                break;

            case UnitsAbilities.Bow:
                ActivateBow();
                break;

            case UnitsAbilities.Knife:
                ActivateKnife();
                break;
            case UnitsAbilities.Bottle:
                ActivateBottle();
                break;

            default:
                break;
        }
    }

    #endregion

    #region Movement

    private void SpearMovement()
    {
        rbWeapon.velocity = -rbWeapon.transform.right * speed;
    }

    private void BibleMovement()
    {
        float weaponSize = unit.size + unit.size * boostManager.GetBoost(BoostType.WeaponSize);
        transform.localScale = new Vector3(weaponSize, weaponSize, weaponSize);

        transform.RotateAround(transform.position, Vector3.forward, speed * Time.deltaTime);
        bible.transform.RotateAround(bible.transform.position, Vector3.forward, -speed * Time.deltaTime);

        //reset EnemyList every cycle
        if(transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 5f)
        {
            weaponDamage.ClearEnemyList();
        }

        //control of new level and new position
        transform.position = unit.unitController.gameObject.transform.position + weaponStorage.weaponOffset;
        if(unit.isUnitActive == false || bibleLevel != weaponStorage.unitForBible.level)
        {
            weaponStorage.isBibleWork = false;
            DestroyWeapon();
        }
    }

    private void BowMovement()
    {
        rbWeapon.velocity = -rbWeapon.transform.right * speed;
    }

    private void KnifeMovement()
    {
        rbWeapon.velocity = -rbWeapon.transform.right * speed;
    }

    private void BottleMovement()
    {        
        verticalVelocity += gravity * Time.deltaTime;
        rbWeapon.transform.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        rbWeapon.transform.position += groundVelocity * Time.deltaTime;

        bottleShadow.transform.position += groundVelocity * Time.deltaTime;

        if(rbWeapon.transform.position.y < bottleShadow.transform.position.y)
            Explosive();

        void Explosive()
        {
            GameObject death = poolManager.GetUnusualPrefab(bottleDeath);
            death.transform.position = transform.position;
            death.SetActive(true);
            death.transform.SetParent(effectsContainer.transform);
            PrefabSettings settings = death.GetComponent<PrefabSettings>();
            float weaponSize = unit.size + unit.size * boostManager.GetBoost(BoostType.WeaponSize);

            if(settings != null)
            {
                settings.SetSettings(
                sortingLayer: TagManager.T_PLAYER, 
                sortingOrder: 11, 
                color: Color.cyan, 
                size: weaponSize * 2,
                animationSpeed: 0.07f);
            }

            Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, weaponSize * 2);
            foreach(Collider2D obj in objects)
            {
                if(obj.CompareTag(TagManager.T_ENEMY) == true)
                    weaponDamage.Hit(obj.GetComponent<EnemyController>(), transform.position);
            }

            bottleShadow.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region Activators
    private void ActivateGarlic()
    {
        StartCoroutine(Resize());

        IEnumerator Resize()
        {
            int countOfSteps = 100;
            float currentSize = 0;
            float endSize = gameObject.transform.localScale.x;
            float sizeStep = endSize / countOfSteps;

            gameObject.transform.localScale = new Vector3(0, 0, 0);           

            while(currentSize <= endSize)
            {
                yield return new WaitForSeconds(0.01f);
                currentSize += sizeStep;
                gameObject.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
            }
            gameObject.SetActive(false);
        }
    }

    private void ActivateAxe(int numberOfWeapon)
    {
        float force = 30f;
        float torqueForce = -200;

        if(unit.level == 2)
        {
            if(numberOfWeapon == 1) torqueForce = -torqueForce;
        }

        if(unit.level == 3)        {

            if(numberOfWeapon == 3) torqueForce = -torqueForce;
        }

        Rigidbody2D rbAxe = GetComponent<Rigidbody2D>();

        rbAxe.AddForce(rbAxe.transform.up * force, ForceMode2D.Impulse);
        rbAxe.AddTorque(torqueForce);
    }

    private void ActivateSpear()
    {
        rbWeapon = GetComponent<Rigidbody2D>();

        float searchRadius = 15f;
        float distance = 9999999;
        Vector2 nearestEnemyPosition = Vector2.zero;

        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        foreach(Collider2D obj in objects)
        {
            if(obj.CompareTag(TagManager.T_ENEMY) == true)
            {
                float currentDistance = Vector2.Distance(transform.position, obj.transform.position);

                if(currentDistance < distance)
                {
                    distance = currentDistance;
                    nearestEnemyPosition = obj.transform.position;                 
                }
            }            
        }

        if(nearestEnemyPosition != Vector2.zero)
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                Mathf.Atan2(nearestEnemyPosition.y - transform.position.y, nearestEnemyPosition.x - transform.position.x) * Mathf.Rad2Deg - 180
            );
        }
        else
        {
            float yAngle = unitSprite.flipX == true ? 180 : 0;
            rbWeapon.transform.eulerAngles = new Vector3(rbWeapon.transform.eulerAngles.x, yAngle, rbWeapon.transform.eulerAngles.z);
        }
        
        isReadyToWork = true;
    }

    private void ActivateBible(Unit currentUnit)
    {
        bible = GetComponentInChildren<SpriteRenderer>();
        isReadyToWork = true;
        bibleLevel = currentUnit.level;
    }

    private void ActivateBow()
    {
        rbWeapon = GetComponent<Rigidbody2D>();
        isReadyToWork = true;
    }

    private void ActivateKnife() 
    {
        rbWeapon = GetComponent<Rigidbody2D>();
        isReadyToWork = true;
    }

    private void ActivateBottle() 
    {
        float minRadiusMovement = 4;
        float maxRadiusMovement = 6;
        float torqueForce = 300;

        rbWeapon = GetComponent<Rigidbody2D>();

        Vector3 goalVector = GetRandomPoint();
        groundVelocity = goalVector.normalized * speed;
        verticalVelocity = Random.Range(minRadiusMovement - 1, maxRadiusMovement);

        torqueForce = goalVector.x < 0 ? torqueForce : -torqueForce;
        rbWeapon.AddTorque(torqueForce);

        bottleShadow = poolManager.GetUnusualPrefab(bottleShadowPrefab);
        bottleShadow.transform.position = transform.position;
        bottleShadow.transform.localScale = bottleShadowStartScale;
        bottleShadow.transform.SetParent(effectsContainer.transform);
        bottleShadow.SetActive(true);

        if(gameObject.activeInHierarchy == true)
        {
            if(coroutine != null) 
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(ScaleShadow());
        }
        isReadyToWork = true;


        Vector3 GetRandomPoint()
        {
            float x = Random.Range(-maxRadiusMovement, maxRadiusMovement);
            float y = Random.Range(-maxRadiusMovement, maxRadiusMovement);

            Vector3 resultPoint = Vector3.zero;

            if((x > -minRadiusMovement && x < minRadiusMovement) && (y > -minRadiusMovement && y < minRadiusMovement))
                resultPoint = GetRandomPoint();
            else
                resultPoint = new Vector3(x, y, 0);

            return resultPoint;
        }

        IEnumerator ScaleShadow()
        {
            while(true)
            {
                if(bottleShadow == null) 
                    break;

                if(verticalVelocity > 0)
                    bottleShadow.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                else
                    bottleShadow.transform.localScale += new Vector3(0.03f, 0.03f, 0.03f);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    #endregion

    private void OnBecameInvisible()
    {
        DestroyWeapon();
    }

    // We don't need this part because we disable weapon when it is invisible

    private void DestroyWeapon(bool mode = false)
    {
        if(bottleShadow != null)
        {
            if(coroutine != null) 
                StopCoroutine(coroutine);

            bottleShadow.SetActive(false);
            bottleShadow = null;
        }
        if(unit != null && unit.unitAbility == UnitsAbilities.Bible)
        {
            weaponStorage.isBibleWork = false;
        }
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += DestroyWeapon;
        currentLifeTime = 0;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= DestroyWeapon;
    }
}
