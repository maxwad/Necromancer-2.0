using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class UnitController : MonoBehaviour
{
    public UnitsTypes unitType;
    public string unitName;
    [SerializeField] private float health;
    [SerializeField] public float magicAttack;
    [SerializeField] public float physicAttack;
    [SerializeField] private float magicDefence;
    [SerializeField] private float physicDefence;
    [SerializeField] public float speedAttack;
    [SerializeField] public float size;
    [SerializeField] public int level;
    [SerializeField] public UnitsAbilities unitAbility;
    [SerializeField] public string abilityDescription;

    [SerializeField] public GameObject attackTool;

    public int quantity;

    public float currentHealth;

    private bool isDead = false;
    private bool isImmortal = false;

    //Death section
    [HideInInspector]public SpriteRenderer unitSprite;
    private Color normalColor;
    private Color originalColor;
    private Color damageColor = Color.red;
    private float blinkTime = 0.1f;

    [SerializeField] private GameObject deathPrefab;

    [SerializeField] GameObject damageNote;
    private Color colorDamage = Color.yellow;

    private TMP_Text unitCountsText;

    private void Start()
    {
        currentHealth = quantity > 0 ? health : 0;
        unitSprite = GetComponent<SpriteRenderer>();
        normalColor = unitSprite.color;
        originalColor = unitSprite.color;

        unitCountsText = GetComponentInChildren<TMP_Text>();
    }

    public void Initilize(Unit unit) 
    {
        unitType = unit.unitType;

        health        = unit.health;
        magicAttack   = unit.magicAttack;
        physicAttack  = unit.physicAttack;
        magicDefence  = unit.magicDefence;
        physicDefence = unit.physicDefence;
        speedAttack   = unit.speedAttack;
        size          = unit.size;
        level         = unit.level;

        unitAbility   = unit.unitAbility;
        attackTool    = unit.attackTool;
        abilityDescription = unit.abilityDescription;

        quantity      = unit.quantity;
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
        if(currentHealth > health * 0.66f) normalColor = originalColor;

        if(currentHealth < health * 0.66f) normalColor = Color.gray;

        if(currentHealth < health * 0.33f) normalColor = Color.red;
    }

    public void TakeDamage(float physicalDamage, float magicDamage)
    {
        if(isImmortal == true) return;

        //TODO: we need to create some damage formula
        float damage = Mathf.Round(physicalDamage + magicDamage);
        currentHealth -= damage;

        if(GlobalStorage.instance.isGlobalMode == false) ShowDamage(damage, colorDamage);  

        if (currentHealth <= 0 && quantity > 1)
        {
            quantity--;
            currentHealth = health;
            CheckColors();
            UpdateSquad(false);
        }

        if (quantity <= 1 && currentHealth <= 0)
        {
            quantity--;
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

    private void MakeMeImmortal(bool mode)
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
        if(quantity != 0) unitCountsText.text = quantity.ToString();

        if(mode == false) EventManager.OnWeLostOneUnitEvent(unitType, quantity);
    }

    public UnitsTypes GetTypeUnit()
    {
        return unitType;
    }

    public int GetQuantityUnit()
    {
        return quantity;
    }

    #endregion

    private void OnEnable()
    {
        EventManager.SpellImmortal += MakeMeImmortal;
    }

    private void OnDisable()
    {
        EventManager.SpellImmortal -= MakeMeImmortal;
    }
}
