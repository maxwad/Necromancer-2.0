using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class SpellDetailUI : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [Header("Details")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text spellName;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text manaCost;
    [SerializeField] private TMP_Text duration;
    [SerializeField] private TMP_Text reloading;
    [SerializeField] private TMP_Text description;
    [SerializeField] private List<Image> costIconsList;
    [SerializeField] private List<TMP_Text> costAmountList;
    
    private Color normalColor = Color.white;
    [SerializeField] private Color warningColor;
    [SerializeField] private Color upgradeColor;

    [Header("Extra Elements")]
    [SerializeField] private GameObject upgradeLabel;
    [SerializeField] private GameObject confirmBlock;
    [SerializeField] private GameObject createBlock;
    [SerializeField] private GameObject levelUpWarning;
    [SerializeField] private GameObject levelUpButton;


    private SpellWorkroom workroom;
    private SpellSO currentSpell;
    private bool isCreating = false;
    private bool isResourceDeficit = false;

    public void FillData(SpellWorkroom room, SpellSO spell, SpellSO previousSpell, bool isFirstSpell, bool createMode)
    {
        if(workroom == null)
        {
            workroom = room;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }

        isResourceDeficit = false;
        isCreating = createMode;

        currentSpell = spell;

        spellName.text = spell.spellName;
        icon.sprite = spell.icon;
        level.text = "Level " + spell.level;
        level.color = (isFirstSpell == false) ? upgradeColor : normalColor;

        manaCost.text = spell.manaCost.ToString();
        duration.text = (spell.actionTime != 0) ? spell.actionTime.ToString() : "-";
        reloading.text = spell.reloading.ToString();

        string value = spell.value.ToString();
        string radius = spell.radius.ToString();
        string actionTime = spell.actionTime.ToString();

        if(isFirstSpell == false)
        {
            manaCost.color = (previousSpell.manaCost != spell.manaCost) ? upgradeColor : normalColor;
            duration.color = (previousSpell.actionTime != spell.actionTime) ? upgradeColor : normalColor;
            reloading.color = (previousSpell.reloading != spell.reloading) ? upgradeColor : normalColor;

            if(spell.value != previousSpell.value)
                value = "<color=#FF7E00><b>" + spell.value + "</b></color>";
            if(spell.radius != previousSpell.radius)
                radius = "<color=#FF7E00><b>" + spell.radius + "</b></color>";
            if(spell.actionTime != previousSpell.actionTime)
                actionTime = "<color=#FF7E00><b>" + spell.actionTime + "</b></color>";
        }
        
        description.text = spell.description
            .Replace("$V", value)
            .Replace("$R", radius)
            .Replace("$T", actionTime);

        FillCost(isFirstSpell);

        Refactoring(isFirstSpell, createMode);
    }

    private void FillCost(bool isFirstSpell)
    {
        for(int i = 0; i < currentSpell.cost.Count; i++)
        {
            costIconsList[i].transform.parent.gameObject.SetActive(false);
        }

        for(int i = 0; i < currentSpell.cost.Count; i++)
        {
            costIconsList[i].transform.parent.gameObject.SetActive(true);
            costIconsList[i].sprite = resourcesIcons[currentSpell.cost[i].type];
            costAmountList[i].text = currentSpell.cost[i].amount.ToString();
            bool isEnough = resourcesManager.CheckMinResource(currentSpell.cost[i].type, currentSpell.cost[i].amount);
            costAmountList[i].color = (isEnough == true) ? normalColor : warningColor;

            if(isEnough == false)
            {
                isResourceDeficit = true;
            }
        }

        if(isFirstSpell == false)
        {
            int level = workroom.GetWorkroomLevel();

            levelUpWarning.SetActive(currentSpell.level > level);
            levelUpButton.SetActive(currentSpell.level <= level);
        }
    }

    private void Refactoring(bool isFirstSpell, bool createMode)
    {
        spellName.gameObject.SetActive(false);
        upgradeLabel.SetActive(false);
        confirmBlock.SetActive(false);
        createBlock.SetActive(false);

        if(isFirstSpell == true)
        {
            spellName.gameObject.SetActive(true);
            if(createMode == true)
            {                
                createBlock.SetActive(true);
            }
        }
        else
        {
            upgradeLabel.SetActive(true);
            createBlock.SetActive(true);
        }
    }

    //Button
    public void TryToAction()
    {
        if(isResourceDeficit == true)
        {
            InfotipManager.ShowWarning("You do not have enough Resources for this action.");
            return;
        }

        confirmBlock.SetActive(true);
    }

    //Button
    public void Action()
    {
        Pay();
        workroom.UpgradeSpell(currentSpell);
        Close();
    }

    //Button
    public void Close()
    {
        confirmBlock.SetActive(false);
    }

    private void Pay()
    {
        for(int i = 0; i < currentSpell.cost.Count; i++)
        {
            resourcesManager.ChangeResource(currentSpell.cost[i].type, -currentSpell.cost[i].amount);
        }
    }
}
