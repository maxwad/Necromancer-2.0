using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;
using Zenject;

public class BattleUIHeroPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;
    private ResourcesManager resourcesManager;

    [Header("Mana")]
    [SerializeField] private TMP_Text manaInfo;
    [SerializeField] private Image manaScale;
    private float currentMaxManaCount;
    private float currentManaCount;
    [SerializeField] private Color manaUpColor;
    [SerializeField] private Color manaDownColor;
    [SerializeField] private Color normalManaColor;

    [Header("Health")]
    [SerializeField] private TMP_Text healthInfo;
    [SerializeField] private Image healthScale;
    private float currentMaxHealthCount;
    private float currentHealthCount;
    [SerializeField] private Color healthUpColor;
    [SerializeField] private Color healthDownColor;
    [SerializeField] private Color normalHealthColor;

    [Header("Gold")]
    [SerializeField] private TMP_Text goldInfo;
    private float startGold;

    [Inject]
    public void Construct(ResourcesManager resourcesManager)
    {
        this.resourcesManager = resourcesManager;
    }

    public void Init(BattleUIManager manager)
    {
        battleUIManager = manager;
    }

    public void UpgradeResourceUI(ResourceType type, float value)
    {
        GetMaxManaHealth();

        if(type == ResourceType.Health) FillHealth(value);
        if(type == ResourceType.Mana) FillMana(value);
        if(type == ResourceType.Gold) FillGold(value);
    }

    public void GetMaxManaHealth()
    {
        currentMaxManaCount = resourcesManager.GetMaxMana();
        currentMaxHealthCount = resourcesManager.GetMaxHealth();
    }

    #region Mana

    public void FillMana(float value = 0)
    {
        Color blinkColor = manaUpColor;
        float current = (value == 0) ? resourcesManager.GetResource(ResourceType.Mana) : value;

        if(currentManaCount > current) blinkColor = manaDownColor;
        currentManaCount = current;

        float widthMana = currentManaCount / currentMaxManaCount;

        manaScale.fillAmount = widthMana;
        manaInfo.text = Mathf.Round(currentManaCount) + "/" + currentMaxManaCount;

        battleUIManager.Blink(manaScale, blinkColor, normalManaColor, 10);
    }


    #endregion

    #region Health

    public void FillHealth(float value = 0)
    {
        Color blinkColor = healthUpColor;
        float current = (value == 0) ? resourcesManager.GetResource(ResourceType.Health) : value;
        if(currentHealthCount > current) blinkColor = healthDownColor;
        currentHealthCount = current;

        float widthHealth = currentHealthCount / currentMaxHealthCount;

        healthScale.fillAmount = widthHealth;

        if(currentHealthCount > 0 && currentHealthCount < 1) currentHealthCount = 1f;

        healthInfo.text = Mathf.Round(currentHealthCount) + "/" + currentMaxHealthCount;

        battleUIManager.Blink(healthScale, blinkColor, normalHealthColor, 10);
    }

    #endregion

    #region Gold
    public void FillGold(float value = 0)
    {
        float current = 0;

        if(value == 0)
            startGold = resourcesManager.GetResource(ResourceType.Gold);
        else
            current = value - startGold;

        goldInfo.text = Mathf.Round(current).ToString();
    }

    #endregion

    private void OnEnable()
    {
        EventManager.UpgradeResource += UpgradeResourceUI;        
    }

    private void OnDisable()
    {
        EventManager.UpgradeResource -= UpgradeResourceUI;        
    }
}
