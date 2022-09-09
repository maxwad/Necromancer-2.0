using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class HeroController : MonoBehaviour
{
    [Header("Level up system")]
    [SerializeField] private float currentTempLevel;
    [SerializeField] public float currentMaxLevel;

    [SerializeField] private float standartTempExpRate = 10;
    [SerializeField] private float levelMultiplierRate = 1;
    [SerializeField] private float currentTempExpGoal;
    [SerializeField] private float currentTempExp;
    private bool isLevelUpComplete = false;

    [Header("Characteristics")]
    [SerializeField] private float maxCurrentHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float maxCurrentMana;
    public float currentMana;

    [SerializeField] private float searchRadius;

    [SerializeField] private float defence;

    [SerializeField] private float luck;

    private PlayerStats playerStatsScript;

    [Space]
    [HideInInspector] public bool isDead = false;
    [Space]
    private SpriteRenderer unitSprite;
    private Color normalColor;
    private Color damageColor = Color.red;
    private float blinkTime = 0.1f;

    [SerializeField] private GameObject deathPrefab;

    [Space]
    private BattleUIManager battleUIManager;
    private bool isFigthingStarted = false;
    private Coroutine autoLevelUp;
    private float autoLevelUpTime = 5f;
    private WaitForSeconds dalayTime;

    [Space]
    [SerializeField] GameObject damageNote;
    private Color colorDamage = new Color(1f, 0.45f, 0.03f, 1);

    private void Start()
    {
        unitSprite = GetComponent<SpriteRenderer>();
        normalColor = unitSprite.color;

        ////ATTENTION! It happens when game starts and NEVER AGAIN!
        //currentMaxLevel   = playerStatsScript.GetStartParameter(PlayersStats.Level);
        ////currentHealth     = playerStatsScript.GetStartParameter(PlayersStats.Health);
        //searchRadius      = playerStatsScript.GetStartParameter(PlayersStats.SearchRadius);
        ////currentMana       = playerStatsScript.GetStartParameter(PlayersStats.Mana);
        //defence           = playerStatsScript.GetStartParameter(PlayersStats.Defence);
        //luck              = playerStatsScript.GetStartParameter(PlayersStats.Luck);

        battleUIManager = GlobalStorage.instance.battleIUManager;

        dalayTime = new WaitForSeconds(autoLevelUpTime);

        UpgradeTempExpGoal();
    }

    private void Update()
    {
        SearchBonuses();

        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            AddMana(BonusType.Mana, 5);
        }
    }

    private void UpgradeTempExpGoal()
    {
        float summExp;
        float totalSummExp = 0;

        for(int i = 1; i <= currentTempLevel + 1; i++)
        {
            summExp = (standartTempExpRate * levelMultiplierRate) * i + totalSummExp;
            totalSummExp += summExp;
        }

        currentTempExpGoal = totalSummExp;
    }

    private void SetStartParameters(PlayersStats stat, float value)
    {
        switch(stat)
        {
            case PlayersStats.Level:
                currentMaxLevel = value;
                break;

            case PlayersStats.Health:
                maxCurrentHealth = value;
                break;

            case PlayersStats.Mana:
                maxCurrentMana = value;
                break;

            case PlayersStats.SearchRadius:
                searchRadius = value;
                break;

            case PlayersStats.Defence:
                defence = value;
                break;

            case PlayersStats.Luck:
                luck = value;
                break;

            default:
                break;
        }
    }

    private void UpgradeStat(PlayersStats stat, float value)
    {
        SetStartParameters(stat, value);
    }

    private void SearchBonuses()
    {
        Collider2D[] findedObjects = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        foreach(var col in findedObjects)
        {
            BonusController bonus = col.GetComponent<BonusController>();

            if(bonus != null) bonus.ActivatateBonus();
        }
    }

    #region DAMAGE
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagManager.T_ENEMY) == true && isDead != true)
        {
            unitSprite.color = damageColor;
            Invoke("ColorBack", blinkTime);
        }
    }

    private void ColorBack()
    {
        unitSprite.color = normalColor;
    }

    public void TakeDamage(float physicalDamage, float magicDamage)
    {
        //TODO: we need to create some damage formula
        float damage = Mathf.Round(physicalDamage + magicDamage);
        currentHealth -= damage;
        if(currentHealth < 0) currentHealth = 0;

        ShowDamage(damage, colorDamage);
        EventManager.OnUpgradeStatCurrentValueEvent(PlayersStats.Health, maxCurrentHealth, currentHealth);

        if (currentHealth <= 0)
        {
            Dead();
        }
    }

    private void ShowDamage(float damageValue, Color colorDamage)
    {
        GameObject damageObject = GlobalStorage.instance.objectsPoolManager.GetObjectFromPool(ObjectPool.DamageText);
        damageObject.transform.position = transform.position;
        damageObject.SetActive(true);
        damageObject.GetComponent<DamageText>().Iniatilize(damageValue, colorDamage);
    }

    #endregion

    private void Dead()
    {
        isDead = true;
        Debug.Log("HERO IS DEAD");
        GameObject death = Instantiate(deathPrefab, transform.position, Quaternion.identity);
        death.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
        gameObject.SetActive(false);

        GlobalStorage.instance.battleIUManager.ShowDefeatBlock();
    }

    private void AddHealth(BonusType type, float value)
    {
        if(type == BonusType.Health && isDead == false)
        {
            // if we wont to add not value but percent
            if(value < 1) value = maxCurrentHealth * value;

            if(currentHealth + value > maxCurrentHealth)
                currentHealth = maxCurrentHealth;
            else
                currentHealth += value;

            EventManager.OnUpgradeStatCurrentValueEvent(PlayersStats.Health, maxCurrentHealth, currentHealth);
        }
    }

    public void AddMana(BonusType type, float value)
    {
        if(type == BonusType.Mana && isDead == false) ChangeMana(value);
    }

    public void SpendMana(float value)
    {
        if(isDead == false) ChangeMana(-value);
    }

    private void ChangeMana(float value)
    {
        if(value <= 0)
        {
            if(Mathf.Abs(value) <= currentMana)
            {
                currentMana -= Mathf.Abs(value);
            }
        }
        else
        {
            // if we wont to add not value but percent
            if(value < 1) value = maxCurrentMana * value;

            if(currentMana + value > maxCurrentMana)
                currentMana = maxCurrentMana;
            else
                currentMana += value;
        }

        EventManager.OnUpgradeStatCurrentValueEvent(PlayersStats.Mana, maxCurrentMana, currentMana);
    }

    private void UpgradeStatCurrentValue(PlayersStats stat, float maxValue, float currentValue)
    {
        if(stat == PlayersStats.Mana) currentMana = currentValue;
    }

    private void AddTempExp(BonusType type, float value)
    {
        if(type == BonusType.TempExp && isDead == false && isLevelUpComplete == false)
        {
            value *= (currentTempLevel == 0 ? 1 : currentTempLevel);

            while(value > 0)
            {
                value--;
                currentTempExp++;

                battleUIManager.UpgradeScale(currentTempExpGoal, currentTempExp);

                if(currentTempExp >= currentTempExpGoal)
                {
                    if(currentTempLevel < currentMaxLevel)
                    {
                        battleUIManager.TempLevelUp(currentTempLevel);
                        currentTempLevel++;

                        //EventManager.OnUpgradeTempLevelEvent(currentTempLevel);
                        if(currentTempLevel != currentMaxLevel)
                        {
                            currentTempExp = 0;
                            UpgradeTempExpGoal();
                        }
                        else
                        {
                            isLevelUpComplete = true;
                            EventManager.OnExpEnoughEvent(true);
                        }
                    }
                }
            }           
        }

        if(isFigthingStarted == false)
        {
            isFigthingStarted = true;
            autoLevelUp = StartCoroutine(AutoLevelUp());
        }
    }

    private IEnumerator AutoLevelUp()
    {
        while(isFigthingStarted == true)
        {
            yield return dalayTime;
            AddTempExp(BonusType.TempExp, (currentTempLevel == 0 ? 1 : currentTempLevel));
        }        
    }

    private void ResetTempLevel(bool mode)
    {
        currentTempLevel = 0;
        currentTempExp = 0;
        isLevelUpComplete = false;
        isFigthingStarted = false;
        if(autoLevelUp != null) StopCoroutine(autoLevelUp);

        if(mode == false)
        {
            currentMaxLevel = playerStatsScript.GetStartParameter(PlayersStats.Level);
            EventManager.OnExpEnoughEvent(false);
            UpgradeTempExpGoal();
        }
    }

    public void Resurrection()
    {
        gameObject.SetActive(true);
        isDead = false;
        AddHealth(BonusType.Health, maxCurrentHealth);
    }

    private void OnEnable()
    {
        EventManager.BonusPickedUp += AddHealth;
        EventManager.BonusPickedUp += AddMana;
        EventManager.BonusPickedUp += AddTempExp;
        EventManager.SetStartPlayerStat += SetStartParameters;
        EventManager.NewBoostedStat += UpgradeStat;
        EventManager.ChangePlayer += ResetTempLevel;
        EventManager.UpgradeStatCurrentValue += UpgradeStatCurrentValue;

        if(playerStatsScript == null) playerStatsScript = GlobalStorage.instance.playerStats;

        ResetTempLevel(false);

        playerStatsScript.GetAllStartParameters();
        currentHealth = playerStatsScript.GetCurrentParameter(PlayersStats.Health);
        currentMana = playerStatsScript.GetCurrentParameter(PlayersStats.Mana);
    }

    private void OnDisable()
    {
        EventManager.BonusPickedUp -= AddHealth;
        EventManager.BonusPickedUp -= AddMana;
        EventManager.BonusPickedUp -= AddTempExp;
        EventManager.SetStartPlayerStat -= SetStartParameters;
        EventManager.NewBoostedStat -= UpgradeStat;
        EventManager.ChangePlayer -= ResetTempLevel;
        EventManager.UpgradeStatCurrentValue -= UpgradeStatCurrentValue;
    }
}
