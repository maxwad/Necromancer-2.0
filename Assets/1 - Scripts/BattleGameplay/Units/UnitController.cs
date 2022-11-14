using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class UnitController : MonoBehaviour
{
    public Unit unit;
    public float currentHealth;

    private float physicDefenceBase;
    private float physicDefence;
    private float magicDefenceBase;
    private float magicDefence;

    private BattleBoostManager boostManager;

    private bool isDead = false;
    private bool isImmortal = false;

    //Death section
    [HideInInspector]public SpriteRenderer unitSprite;
    private Color normalColor;
    private Color originalColor;
    private Color damageText;
    private Color damageColor = Color.red;
    private float blinkTime = 0.1f;

    [SerializeField] private GameObject deathPrefab;

    [SerializeField] private GameObject damageNote;
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

        unit = unitSource;
        currentHealth = unitSource.health;

        physicDefenceBase = unitSource.physicDefence;
        physicDefence = physicDefenceBase + physicDefenceBase * boostManager.GetBoost(BoostType.PhysicDefence);

        magicDefenceBase = unitSource.magicDefence;
        magicDefence = magicDefenceBase + magicDefenceBase * boostManager.GetBoost(BoostType.MagicDefence);

        unit.SetUnitController(this);
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
        GameObject damageObject = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.DamageText);
        damageObject.transform.position = transform.position;
        damageObject.SetActive(true);
        damageObject.GetComponent<DamageText>().Iniatilize(damageValue, colorDamage);
    }

    private void Dead()
    {
        isDead = true;
        GameObject death =  Instantiate(deathPrefab, transform.position, Quaternion.identity);
        death.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
        Destroy(gameObject);
    }

    private void MakeMeImmortal(bool mode = true)
    {
        isImmortal = mode;
    }

    #endregion

    public void KillOneUnit()
    {
        TakeDamage(currentHealth, currentHealth);
    }

    #region For Squad updating
    public void UpdateSquad(bool mode)
    {
        if(unitCountsText != null)
        {
            if(unit.quantity != 0) unitCountsText.text = unit.quantity.ToString();
        }

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

    private void OnEnable()
    {
        EventManager.SpellImmortal += MakeMeImmortal;
        EventManager.SetBattleBoost += UpgradeParameters;
        EventManager.Victory += Victory;

        MakeMeImmortal(false);
    }

    private void OnDisable()
    {
        EventManager.SpellImmortal -= MakeMeImmortal;
        EventManager.SetBattleBoost -= UpgradeParameters;
        EventManager.Victory -= Victory;
    }
}
