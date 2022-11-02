using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public bool isBoss = false;
    private Enemy originalStats;

    public EnemiesTypes enemiesType;
    public string enemyName;
    public Sprite icon;
    public float health;
    public float magicAttack;
    public float physicAttack;
    public float magicDefence;
    public float physicDefence;
    public float speedAttack;
    public float size;
    public EnemyAbilities EnemyAbility;

    public GameObject attackTool;

    public int exp;

    [SerializeField] private float currentHealth;
    private float delayAttack;

    private float maxDamage = 200;
    private SpriteRenderer enemySprite;
    private Color normalColor = Color.white;
    private Color damageColor = Color.black;
    private float blinkTime = 0.1f;

    [SerializeField] private GameObject damageNote;
    private Color colorDamage = Color.red;

    private HeroController hero;
    private Rigidbody2D rbEnemy;
    private float pushForce = 4000f;

    public BonusType bonusType;

    private EnemyMovement movementScript;

    private void Awake()
    {
        rbEnemy = GetComponent<Rigidbody2D>();
        hero = GlobalStorage.instance.hero;

        currentHealth = health;
        delayAttack = speedAttack;
        enemySprite = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        delayAttack -= Time.deltaTime;

        if(isBoss)
        {
           // Debug.Log(exp);
        }        
    }

    private void LateUpdate()
    {
        // we need check for death here because enemy can get much damage at the same time and activate few dead functions
        if(currentHealth <= 0) Dead();
    }

    public void Initialize(Enemy stats = null)
    {
        if(originalStats == null) originalStats = stats;

        enemiesType   = originalStats.EnemiesType;
        enemyName     = originalStats.enemyName;
        icon          = originalStats.enemyIcon;

        health        = originalStats.health;
        magicAttack   = originalStats.magicAttack;
        physicAttack  = originalStats.physicAttack;
        magicDefence  = originalStats.magicDefence;
        physicDefence = originalStats.physicDefence;
        speedAttack   = originalStats.speedAttack;
        size          = originalStats.size;

        EnemyAbility  = originalStats.EnemyAbility;
        attackTool    = originalStats.attackTool;

        exp           = originalStats.exp;
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        //TODO: check depending of time
        if (delayAttack <= 0)
        {
            foreach(ContactPoint2D obj in collision.contacts)
            {                    
                if (obj.collider.gameObject.CompareTag(TagManager.T_PLAYER) == true)
                {
                    hero.TakeDamage(physicAttack, magicAttack);
                    delayAttack = speedAttack;
                }

                if (obj.collider.gameObject.CompareTag(TagManager.T_SQUAD) == true)
                {
                    obj.collider.gameObject.GetComponent<UnitController>().TakeDamage(physicAttack, magicAttack);
                    delayAttack = speedAttack;
                }
            }
        }                
    }

    public void TakeDamage(float physicalDamage, float magicDamage, Vector3 forceDirection)
    {
        if(currentHealth > 0)
        {            
            //TODO: we need to create some damage formula
            float damage = physicalDamage + magicDamage;
            currentHealth -= damage;

            Blink();
            ShowDamage(damage, colorDamage);

            if(forceDirection != Vector3.zero) PushMe(forceDirection, pushForce);
            
        }   
    }

    public void Kill()
    {
        float damage = currentHealth > maxDamage ? maxDamage : currentHealth;
        TakeDamage(damage, damage, Vector3.zero);
    }

    public void Blink()
    {
        enemySprite.color = damageColor;
        Invoke("ColorBack", blinkTime);
    }

    private void ColorBack()
    {
        CheckColors();
        enemySprite.color = normalColor;
    }

    private void CheckColors()
    {
        float multiplier = isBoss == true ? 50 : 1;

        if(currentHealth > health * multiplier * 0.66f) normalColor = Color.white;

        if(currentHealth < health * multiplier * 0.66f) normalColor = Color.gray;

        if(currentHealth < health * multiplier * 0.33f) normalColor = Color.red;
    }

    public void PushMe(Vector3 direction, float force)
    {
        Vector3 kickForce = (transform.position - direction).normalized * force;
        rbEnemy.AddForce(kickForce, ForceMode2D.Force);
    }

    private void ShowDamage(float damageValue, Color colorDamage)
    {
        GameObject damageObject = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.DamageText);
        damageObject.transform.position = transform.position;
        damageObject.SetActive(true);
        damageObject.GetComponent<DamageText>().Iniatilize(damageValue, colorDamage);
    }

    private void Dead()
    {           
        GameObject death = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.EnemyDeath);
        death.transform.position = transform.position;
        death.SetActive(true);

        GameObject bloodSpot = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.BloodSpot);
        bloodSpot.transform.position = transform.position;
        bloodSpot.SetActive(true);

        CreateBonus();

        EventManager.OnEnemyDestroyedEvent(gameObject);

        ResetEnemy();
    }

    private void ResetEnemy()
    {
        if(isBoss == true)
        {
            ReturnBossToOrdinaryEnemy();
            //some event
        }

        currentHealth = health;
        ColorBack();
        movementScript.MakeMeFixed(false);
        movementScript.StopMoving(false);
        movementScript.ResetSpeed();

        gameObject.SetActive(false);
    }

    private void CreateBonus()
    {
        GlobalStorage.instance.bonusManager.CreateBonus(isBoss, bonusType, transform.position, exp);
    }

    public void MakeBoss() 
    {
        isBoss                = true;
        currentHealth        *= 20;
        magicAttack          *= 3;
        physicAttack         *= 3;
        transform.localScale *= 2;
        rbEnemy.mass         *= 2;
        exp                  *= 20;

        movementScript.BoostSpeed(0.2f);
        BossController bossController = gameObject.AddComponent<BossController>();
        bossController.Init();
    }    

    private void ReturnBossToOrdinaryEnemy()
    {
        isBoss                = false;
        currentHealth        /= 50;
        magicAttack          /= 3;
        physicAttack         /= 3;
        transform.localScale /= 2;
        rbEnemy.mass         /= 2;
        exp                  /= 50;

        gameObject.GetComponent<BossController>().StopSpelling();
        Destroy(gameObject.GetComponent<BossController>());
    }


    private void BackToPool()
    {
        ResetEnemy();
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += BackToPool;
    }
    private void OnDisable()
    {
        EventManager.EndOfBattle -= BackToPool;
    }
}
