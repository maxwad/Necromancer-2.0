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

    private float standartTempExpRate = 0.025f;
    private float levelMultiplierRate = 1f;
    private float currentTempExpGoal;
    private float currentTempExp;
    private bool isLevelUpComplete = false;

    [Header("Characteristics")]
    [SerializeField] private float maxCurrentHealth;
    [SerializeField] private float currentHealth;

    private float searchRadiusBase;
    private float searchRadius;
    private float defenceBase;
    private float defence;

    [Header("Damage")]
    private bool isBattleEnded = false;
    [HideInInspector] public bool isDead = false;
    [Space]
    private SpriteRenderer unitSprite;
    private Color normalColor;
    private Color damageColor = Color.red;
    private float blinkTime = 0.1f;

    [SerializeField] private GameObject deathPrefab;

    [Space]
    [SerializeField] GameObject damageNote;
    private Color damageText = Color.white;
    private Color colorDamage = new Color(1f, 0.45f, 0.03f, 1);
    private Color criticalColor = Color.black;

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
    }

    private void Update()
    {
        SearchBonuses();
    }

    private void UpgradeTempExpGoal()
    {
        currentTempExp = 0;
        currentTempExpGoal = Mathf.Pow(((currentTempLevel + 1) / standartTempExpRate), levelMultiplierRate);

        //Debug.Log(currentTempExpGoal);
    }

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

    public void TakeDamage(float physicalDamage, float magicDamage, bool isCritical = false)
    {
        if(isBattleEnded == true) return;

        float phDamageComponent = physicalDamage - defence;
        if(phDamageComponent < 0) phDamageComponent = 0;

        float mDamageComponent = magicDamage - defence;
        if(mDamageComponent < 0) mDamageComponent = 0;

        float damage = phDamageComponent + mDamageComponent;

        damageText = colorDamage;

        if(isCritical == true)
        {
            damage *= 2;
            damageText = criticalColor;
        }

        resourcesManager.ChangeResource(ResourceType.Health, -damage);
        currentHealth = resourcesManager.GetResource(ResourceType.Health);

        if(currentHealth < 0) currentHealth = 0;

        ShowDamage(damage, damageText);

        if (currentHealth <= 0)
        {
            Dead();
        }
    }

    private void ShowDamage(float damageValue, Color color)
    {
        GameObject damageObject = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.DamageText);
        damageObject.transform.position = transform.position;
        damageObject.SetActive(true);
        damageObject.GetComponent<DamageText>().Iniatilize(damageValue, color);
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
            //value *= (currentTempLevel == 0 ? 1 : currentTempLevel);

            //Debug.Log(currentTempExpGoal + " " + value);
            while(value > 0)
            {
                value--;
                currentTempExp++;

                battleUIManager.expPart.UpgradeScale(currentTempExpGoal, currentTempExp);

                if(currentTempExp >= currentTempExpGoal)
                {
                    if(currentTempLevel < currentMaxLevel)
                    {
                        battleUIManager.expPart.TempLevelUp(currentTempLevel);
                        currentTempLevel++;

                        if(currentTempLevel <= currentMaxLevel) runesManager.TurnOnRune(currentTempLevel - 1);

                        if(currentTempLevel != currentMaxLevel)
                        {
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
            AddTempExp(BonusType.TempExp, 1);
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

    public bool IsHeroDead()
    {
        return isDead;
    }

    private void Victory()
    {
        isBattleEnded = true;
        if(autoLevelUp != null) StopCoroutine(autoLevelUp);
    }

    private void OnEnable()
    {
        EventManager.BonusPickedUp += AddTempExp;
        EventManager.SetBattleBoost += SetNewParameters;
        EventManager.SwitchPlayer += ResetTempLevel;
        EventManager.Victory += Victory;

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

        isBattleEnded = false;

        UpgradeTempExpGoal();
    }

    private void OnDisable()
    {
        EventManager.BonusPickedUp -= AddTempExp;
        EventManager.SetBattleBoost += SetNewParameters;
        EventManager.SwitchPlayer -= ResetTempLevel;
        EventManager.Victory -= Victory;
    }
}
