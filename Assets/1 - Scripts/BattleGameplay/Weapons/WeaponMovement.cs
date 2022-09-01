using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class WeaponMovement : MonoBehaviour
{
    UnitController controller;
    WeaponStorage weaponStorage;
    private bool isReadyToWork = false;

    private Rigidbody2D rbWeapon;

    private SpriteRenderer bible;

    [SerializeField] private GameObject bottleDeath;
    [SerializeField] private GameObject bottleShadowPrefab;
    private GameObject bottleShadow;
    private Vector3 groundVelocity;
    private float verticalVelocity;
    private float gravity = -10;

    public float speed = 1;
    private SpriteRenderer unitSprite;


    private void Update()
    {
        if(isReadyToWork == true) WeaponMoving();
    }

    private void WeaponMoving()
    {
        switch(controller.unitAbility)
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
    public void SetSettings(UnitController unitController, WeaponStorage weaponSource)
    {
        controller = unitController;
        unitSprite = unitController.unitSprite;
        weaponStorage = weaponSource;
    }

    public void ActivateWeapon(UnitController unitController, int index = 0)
    {
        switch(unitController.unitAbility)
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
                ActivateBible();
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

    private void OnBecameInvisible()
    {
        if(bottleShadow != null) Destroy(bottleShadow);

        Destroy(gameObject);
    }

    #endregion

    #region Movement

    private void SpearMovement()
    {
        rbWeapon.velocity = -rbWeapon.transform.right * speed;
    }

    private void BibleMovement()
    {
        transform.RotateAround(transform.position, Vector3.forward, speed * Time.deltaTime);
        bible.transform.RotateAround(bible.transform.position, Vector3.forward, -speed * Time.deltaTime);

        //reset EnemyList every cycle
        if(transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 5f)
        {
            GetComponent<WeaponDamage>().ClearEnemyList();
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
            GameObject death = Instantiate(bottleDeath, transform.position, Quaternion.identity);
            death.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
            PrefabSettings settings = death.GetComponent<PrefabSettings>();

            if(settings != null) settings.SetSettings(
                sortingLayer: TagManager.T_PLAYER, 
                sortingOrder: 11, 
                color: UnityEngine.Color.cyan, 
                size: controller.size * 2,
                animationSpeed: 0.07f);

            Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, controller.size * 2);
            foreach(Collider2D obj in objects)
            {
                if(obj.CompareTag(TagManager.T_ENEMY) == true)
                    obj.GetComponent<EnemyController>().TakeDamage(controller.physicAttack, controller.magicAttack, transform.position);
            }

            Destroy(bottleShadow);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Activators
    private void ActivateGarlic()
    {
        StartCoroutine(Resize());

        IEnumerator Resize()
        {
            float lifetime = controller.speedAttack;
            float currentSize;
            float sizeStep = 0.075f;

              
                gameObject.transform.localScale = new Vector3(0, 0, 0);
                currentSize = 0;
                GetComponent<WeaponDamage>().ClearEnemyList();                

                while(currentSize <= lifetime)
                {
                    yield return new WaitForSeconds(0.005f);
                    currentSize += sizeStep;

                    gameObject.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
                }
                gameObject.transform.localScale = new Vector3(0, 0, 0);
                //yield return new WaitForSeconds(controller.speedAttack);
                       
        }
    }

    private void ActivateAxe(int numberOfWeapon)
    {
        float force = 30f;
        float torqueForce = -200;

        if(controller.level == 2)
        {
            if(numberOfWeapon == 1) torqueForce = -torqueForce;
        }

        if(controller.level == 3)        {

            if(numberOfWeapon == 3) torqueForce = -torqueForce;
        }

        Rigidbody2D rbAxe = GetComponent<Rigidbody2D>();

        rbAxe.AddForce(rbAxe.transform.up * force, ForceMode2D.Impulse);
        rbAxe.AddTorque(torqueForce);
    }

    private void ActivateSpear()
    {
        rbWeapon = GetComponent<Rigidbody2D>();

        float searchRadius = 25f;
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

    private void ActivateBible()
    {
        bible = GetComponentInChildren<SpriteRenderer>();
        isReadyToWork = true;
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

        bottleShadow = Instantiate(bottleShadowPrefab, transform.position, Quaternion.identity);
        bottleShadow.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);

        StartCoroutine(ScaleShadow());

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
                if(verticalVelocity > 0)
                    bottleShadow.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                else
                    bottleShadow.transform.localScale += new Vector3(0.03f, 0.03f, 0.03f);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    #endregion

    //private void DestroyWeapon(bool mode)
    //{
    //    if(mode == true) Destroy(gameObject);
    //}

    //private void OnEnable()
    //{
    //    EventManager.ChangePlayer += DestroyWeapon;
    //}

    //private void OnDisable()
    //{
    //    EventManager.ChangePlayer += DestroyWeapon;
    //}
}
