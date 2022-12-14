using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class GMInterfaceHero : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private PlayerPersonalWindow personalWindow;
    public Dictionary<ResourceType, float> resourcesDict = new Dictionary<ResourceType, float>();

    [Header("Hero")]
    [SerializeField] private GameObject heroContainer;
    [SerializeField] private TMP_Text manaAmount;
    [SerializeField] private Image manaScale;
    private float manaMax;
    [SerializeField] private TMP_Text healthAmount;
    [SerializeField] private Image healthScale;
    private float healthMax;
    [SerializeField] private Image levelScale;
    [SerializeField] private TMP_Text levelCount;
    [SerializeField] private TooltipTrigger levelTooltip;
    [SerializeField] private Button heroButton;
    [SerializeField] private Animator skillsButton;

    [Header("ShortCuts")]
    public KeyCode armyKey;
    public KeyCode skillsKey;
    public KeyCode microKey;
    public KeyCode spellsKey;

    private void Start()
    {
        personalWindow = GlobalStorage.instance.playerMilitaryWindow;

        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesDict = resourcesManager.GetAllResources();

        healthMax = resourcesManager.GetMaxHealth();
        manaMax = resourcesManager.GetMaxMana();

        UpgradeManaHealthUI(ResourceType.Mana, manaMax);
        UpgradeManaHealthUI(ResourceType.Health, healthMax);
    }

    private void FillResource(ResourceType type, float value)
    {
        if(type == ResourceType.Health || type == ResourceType.Mana) UpgradeManaHealthUI(type, value);
    }

    private void UpgradeMaxManaHealth(PlayersStats stat, float maxValue)
    {
        if(stat != PlayersStats.Mana || stat == PlayersStats.Health) return;

        ResourceType type = ResourceType.Mana;

        if(stat == PlayersStats.Mana)
        {
            manaMax = maxValue;
        }

        if(stat == PlayersStats.Health)
        {
            healthMax = maxValue;
            type = ResourceType.Health;
        }

        UpgradeManaHealthUI(type, resourcesDict[type]);
    }

    private void UpgradeManaHealthUI(ResourceType stat, float value)
    {
        float maxAmount;
        TMP_Text currentText;
        Image scale;

        float currentAmount = value;

        if(stat == ResourceType.Mana)
        {
            maxAmount = manaMax;
            currentText = manaAmount;
            scale = manaScale;
        }
        else
        {
            maxAmount = healthMax;
            currentText = healthAmount;
            scale = healthScale;

            if(currentAmount > 0 && currentAmount < 1) currentAmount = 1f;
        }

        scale.fillAmount = currentAmount / maxAmount;
        currentText.text = Mathf.Round(currentAmount) + "/" + maxAmount;
    }

    public void UpgradeLevel(LevelData data)
    {
        levelScale.fillAmount = data.currentExp / data.boundExp;
        levelCount.text = data.level.ToString();
        levelTooltip.content = data.currentExp + "/" + data.boundExp;
    }

    public void UpgradeAbilityPoints(int amount)
    {
        bool mode = (amount == 0) ? false : true;
        skillsButton.GetComponent<Animator>().SetBool(TagManager.A_BLINK, mode);
    }

    public void ShowBlock(bool mode)
    {
        heroContainer.SetActive(mode);
    }

    public void OpenArmyWindow()
    {
        personalWindow.PressButton(armyKey);
    }

    public void OpenSkillsWindow()
    {
        personalWindow.PressButton(skillsKey);
    }

    public void OpenMicroWindow()
    {
        personalWindow.PressButton(microKey);
    }

    public void OpenSpellsWindow()
    {
        personalWindow.PressButton(spellsKey);
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeMaxManaHealth;
        EventManager.UpgradeResource += FillResource;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeMaxManaHealth;
        EventManager.UpgradeResource -= FillResource;
    }
}
