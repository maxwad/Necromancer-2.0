using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class TombUI : MonoBehaviour
{
    private CanvasGroup canvas;
    private TombsManager tombsManager;
    private ResourcesManager resourcesManager;

    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [SerializeField] private GameObject uiPanel;
    private GameObject tomb;
    private ObjectOwner owner;
    private SpellSO spell;

    [Header("Blocks")]
    [SerializeField] private GameObject defaultBlock;
    [SerializeField] private GameObject emptyBlock;
    [SerializeField] private GameObject infoBlock;

    [Header("Info Details")]
    [SerializeField] private TMP_Text spellTitle;
    [SerializeField] private Image spellIcon;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private TMP_Text duration;
    [SerializeField] private TMP_Text reloading;
    [SerializeField] private TMP_Text description;

    [Header("Extra Reward")]
    [SerializeField] private  List <GameObject> rewardItems;
    private List<Image> rewardIcons = new List<Image>();
    private List<TMP_Text> rewardCounts = new List<TMP_Text>();



    private void Start()
    {       
        tombsManager = GlobalStorage.instance.tombsManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesIcons = resourcesManager.GetAllResourcesIcons();

        canvas = uiPanel.GetComponent<CanvasGroup>();

        foreach(var item in rewardItems)
        {
            rewardIcons.Add(item.GetComponentInChildren<Image>());
            rewardCounts.Add(item.GetComponentInChildren<TMP_Text>());
        }
    }

    public void Open(bool clickMode, GameObject tomb)
    {
        MenuManager.instance.MiniPause(true);
        uiPanel.SetActive(true);

        this.tomb = tomb;
        Init(clickMode);

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        MenuManager.instance.MiniPause(false);
        uiPanel.SetActive(false);
    }

    public void Init(bool clickMode)
    {
        defaultBlock.SetActive(false);
        emptyBlock.SetActive(false);
        infoBlock.SetActive(false);

        owner = tomb.GetComponent<ObjectOwner>();
        spell = tombsManager.GetSpell(tomb);
        bool defaultMode = false;

        if(owner.GetVisitStatus() == false)
        {
            if(clickMode == true)
            {
                defaultMode = true;
            }
            else
            {
                tombsManager.UnlockSpell(spell);
                owner.SetVisitStatus(true);
            }
        }       

        ShowSpell(defaultMode);
    }

    private void ShowSpell(bool defaultMode)
    {
        if(defaultMode == true)
        {
            defaultBlock.SetActive(true);
        }
        else
        {
            if(spell == null)
            {
                emptyBlock.SetActive(true);
            }
            else
            {
                ShowDetails();
            }
        }
    }

    private void ShowDetails()
    {
        infoBlock.SetActive(true);

        spellTitle.text = spell.spellName;
        spellIcon.sprite = spell.icon;

        cost.text = spell.manaCost.ToString();
        duration.text = (spell.actionTime != 0) ? spell.actionTime.ToString() : "-";
        reloading.text = spell.reloading.ToString();

        description.text = spell.description
            .Replace("$V", spell.value.ToString())
            .Replace("$R", spell.radius.ToString())
            .Replace("$T", spell.actionTime.ToString());

        Reward reward = tombsManager.GetReward(tomb);

        if(reward == null) return;

        foreach(var item in rewardItems)
            item.SetActive(false);

        for(int i = 0; i < reward.resourcesList.Count; i++)
        {
            rewardItems[i].SetActive(true);
            rewardIcons[i].sprite = resourcesIcons[reward.resourcesList[i]];
            rewardCounts[i].text = reward.resourcesQuantity[i].ToString();
        }

    }
}
