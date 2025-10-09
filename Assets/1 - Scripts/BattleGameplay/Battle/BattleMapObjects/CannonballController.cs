using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CannonballController : MonoBehaviour
{
    [SerializeField] private float minPlayerDamage = 2f;
    [SerializeField] private float maxPlayerDamage = 5f;
    [SerializeField] private float minEnemyDamage = 1f;
    [SerializeField] private float maxEnemyDamage = 2f;
    [SerializeField] private float radiusExplosion = 1f;
    [SerializeField] private float verticalVelocity = 4f;
    [SerializeField] private GameObject bottleShadowPrefab;
    [SerializeField] private Vector3 bottleShadowStartScale = Vector3.one;

    private Vector3 playersPosition;
    private Vector3 groundVelocity;
    private Vector3 shadowGroundVelocity;
    private Vector3 shadowOffset = new Vector3(0f, 5f, 0f);
    private float speed = 4f;
    private float currentVerticalVelocity;
    private float gravity = -10;
    private SpriteRenderer cannonSprite;
    private SpriteRenderer shadowSprite;
    private Rigidbody2D rbCannon;
    private GameObject bottleShadow;

    private bool canIMove = false;

    [SerializeField] private GameObject death;

    private Coroutine coroutine;
    private ObjectsPoolManager poolManager;
    private GameObject effectsContainer;

    [Inject]
    public void Construct([Inject(Id = Constants.EFFECTS_CONTAINER)] GameObject effectsContainer, ObjectsPoolManager poolManager)
    {
        this.effectsContainer = effectsContainer;
        this.poolManager = poolManager;

        cannonSprite = GetComponent<SpriteRenderer>();
        rbCannon = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(canIMove == true) 
            Movement();
    }

    public void Initialize(Vector3 point)
    {
        playersPosition = point;
        canIMove = true;

        ActivateCannon();
    }

    private void ActivateCannon()
    {
        currentVerticalVelocity = verticalVelocity;
        groundVelocity = (playersPosition - transform.position).normalized * speed;
        shadowGroundVelocity = (playersPosition + shadowOffset - transform.position).normalized * speed;

        bottleShadow = poolManager.GetUnusualPrefab(bottleShadowPrefab);
        bottleShadow.transform.position = transform.position - shadowOffset;
        bottleShadow.transform.localScale = bottleShadowStartScale;
        bottleShadow.transform.SetParent(effectsContainer.transform);
        bottleShadow.SetActive(true);
        shadowSprite = bottleShadow.GetComponent<SpriteRenderer>();

        if(gameObject.activeInHierarchy == true)
        {
            if(coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(ScaleShadow());
        }

        IEnumerator ScaleShadow()
        {
            while(true)
            {
                if(bottleShadow == null)
                    break;

                if(currentVerticalVelocity > 0)
                    bottleShadow.transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);
                else
                    bottleShadow.transform.localScale += new Vector3(0.03f, 0.03f, 0.03f);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void Movement()
    {        
        cannonSprite.sortingOrder = (int)playersPosition.y * 100;
        shadowSprite.sortingOrder = (int)playersPosition.y * 100;

        currentVerticalVelocity += gravity * Time.deltaTime;
        rbCannon.transform.position += (groundVelocity + new Vector3(0, currentVerticalVelocity, 0)) * Time.deltaTime;

        bottleShadow.transform.position = new Vector3(rbCannon.transform.position.x, bottleShadow.transform.position.y + shadowGroundVelocity.y * Time.deltaTime, transform.position.z);

        if(rbCannon.transform.position.y < bottleShadow.transform.position.y)
        {
            Explosion();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_PLAYER) == true)
        {
            if(currentVerticalVelocity < 0)
            {
                Explosion();
            }
        }
    }

    private void Explosion()
    {
        CreateEffect();

        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, radiusExplosion);
        foreach(Collider2D obj in objects)
        {
            if(obj.CompareTag(TagManager.T_PLAYER) == true )
            {
                float damage = Random.Range(minPlayerDamage, maxPlayerDamage);
                obj.GetComponent<HeroController>().TakeDamage(damage / 2, damage / 2);
            }

            if(obj.CompareTag(TagManager.T_SQUAD) == true)
            {
                float damage = Random.Range(minPlayerDamage, maxPlayerDamage);
                obj.GetComponent<UnitController>().TakeDamage(damage / 2, damage / 2);
            }

            if(obj.CompareTag(TagManager.T_ENEMY) == true)
            {
                float damage = Random.Range(minEnemyDamage, maxEnemyDamage);
                obj.GetComponent<EnemyController>().TakeDamage(damage / 2, damage / 2, transform.position);
            }
        }

        gameObject.SetActive(false);
        bottleShadow.SetActive(false);
    }

    private void CreateEffect()
    {
        GameObject dust = poolManager.GetUnusualPrefab(death);
        dust.transform.position = transform.position;
        dust.transform.SetParent(effectsContainer.transform);
        dust.SetActive(true);

        SimpleAnimationObject settings = dust.GetComponent<SimpleAnimationObject>();
        if(settings != null)
            settings.SetSettings(color: Color.gray, sortingOrder: -(int)transform.position.y * 100, sortingLayer: TagManager.T_PLAYER, animationSpeed: 0.1f);
    }
}
