using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyController : MonoBehaviour
{
    private Enemy originalStats;

    public EnemiesTypes enemiesType;
    public string enemyName;
    public Sprite icon;
    public float healthBase;
    public float health;

    public float magicAttackBase;
    public float magicAttack;

    public float physicAttackBase;
    public float physicAttack;

    public float magicDefenceBase;
    public float magicDefence;

    public float physicDefenceBase;
    public float physicDefence;

    public float speedAttackBase;
    public float speedAttack;

    public float size;
    public EnemyAbilities EnemyAbility;

    public GameObject attackTool;

    public float exp;
    public float luck = 3f;

    [SerializeField] private float currentHealth;
    private float delayAttack;
    private float bossDamageMultiplier;

    private float maxDamage = 300;
    private SpriteRenderer enemySprite;
    private Color normalColor = Color.white;
    private Color damageColor = Color.black;
    private float blinkTime = 0.1f;
    [HideInInspector] public bool isBoss = false;
    private float bossCreateMainMultiplier = 2f;
    private float bossCreateSecondMultiplier = 20f;

    [SerializeField] private GameObject damageNote;
    private Color colorDamage = Color.red;
    private Color criticalColor = Color.black;
    private Color damageText;

    private HeroController hero;
    private Rigidbody2D rbEnemy;
    private float pushForce = 4000f;

    public BonusType bonusType;

    private EnemyMovement movementScript;
    private BattleBoostManager boostManager;

    private void Awake()
    {
        rbEnemy = GetComponent<Rigidbody2D>();
        hero = GlobalStorage.instance.hero;

        currentHealth = healthBase;
        enemySprite = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        delayAttack -= Time.deltaTime;      
    }

    private void LateUpdate()
    {
        // we need check for death here because enemy can get much damage at the same time and activate few dead functions
        if(currentHealth <= 0) 
        {
            //BuildDebagger.instance.Show("Check health");
            Dead();

        }
    }

    public void Initialize(Enemy stats = null)
    {
        if(originalStats == null) originalStats = stats;

        enemiesType       = originalStats.EnemiesType;
        enemyName         = originalStats.enemyName;
        icon              = originalStats.enemyIcon;

        magicAttackBase   = originalStats.magicAttack;
        physicAttackBase  = originalStats.physicAttack;
        magicDefenceBase  = originalStats.magicDefence;
        physicDefenceBase = originalStats.physicDefence;
        speedAttackBase   = originalStats.speedAttack;
        size              = originalStats.size;

        EnemyAbility      = originalStats.EnemyAbility;
        attackTool        = originalStats.attackTool;

        exp               = originalStats.exp;

        healthBase        = originalStats.health;
        currentHealth     = healthBase;
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        //TODO: check depending of time
        if (delayAttack <= 0)
        {
            foreach(ContactPoint2D obj in collision.contacts)
            {
                bool isCriticalDamage = Random.Range(0, 100) < luck;
                if (obj.collider.gameObject.CompareTag(TagManager.T_PLAYER) == true)
                {
                    hero.TakeDamage(physicAttackBase, magicAttackBase, isCriticalDamage);
                    delayAttack = speedAttack;
                }

                if (obj.collider.gameObject.CompareTag(TagManager.T_SQUAD) == true)
                {
                    obj.collider.gameObject.GetComponent<UnitController>().TakeDamage(physicAttackBase, magicAttackBase, isCriticalDamage);
                    delayAttack = speedAttack;
                }
            }
        }                
    }

    public void TakeDamage(float physicalDamage, float magicDamage, Vector3 forceDirection, bool isCritical = false)
    {
        if(currentHealth > 0)
        {
            float phDamageComponent = physicalDamage - physicDefence;
            if(phDamageComponent < 0) phDamageComponent = 0;

            float mDamageComponent = magicDamage - magicDefence;
            if(mDamageComponent < 0) mDamageComponent = 0;

            float damage = phDamageComponent + mDamageComponent;

            if(isBoss == true) damage = damage + damage * bossDamageMultiplier;

            damageText = colorDamage;

            if(isCritical == true)
            {
                damage *= 2;
                damageText = criticalColor;
            }

            currentHealth -= damage;

            Blink();
            ShowDamage(damage, damageText);

            if(forceDirection != Vector3.zero) PushMe(forceDirection, pushForce);
            
        }   
    }

    public void Kill()
    {
        float damage = currentHealth + magicDefence + physicDefence;
        if(damage > maxDamage) damage = maxDamage;
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
        float multiplier = isBoss == true ? bossCreateSecondMultiplier : 1;

        if(currentHealth > healthBase * multiplier * 0.66f) normalColor = Color.white;

        if(currentHealth < healthBase * multiplier * 0.66f) normalColor = Color.gray;

        if(currentHealth < healthBase * multiplier * 0.33f) normalColor = Color.red;
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

        currentHealth = healthBase;
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
        isBoss               = true;
        currentHealth        *= bossCreateSecondMultiplier;
        magicAttackBase      *= bossCreateMainMultiplier;
        physicAttackBase     *= bossCreateMainMultiplier;
        transform.localScale *= bossCreateMainMultiplier;
        rbEnemy.mass         *= bossCreateMainMultiplier;
        exp                  *= bossCreateSecondMultiplier;

        movementScript.BoostSpeed(0.2f);
        BossController bossController = gameObject.AddComponent<BossController>();
        bossController.Init();
    }    

    private void ReturnBossToOrdinaryEnemy()
    {
        isBoss               = false;
        currentHealth        /= bossCreateSecondMultiplier;
        magicAttackBase      /= bossCreateMainMultiplier;
        physicAttackBase     /= bossCreateMainMultiplier;
        transform.localScale /= bossCreateMainMultiplier;
        rbEnemy.mass         /= bossCreateMainMultiplier;
        exp                  /= bossCreateSecondMultiplier;

        gameObject.GetComponent<BossController>().StopSpelling();
        Destroy(gameObject.GetComponent<BossController>());
    }

    private void BackToPool()
    {
        ResetEnemy();
    }

    private void UpgradeParameters(BoostType boost, float value)
    {
        if(boost == BoostType.EnemyHealth) health = healthBase + healthBase * value;
        if(boost == BoostType.EnemyPhysicAttack) physicAttack = physicAttackBase + physicAttackBase * value;
        if(boost == BoostType.EnemyPhysicDefence) physicDefence = physicDefenceBase + physicDefenceBase * value;
        if(boost == BoostType.EnemyMagicAttack) magicAttack = magicAttackBase + magicAttackBase * value;
        if(boost == BoostType.EnemyMagicDefence) magicDefence = magicDefenceBase + magicDefenceBase * value;
        if(boost == BoostType.EnemyCoolDown) speedAttack = speedAttackBase + speedAttackBase * value;
        if(boost == BoostType.BossDamade) bossDamageMultiplier = value;
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += BackToPool;
        EventManager.SetBattleBoost += UpgradeParameters;

        if(boostManager == null) boostManager = GlobalStorage.instance.unitBoostManager;

        health = healthBase + healthBase * boostManager.GetBoost(BoostType.EnemyHealth);
        currentHealth = health;
        physicAttack = physicAttackBase + physicAttackBase * boostManager.GetBoost(BoostType.EnemyPhysicAttack);
        physicDefence = physicDefenceBase + physicDefenceBase * boostManager.GetBoost(BoostType.EnemyPhysicDefence);
        magicAttack = magicAttackBase + magicAttackBase * boostManager.GetBoost(BoostType.EnemyMagicAttack);
        magicDefence = magicDefenceBase + magicDefenceBase * boostManager.GetBoost(BoostType.EnemyMagicDefence);
        speedAttack = speedAttackBase + speedAttackBase * boostManager.GetBoost(BoostType.EnemyCoolDown);
        bossDamageMultiplier = boostManager.GetBoost(BoostType.BossDamade);

        delayAttack = speedAttack;

        //BuildDebagger.instance.Show("Enable with " + health + " health"); 
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= BackToPool;
        EventManager.SetBattleBoost -= UpgradeParameters;
    }
}
