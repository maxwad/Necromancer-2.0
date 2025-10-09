using Enums;
using System.Collections;
using UnityEngine;
using Zenject;

public class WeaponMovement : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] private Rigidbody2D weaponRB;
    [SerializeField] private float speed = 1;
    [SerializeField] private float lifeTime = 0.1f;
    [SerializeField] private SimpleAnimationObject deathEffectPrefab;
    [SerializeField] private MonoBehaviour shadowPrefab;

    private bool isReadyToWork = false;
    private Vector3 groundVelocity;
    private float verticalVelocity;
    private float gravity = -10;
    private Vector3 bottleShadowStartScale = new Vector3(0.4f, 0.4f, 0.4f);

    private MonoBehaviour bottleShadow;
    private Unit unit;
    private SpriteRenderer unitSprite;

    private int bibleLevel;
    private float currentLifeTime = 0;
    private Coroutine coroutine;

    private GameObject effectsContainer;
    private WeaponStorage weaponStorage;
    private BoostManager boostManager;
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
        this.poolManager = poolManager;
    }

    private void Update()
    {
        if(lifeTime != 0)
        {
            currentLifeTime += Time.deltaTime;

            if(currentLifeTime >= lifeTime)
            {
                DestroyWeapon();
            }
        }

        if(isReadyToWork)
        {
            WeaponMoving();
        }
    }

    private void WeaponMoving()
    {
        switch(unit.unitWeapon)
        {
            case UnitsWeapon.Spear:
                SpearMovement();
                break;

            case UnitsWeapon.Bible:
                BibleMovement();
                break;

            case UnitsWeapon.Arrow:
                BowMovement();
                break;

            case UnitsWeapon.Knife:
                KnifeMovement();
                break;

            case UnitsWeapon.Bottle:
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
        weapon.ClearEnemyList();

        switch(unit.unitWeapon)
        {
            case UnitsWeapon.Garlic:
                ActivateGarlic();
                break;

            case UnitsWeapon.Axe:
                ActivateAxe(index);
                break;

            case UnitsWeapon.Spear:
                ActivateSpear();
                break;

            case UnitsWeapon.Bible:
                ActivateBible(unit);
                break;

            case UnitsWeapon.Arrow:
                ActivateBow();
                break;

            case UnitsWeapon.Knife:
                ActivateKnife();
                break;
            case UnitsWeapon.Bottle:
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
        weaponRB.velocity = -weaponRB.transform.right * speed;
    }

    private void BibleMovement()
    {
        float weaponSize = unit.size + unit.size * boostManager.GetBoost(BoostType.WeaponSize);
        transform.localScale = new Vector3(weaponSize, weaponSize, weaponSize);

        transform.RotateAround(transform.position, Vector3.forward, speed * Time.deltaTime);
        weaponSprite.transform.RotateAround(weaponSprite.transform.position, Vector3.forward, -speed * Time.deltaTime);

        //reset EnemyList every cycle
        if(transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 5f)
        {
            weapon.ClearEnemyList();
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
        weaponRB.velocity = -weaponRB.transform.right * speed;
    }

    private void KnifeMovement()
    {
        weaponRB.velocity = -weaponRB.transform.right * speed;
    }

    private void BottleMovement()
    {
        verticalVelocity += gravity * Time.deltaTime;
        weaponRB.transform.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        weaponRB.transform.position += groundVelocity * Time.deltaTime;

        bottleShadow.transform.position += groundVelocity * Time.deltaTime;

        if(weaponRB.transform.position.y < bottleShadow.transform.position.y)
        {
            Explosive();
        }

        void Explosive()
        {
            SimpleAnimationObject death = poolManager.GetOrCreateElement(deathEffectPrefab, this, effectsContainer.transform, true);
            death.transform.position = transform.position;

            float weaponSize = unit.size + unit.size * boostManager.GetBoost(BoostType.WeaponSize);

            if(death != null)
            {
                death.SetSettings(
                sortingLayer: TagManager.T_PLAYER,
                sortingOrder: 11,
                color: Color.cyan,
                size: weaponSize,
                animationSpeed: 0.07f,
                prefabSource: this
                );
            }

            Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, weaponSize);
            foreach(Collider2D obj in objects)
            {
                if(obj.CompareTag(TagManager.T_ENEMY))
                {
                    weapon.Hit(obj.GetComponent<EnemyController>(), transform.position);
                }
            }

            DestroyWeapon();
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

            DestroyWeapon();
        }
    }

    private void ActivateAxe(int numberOfWeapon)
    {
        float force = 30f;
        float torqueForce = -200;

        if(unit.level == 2)
        {
            if(numberOfWeapon == 1)
            {
                torqueForce = -torqueForce;
            }
        }

        if(unit.level == 3)
        {
            if(numberOfWeapon == 3)
            {
                torqueForce = -torqueForce;
            }
        }

        weaponRB.AddForce(weaponRB.transform.up * force, ForceMode2D.Impulse);
        weaponRB.AddTorque(torqueForce);
    }

    private void ActivateSpear()
    {
        float searchRadius = 15f;
        float distance = 9999999;
        Vector2 nearestEnemyPosition = Vector2.zero;

        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        foreach(Collider2D obj in objects)
        {
            if(obj.CompareTag(TagManager.T_ENEMY))
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
            weaponRB.transform.eulerAngles = new Vector3(weaponRB.transform.eulerAngles.x, yAngle, weaponRB.transform.eulerAngles.z);
        }

        isReadyToWork = true;
    }

    private void ActivateBible(Unit currentUnit)
    {
        isReadyToWork = true;
        bibleLevel = currentUnit.level;
    }

    private void ActivateBow()
    {
        isReadyToWork = true;
    }

    private void ActivateKnife()
    {
        isReadyToWork = true;
    }

    private void ActivateBottle()
    {
        float minRadiusMovement = 4;
        float maxRadiusMovement = 6;
        float torqueForce = 300;

        Vector3 goalVector = GetRandomPoint();
        groundVelocity = goalVector.normalized * speed;
        verticalVelocity = Random.Range(minRadiusMovement - 1, maxRadiusMovement);

        torqueForce = goalVector.x < 0 ? torqueForce : -torqueForce;
        weaponRB.AddTorque(torqueForce);

        bottleShadow = poolManager.GetOrCreateElement(shadowPrefab, this, effectsContainer.transform);
        bottleShadow.transform.position = transform.position;
        bottleShadow.transform.localScale = bottleShadowStartScale;

        if(gameObject.activeInHierarchy == true)
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(ScaleShadow());
        }

        isReadyToWork = true;

        Vector3 GetRandomPoint()
        {
            float x = Random.Range(-maxRadiusMovement, maxRadiusMovement);
            float y = Random.Range(-maxRadiusMovement, maxRadiusMovement);

            Vector3 resultPoint = Vector3.zero;

            if((x > -minRadiusMovement && x < minRadiusMovement) && (y > -minRadiusMovement && y < minRadiusMovement))
            {
                resultPoint = GetRandomPoint();
            }
            else
            {
                resultPoint = new Vector3(x, y, 0);
            }

            return resultPoint;
        }

        IEnumerator ScaleShadow()
        {
            while(true)
            {
                if(bottleShadow == null)
                {
                    break;
                }

                if(verticalVelocity > 0)
                {
                    bottleShadow.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                }
                else
                {
                    bottleShadow.transform.localScale += new Vector3(0.03f, 0.03f, 0.03f);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    #endregion

    private void OnBecameInvisible()
    {
        DestroyWeapon();
    }

    private void DestroyWeapon()
    {
        if(bottleShadow != null)
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            poolManager.DiscardByInstance(bottleShadow, this);
            bottleShadow = null;
        }

        if(unit != null && unit.unitWeapon == UnitsWeapon.Bible)
        {
            weaponStorage.isBibleWork = false;
        }

        EventManager.OnWeaponDestroyedEvent(weapon);
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += DestroyWeapon;
        currentLifeTime = 0;
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= DestroyWeapon;
    }
}
