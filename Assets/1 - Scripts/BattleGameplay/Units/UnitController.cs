using UnityEngine;
using TMPro;
using static NameManager;
using System.Collections;
using System;

public class UnitController : MonoBehaviour
{
    private BattleBoostManager boostManager;
    private ObjectsPoolManager poolManager;
    private UnitManager unitManager;
    private AttackController attackController;

    [HideInInspector] public Unit unit;
    public float currentHealth;

    private float physicDefenceBase;
    private float physicDefence;
    private float magicDefenceBase;
    private float magicDefence;

    private int level;
    private float killToNextLevel;
    private float currentKillCount = 0;
    private float currentLevelBounds = 0;
    private bool stopLevelUp = false;

    [SerializeField] private Color scaleColorUsual;
    [SerializeField] private Color scaleColorNewLevel;
    private Coroutine blinkCoroutine;

    private bool isImmortal = false;

    [Header("DEAD")]
    private bool isDead = false;
    [HideInInspector]public SpriteRenderer unitSprite;
    public SpriteRenderer levelUpSprite;

    private Color normalColor;
    private Color originalColor;
    private Color damageText;
    private Color damageColor = Color.red;
    private float blinkTime = 0.1f;

    private Color colorDamage = Color.yellow;
    private Color criticalColor = Color.black;

    private TMP_Text unitCountsText;

    private void Start()
    {
        unitSprite = GetComponent<SpriteRenderer>();
        normalColor = unitSprite.color;
        originalColor = unitSprite.color;

        unitCountsText = GetComponentInChildren<TMP_Text>();
    }

    public void Initilize(Unit unitSource) 
    {
        if(boostManager == null) boostManager = GlobalStorage.instance.boostManager;

        int tempQuantity = 0;
        if(unit != null) tempQuantity = unit.quantity;

        unit = unitSource;
        currentHealth = unitSource.health;

        physicDefenceBase = unitSource.physicDefence;
        physicDefence = physicDefenceBase + physicDefenceBase * boostManager.GetBoost(BoostType.PhysicDefence);

        magicDefenceBase = unitSource.magicDefence;
        magicDefence = magicDefenceBase + magicDefenceBase * boostManager.GetBoost(BoostType.MagicDefence);

        unit.SetUnitController(this);
        if(tempQuantity != 0) unit.SetQuantity(tempQuantity);

        level = unitSource.level;
        killToNextLevel = unitSource.killToNextLevel;

        levelUpSprite.color = scaleColorUsual;
        currentKillCount = 0;
        currentLevelBounds = RecalculateLevelUpBound();

        //if(attackController == null) attackController = GetComponent<AttackController>();


        if(level != 1)
        {
            attackController.ReloadAttack(this);
            Debug.Log("Controller level " + unit.level);
            //Debug.Log("NEW LEVEL = " + level);
            //visual upgrade
        }
    }


    #region Damage

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagManager.T_ENEMY) == true && isDead != true)
        {
            if(isImmortal == true) return;

            Blink();
        }
    }

    public void TakeDamage(float physicalDamage, float magicDamage, bool isCritical = false)
    {
        if(isImmortal == true) return;

        float phDamageComponent = physicalDamage - physicDefence;
        if(phDamageComponent < 0) phDamageComponent = 0;

        float mDamageComponent = magicDamage - magicDefence;
        if(mDamageComponent < 0) mDamageComponent = 0;

        float damage = phDamageComponent + mDamageComponent;
        currentHealth -= damage;

        damageText = colorDamage;

        if(isCritical == true)
        {
            damage *= 2;
            damageText = criticalColor;
        }

        if(GlobalStorage.instance.isGlobalMode == false) ShowDamage(damage, damageText);

        if (currentHealth <= 0 && unit.quantity > 1)
        {
            unit.quantity--;
            currentHealth = unit.health;
            CheckColors();
            UpdateSquad(false);
        }

        if (unit.quantity <= 1 && currentHealth <= 0)
        {
            unit.quantity--;
            UpdateSquad(false);
            Dead();
        }
    }

    private void ShowDamage(float damageValue, Color colorDamage)
    {
        GameObject damageObject = poolManager.GetObject(ObjectPool.DamageText);
        damageObject.transform.position = transform.position;
        damageObject.SetActive(true);
        damageObject.GetComponent<DamageText>().Iniatilize(damageValue, colorDamage);
    }

    private void Dead()
    {
        isDead = true;
        GameObject death = poolManager.GetObject(ObjectPool.EnemyDeath);
        death.transform.position = transform.position;
        death.SetActive(true);
        Destroy(gameObject);
    }

    private void MakeMeImmortal(bool mode = true)
    {
        isImmortal = mode;
    }

    public void KillOneUnit()
    {
        TakeDamage(currentHealth, currentHealth);
    }

    #endregion


    #region SQUAD UPDATE

    public void UpdateSquad(bool mode)
    {
        if(unitCountsText != null)
        {
            if(unit.quantity != 0) unitCountsText.text = unit.quantity.ToString();
        }

        currentLevelBounds = RecalculateLevelUpBound();
        UpdateLevelUpScale();
        CheckNewLevel();

        if(mode == false) EventManager.OnWeLostOneUnitEvent(unit.unitType);
    }

    public UnitsTypes GetTypeUnit()
    {
        return unit.unitType;
    }

    public int GetQuantityUnit()
    {
        return unit.quantity;
    }

    #endregion

    private void UpgradeParameters(BoostType boost, float value)
    {
        if(boost == BoostType.PhysicDefence) physicDefence = physicDefenceBase + physicDefenceBase * value;
        if(boost == BoostType.MagicDefence) magicDefence = magicDefenceBase + magicDefenceBase * value;
    }

    private void Victory()
    {
        MakeMeImmortal(true);
    }

    private void IKilledEnemy(UnitsAbilities weapon)
    {
        if(isDead == false && weapon == unit.unitAbility && stopLevelUp == false)
        {
            currentKillCount++;
            UpdateLevelUpScale();
            CheckNewLevel();
        }
    }

    private float RecalculateLevelUpBound()
    {
        //return killToNextLevel * Mathf.Pow(level, 2) * unit.quantity;
        return 2 * Mathf.Pow(level, 2);
    }

    private void CheckNewLevel()
    {
        if(currentKillCount >= currentLevelBounds)
        {
            Unit newUnit = unitManager.GetNewSquad(unit.unitType, level + 1);
            if(newUnit != null) 
                Initilize(newUnit);
            else
                stopLevelUp = true;
        }
    }

    private void UpdateLevelUpScale()
    {
        float width = currentKillCount / currentLevelBounds;
        levelUpSprite.size = new Vector2(width, levelUpSprite.size.y);

        if(width < 1f)
        {
            BlinkLevel();
        }
        else
        {
            levelUpSprite.color = scaleColorNewLevel;
        }
    }

    private void ResetLevelUpBar()
    {
        if(blinkCoroutine != null) StopCoroutine(blinkCoroutine);
    }

    #region BLINK
    public void Blink()
    {
        unitSprite.color = damageColor;
        Invoke("ColorBack", blinkTime);
    }

    private void ColorBack()
    {
        CheckColors();
        unitSprite.color = normalColor;
    }

    private void CheckColors()
    {
        if(currentHealth > unit.health * 0.66f) normalColor = originalColor;

        if(currentHealth < unit.health * 0.66f) normalColor = Color.gray;

        if(currentHealth < unit.health * 0.33f) normalColor = Color.red;
    }

    public void BlinkLevel()
    {
        levelUpSprite.color = scaleColorNewLevel;
        blinkCoroutine = StartCoroutine(ColorBackLevel(scaleColorUsual));
    }

    private IEnumerator ColorBackLevel(Color normalColor)
    {
        //the bigger divider the slower animation
        float divider = 5;
        float time = 0;

        while(levelUpSprite.color != normalColor)
        {
            time += Time.deltaTime;
            levelUpSprite.color = Color.Lerp(levelUpSprite.color, normalColor, time / divider);
            yield return new WaitForSeconds(blinkTime);
        }
    }

    #endregion

    private void OnEnable()
    {
        EventManager.SpellImmortal += MakeMeImmortal;
        EventManager.SetBattleBoost += UpgradeParameters;
        EventManager.Victory += Victory;
        EventManager.EndOfBattle += ResetLevelUpBar;
        EventManager.KillEnemyBy += IKilledEnemy;

        if(unitManager == null)
        {
            poolManager = GlobalStorage.instance.objectsPoolManager;
            unitManager = GlobalStorage.instance.unitManager;
            attackController = GetComponent<AttackController>();
        }

        if(unit != null)
        {
            Unit newUnit = unitManager.GetNewSquad(unit.unitType);
            if(newUnit != null) Initilize(newUnit);
        }

        MakeMeImmortal(false);
        levelUpSprite.size = new Vector2(0, levelUpSprite.size.y);
    }

    private void OnDisable()
    {
        EventManager.SpellImmortal -= MakeMeImmortal;
        EventManager.SetBattleBoost -= UpgradeParameters;
        EventManager.Victory -= Victory;
        EventManager.EndOfBattle -= ResetLevelUpBar;
        EventManager.KillEnemyBy -= IKilledEnemy;
    }
}
