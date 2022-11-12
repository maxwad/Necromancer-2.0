using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyController : MonoBehaviour
{
    private EnemySO originalStats;

    public EnemiesTypes enemiesType;
    public string enemyName;
    public Sprite icon;

    public float healthBase;
    public float health;
    public float mAttack;
    public float pAttack;
    public float mDefence;
    public float pDefence;
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
    private SpriteRenderer enemySpriteRenderer;
    private Sprite enemySprite;
    private Color normalColor = Color.white;
    private Color damageColor = Color.black;
    private float blinkTime = 0.1f;
    [HideInInspector] public bool isBoss = false;
    private BossController bossController;
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
    public EnemyManager enemyManager;

    private void Awake()
    {
        rbEnemy = GetComponent<Rigidbody2D>();
        hero = GlobalStorage.instance.hero;
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        enemySprite = enemySpriteRenderer.sprite;
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

    public void InitializeParameters()
    {
        if(originalStats == null) return;

        if(isBoss == true)
        {
            isBoss = false;

            bossController = GetComponent<BossController>();
            if(bossController != null) Destroy(bossController);

            EnemyBossWeapons bossWeapons = GetComponent<EnemyBossWeapons>();
            if(bossWeapons != null) Destroy(bossWeapons);


            Debug.Log("Reset");
        }

        enemyName   = originalStats.enemyName;
        icon        = originalStats.enemyIcon;

        pAttack     = originalStats.physicAttack + originalStats.physicAttack * boostManager.GetBoost(BoostType.EnemyPhysicAttack);
        pDefence    = originalStats.physicDefence + originalStats.physicDefence * boostManager.GetBoost(BoostType.EnemyPhysicDefence);
        mAttack     = originalStats.magicAttack + originalStats.magicAttack * boostManager.GetBoost(BoostType.EnemyMagicAttack);
        mDefence    = originalStats.magicDefence + originalStats.magicDefence * boostManager.GetBoost(BoostType.EnemyMagicDefence);
        speedAttack = originalStats.speedAttack + originalStats.speedAttack * boostManager.GetBoost(BoostType.EnemyCoolDown);
        delayAttack = speedAttack;

        size        = originalStats.size;
        exp         = originalStats.exp;

        health      = originalStats.health + originalStats.health * boostManager.GetBoost(BoostType.EnemyHealth);
        currentHealth = health;
        transform.localScale = new Vector3(size, size, size);

        bossDamageMultiplier = boostManager.GetBoost(BoostType.BossDamade);


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
                    hero.TakeDamage(pAttack, mAttack, isCriticalDamage);
                    delayAttack = speedAttack;
                }

                if (obj.collider.gameObject.CompareTag(TagManager.T_SQUAD) == true)
                {
                    obj.collider.gameObject.GetComponent<UnitController>().TakeDamage(pAttack, mAttack, isCriticalDamage);
                    delayAttack = speedAttack;
                }
            }
        }                
    }

    public void TakeDamage(float physicalDamage, float magicDamage, Vector3 forceDirection, bool isCritical = false)
    {
        if(currentHealth > 0)
        {
            float phDamageComponent = physicalDamage - pDefence;
            if(phDamageComponent < 0) phDamageComponent = 0;

            float mDamageComponent = magicDamage - mDefence;
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

            if(isBoss == true && bossController != null)
            {
                bossController.UpdateBossHealth(currentHealth > 0 ? currentHealth : 0);
            }

            Blink();
            ShowDamage(damage, damageText);

            if(forceDirection != Vector3.zero) PushMe(forceDirection, pushForce);
            
        }   
    }

    public void Kill()
    {
        float damage = currentHealth + mDefence + pDefence;
        if(damage > maxDamage) damage = maxDamage;
        TakeDamage(damage, damage, Vector3.zero);
    }

    public void Blink()
    {
        enemySpriteRenderer.color = damageColor;
        Invoke("ColorBack", blinkTime);
    }

    private void ColorBack()
    {
        CheckColors();
        enemySpriteRenderer.color = normalColor;
    }

    private void CheckColors()
    {
        float multiplier = isBoss == true ? bossCreateSecondMultiplier : 1;

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
        if(isBoss == true) ReturnBossToOrdinaryEnemy();

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
        isBoss               = true;
        currentHealth        *= bossCreateSecondMultiplier;
        mAttack              *= bossCreateMainMultiplier;
        pAttack             *= bossCreateMainMultiplier;
        transform.localScale *= bossCreateMainMultiplier;
        rbEnemy.mass         *= bossCreateMainMultiplier;
        exp                  *= bossCreateSecondMultiplier;

        movementScript.BoostSpeed(0.2f);
        bossController = gameObject.AddComponent<BossController>();
        bossController.Init(currentHealth, enemySprite);
    }    

    private void ReturnBossToOrdinaryEnemy()
    {
        isBoss               = false;
        currentHealth        /= bossCreateSecondMultiplier;
        mAttack              /= bossCreateMainMultiplier;
        pAttack              /= bossCreateMainMultiplier;
        transform.localScale /= bossCreateMainMultiplier;
        rbEnemy.mass         /= bossCreateMainMultiplier;
        exp                  /= bossCreateSecondMultiplier;

        if(bossController != null)
        {
            bool runeClear = (currentHealth <= 0) ? true : false;
            bossController.BossDeath(runeClear);
            Destroy(bossController);
        }
    }

    private void BackToPool()
    {
        ResetEnemy();
    }

    private void UpgradeParameters(BoostType boost, float value)
    {
        if(boost == BoostType.EnemyHealth)
        {
            health = originalStats.health + originalStats.health * value;
            if(isBoss == false && currentHealth > health) currentHealth = health;
            //Debug.Log("Health after Upgrade = " + currentHealth);
        }

        if(boost == BoostType.EnemyPhysicAttack) pAttack       = originalStats.physicAttack + originalStats.physicAttack * value;
        if(boost == BoostType.EnemyPhysicDefence) pDefence     = originalStats.physicDefence + originalStats.physicDefence * value;
        if(boost == BoostType.EnemyMagicAttack) mAttack        = originalStats.magicAttack + originalStats.magicAttack * value;
        if(boost == BoostType.EnemyMagicDefence) mDefence      = originalStats.magicDefence + originalStats.magicDefence * value;
        if(boost == BoostType.EnemyCoolDown) speedAttack       = originalStats.speedAttack + originalStats.speedAttack * value;
        if(boost == BoostType.BossDamade) bossDamageMultiplier = value;
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += BackToPool;
        EventManager.SetBattleBoost += UpgradeParameters;

        if(boostManager == null)
        {
            enemyManager = GlobalStorage.instance.enemyManager;
            boostManager = GlobalStorage.instance.boostManager;
        }

        if(originalStats == null) originalStats = enemyManager.GetEnemySO(enemiesType);
        InitializeParameters();
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= BackToPool;
        EventManager.SetBattleBoost -= UpgradeParameters;
    }
}
