using UnityEngine;
using TMPro;
using static Enums;
using System.Collections;
using System;
using Zenject;

public class UnitController : MonoBehaviour
{
    private BoostManager boostManager;
    private ObjectsPoolManager poolManager;
    private PlayersArmy playersArmy;
    private AttackController attackController;

    [HideInInspector] public Unit unit;
    public int quantity = 1;
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

    [Inject]
    public void Construct
    (
    BoostManager boostManager,
    ObjectsPoolManager poolManager,
    PlayersArmy playersArmy
    )
    {
        this.boostManager = boostManager;
        this.poolManager = poolManager;
        this.playersArmy = playersArmy;

        attackController = GetComponent<AttackController>();
        unitCountsText = GetComponentInChildren<TMP_Text>();
        unitSprite = GetComponent<SpriteRenderer>();
        normalColor = unitSprite.color;
        originalColor = unitSprite.color;
    }

    public void Activate()
    {
        isDead = false;
        stopLevelUp = false;
    }

    public void Initilize(Unit unitSource) 
    {
        unit = unitSource;
        currentHealth = unitSource.health;

        physicDefenceBase = unitSource.physicDefence;
        physicDefence = physicDefenceBase + physicDefenceBase * boostManager.GetBoost(BoostType.PhysicDefence);

        magicDefenceBase = unitSource.magicDefence;
        magicDefence = magicDefenceBase + magicDefenceBase * boostManager.GetBoost(BoostType.MagicDefence);

        unit.SetUnitController(this);

        level = unitSource.level;
        killToNextLevel = unitSource.killToNextLevel;

        levelUpSprite.color = scaleColorUsual;
        currentKillCount = 0;

        if(playersArmy.CheckNextUnitLevel(unit.unitType) == false)
        {
            stopLevelUp = true;
            levelUpSprite.size = new Vector2(0, levelUpSprite.size.y);
        }
        else
        {
            RecalculateLevelUpBound();
            UpdateLevelUpScale();
        }

        attackController.ReloadAttack(unit);
        CheckColors();

        if(level != 1) ShowEffectLevelUp();
    }

    public void AddQuantity(int amount)
    {
        if(quantity == 0) ResetSquad();

        quantity += amount;
        unitCountsText.text = quantity.ToString();
        if(quantity != 0) UpdateLevel();
    }

    #region Damage

    public void TakeDamage(float physicalDamage, float magicDamage, bool isCritical = false)
    {
        if(isDead == true) return;

        float phDamageComponent = physicalDamage - physicDefence;
        if(phDamageComponent < 0) phDamageComponent = 0;

        float mDamageComponent = magicDamage - magicDefence;
        if(mDamageComponent < 0) mDamageComponent = 0;

        float damage = phDamageComponent + mDamageComponent;
        if(isImmortal == true) damage = 0;

        currentHealth -= damage;

        damageText = colorDamage;

        if(isCritical == true)
        {
            damage *= 2;
            damageText = criticalColor;
        }

        if (currentHealth <= 0)
        {
            currentHealth = unit.health;
            playersArmy.SquadLost(unit.unitType);
        }

        ShowDamage(damage, damageText);
    }

    private void ShowDamage(float damageValue, Color colorDamage)
    {
        Blink();

        GameObject damageObject = poolManager.GetObject(ObjectPool.DamageText);
        damageObject.transform.position = transform.position;
        damageObject.SetActive(true);
        damageObject.GetComponent<DamageText>().Iniatilize(damageValue, colorDamage);
    }

    public void Dead()
    {
        isDead = true;
        GameObject death = poolManager.GetObject(ObjectPool.EnemyDeath);
        death.transform.position = transform.position;
        death.SetActive(true);
    }

    private void MakeMeImmortal(bool mode = true)
    {
        isImmortal = mode;
    }

    #endregion

    private void UpgradeParameters(BoostType boost, float value)
    {
        if(boost == BoostType.PhysicDefence) 
            physicDefence = physicDefenceBase + physicDefenceBase * value;

        if(boost == BoostType.MagicDefence) 
            magicDefence = magicDefenceBase + magicDefenceBase * value;
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
            UpdateLevel();
        }
    }

    private void UpdateLevel()
    {
        RecalculateLevelUpBound();

        if(currentKillCount >= currentLevelBounds)
        {
            CheckNewLevel();
        }
        else
        {
            UpdateLevelUpScale();
        }

    }

    private void RecalculateLevelUpBound()
    {
        currentLevelBounds = killToNextLevel * Mathf.Pow(level, 2) * quantity;
    }

    private void CheckNewLevel()
    {
        if(playersArmy.CheckNextUnitLevel(unit.unitType) == false)
        {
            stopLevelUp = true;
            levelUpSprite.size = new Vector2(0, levelUpSprite.size.y);
        }
        else
        {
            stopLevelUp = false;
            playersArmy.UpgradeSquad(unit.unitType);
        }

        currentKillCount = 0;
    }

    private void UpdateLevelUpScale()
    {
        float width = currentKillCount / currentLevelBounds;

        if(width < 1f)
        {
            BlinkOfLevel();
            levelUpSprite.size = new Vector2(width, levelUpSprite.size.y);
        }
    }

    private void ResetSquad(bool mode = false)
    {
        levelUpSprite.size = new Vector2(0, levelUpSprite.size.y);
        stopLevelUp = false;
        currentKillCount = 0;

        if(mode == true)
        {
            if(blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        }
    }

    public void ShowEffectRessurection(int amount)
    {
        BonusTipUIManager.ShowVisualEffectInBattle(transform.position, VisualEffects.Resurection, amount, mark: "+");
    }

    public void ShowEffectDeath()
    {
        BonusTipUIManager.ShowVisualEffectInBattle(transform.position, VisualEffects.Death);
    }

    public void ShowEffectLevelUp()
    {
        BonusTipUIManager.ShowVisualEffectInBattle(transform.position, VisualEffects.LevelUp, text: "LevelUp!");
    }

    #region BLINK
    public void Blink()
    {
        unitSprite.color = damageColor;
        Invoke("ColorBack", blinkTime);
    }

    //for Invoke
    private void ColorBack()
    {
        CheckColors();
        unitSprite.color = normalColor;
    }

    private void CheckColors()
    {
        if(currentHealth > unit.health * 0.66f) normalColor = originalColor;

        if(currentHealth <= unit.health * 0.66f) normalColor = Color.gray;

        if(currentHealth <= unit.health * 0.33f) normalColor = Color.red;
    }

    public void BlinkOfLevel()
    {
        if(gameObject.activeInHierarchy == true)
        {
            levelUpSprite.color = scaleColorNewLevel;
            blinkCoroutine = StartCoroutine(ColorBackLevel(scaleColorUsual));
        }
    }

    private IEnumerator ColorBackLevel(Color normalColor)
    {
        WaitForSeconds delay = new WaitForSeconds(blinkTime);
        float time = 0;

        while(levelUpSprite.color != normalColor)
        {
            time += Time.deltaTime;
            levelUpSprite.color = Color.Lerp(levelUpSprite.color, normalColor, time);
            yield return delay;
        }
    }

    #endregion

    private void OnEnable()
    {
        EventManager.SpellImmortal += MakeMeImmortal;
        EventManager.SetBattleBoost += UpgradeParameters;
        EventManager.Victory += Victory;
        EventManager.SwitchPlayer += ResetSquad;
        EventManager.KillEnemyBy += IKilledEnemy;

        unitCountsText.text = quantity.ToString();
        MakeMeImmortal(false);
        isDead = false;
    }

    private void OnDisable()
    {
        EventManager.SpellImmortal -= MakeMeImmortal;
        EventManager.SetBattleBoost -= UpgradeParameters;
        EventManager.Victory -= Victory;
        EventManager.SwitchPlayer -= ResetSquad;
        EventManager.KillEnemyBy -= IKilledEnemy;
    }
}
