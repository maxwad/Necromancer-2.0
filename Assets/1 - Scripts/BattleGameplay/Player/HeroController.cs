using System.Collections;
using UnityEngine;
using TMPro;
using static NameManager;

public class HeroController : MonoBehaviour
{
    private PlayerStats playerStats;
    private MacroLevelUpManager levelManager;
    private ResourcesManager resourcesManager;
    private RunesManager runesManager;

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

    private float searchRadiusBase;
    private float searchRadius;
    private float defenceBase;
    private float defence;

    [Header("Damage")]
    [HideInInspector] public bool isDead = false;
    [Space]
    private SpriteRenderer unitSprite;
    private Color normalColor;
    private Color damageColor = Color.red;
    private float blinkTime = 0.1f;

    [SerializeField] private GameObject deathPrefab;

    [Space]
    [SerializeField] GameObject damageNote;
    private Color colorDamage = new Color(1f, 0.45f, 0.03f, 1);

    [Header("UI")]
    [Space]
    private BattleUIManager battleUIManager;
    private bool isFigthingStarted = false;
    private Coroutine autoLevelUp;
    private float autoLevelUpTime = 5f;
    private WaitForSeconds dalayTime;

    private void Start()
    {
        unitSprite = GetComponent<SpriteRenderer>();
        normalColor = unitSprite.color;

        battleUIManager = GlobalStorage.instance.battleIUManager;

        dalayTime = new WaitForSeconds(autoLevelUpTime);

        UpgradeTempExpGoal();
    }

    private void Update()
    {
        SearchBonuses();
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

    //private void SetNewParameters(PlayersStats stat, float value)
    //{
    //    if(stat == PlayersStats.SearchRadius) searchRadius = value;
    //    if(stat == PlayersStats.Defence) defence = value;
    //}

    private void SetNewParameters(BoostType boost, float value)
    {
        if(boost == BoostType.BonusRadius) searchRadius = searchRadiusBase + searchRadiusBase * value;
        if(boost == BoostType.HeroDefence) defence = defenceBase + defenceBase * value;
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

        resourcesManager.ChangeResource(ResourceType.Health, -damage);
        currentHealth = resourcesManager.GetResource(ResourceType.Health);

        if(currentHealth < 0) currentHealth = 0;

        ShowDamage(damage, colorDamage);

        if (currentHealth <= 0)
        {
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

                        if(currentTempLevel != currentMaxLevel)
                        {
                            currentTempExp = 0;
                            UpgradeTempExpGoal();
                            runesManager.TurnOnRune(currentTempLevel - 1);
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
            currentMaxLevel = levelManager.GetCurrentLevel();
            EventManager.OnExpEnoughEvent(false);
            UpgradeTempExpGoal();
        }
    }

    public void Resurrection()
    {
        gameObject.SetActive(true);
        isDead = false;

        if(resourcesManager == null) resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesManager.ChangeResource(ResourceType.Health, resourcesManager.maxHealth);
    }

    private void OnEnable()
    {
        EventManager.BonusPickedUp += AddTempExp;
        //EventManager.SetNewPlayerStat += SetNewParameters;
        EventManager.SetBattleBoost += SetNewParameters;
        EventManager.SwitchPlayer += ResetTempLevel;

        if(playerStats == null)
        {
            playerStats = GlobalStorage.instance.playerStats;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            levelManager = GlobalStorage.instance.macroLevelUpManager;
            runesManager = GlobalStorage.instance.runesManager;
        }        

        ResetTempLevel(false);

        searchRadiusBase = playerStats.GetCurrentParameter(PlayersStats.SearchRadius);
        searchRadius = searchRadiusBase;
        defenceBase = playerStats.GetCurrentParameter(PlayersStats.Defence);
        defence = defenceBase;

        currentHealth = resourcesManager.GetResource(ResourceType.Health);
    }

    private void OnDisable()
    {
        EventManager.BonusPickedUp -= AddTempExp;
        //EventManager.SetNewPlayerStat -= SetNewParameters;
        EventManager.SetBattleBoost += SetNewParameters;
        EventManager.SwitchPlayer -= ResetTempLevel;
    }
}
