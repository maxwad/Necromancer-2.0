using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private MacroLevelUpManager macroManager;
    [HideInInspector] public MacroAbilitySO skill;
    private int index;

    [SerializeField] private Image icon;
    [SerializeField] private GameObject link;
    [SerializeField] private GameObject fakeLink;
    [SerializeField] private InfotipTrigger infotip;
    [SerializeField] private Image border;
    [SerializeField] private Image bgSkill;

    [SerializeField] private Color readyColor;
    [SerializeField] private Color normalColor;

    private bool isTaken = false;
    private bool amINext = false;
    private bool isMousePressed = false;

    public void Init(MacroAbilitySO ability, int ind, float linkWidth)
    {
        macroManager = GlobalStorage.instance.macroLevelUpManager;

        skill = ability;
        index = ind;
        icon.sprite = skill.inActiveIcon;
        link.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, linkWidth);
        fakeLink.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, linkWidth);

        infotip.SetSkill(skill);

        if(index == 0)
        {
            link.SetActive(false);
            fakeLink.SetActive(false);
            IAmNextSkill(true);
        }
        else
        {
            link.SetActive(false);
            IAmNextSkill(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(GlobalStorage.instance.IsGlobalMode() == false)
        {
            InfotipManager.ShowWarning("You cannot make any changes during the battle.");
            return;
        }

        if(isTaken == true) return;

        if(amINext == false)
        {
            InfotipManager.ShowWarning("Open previous skill first.");
            return;
        }

        if(macroManager.GetAbilityPoints() >= skill.cost)
        {
            isMousePressed = true;
        }
        else
        {
            InfotipManager.ShowWarning("It's too expensive for you!");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isMousePressed = false;
        if(isTaken == false) border.fillAmount = 1f;
    }

    private void Update()
    {
        if(isTaken == false)
        {
            if(isMousePressed == true)
            {
                border.fillAmount -= Time.unscaledDeltaTime * 2;
                if(border.fillAmount <= 0f)
                {
                    MarkAsOpened();
                }
            }
        }
    }

    public void MarkAsOpened(bool loadMode = false)
    {
        icon.sprite = skill.activeIcon;

        if(index != 0)
        {
            link.SetActive(true);
            fakeLink.SetActive(false);
        }

        isTaken = true;
        amINext = false;

        macroManager.OpenAbility(skill, loadMode);
        
        if(loadMode == true)
            border.fillAmount = 0f;
    }

    public void IAmNextSkill(bool mode)
    {

        bgSkill.color = (mode == true) ? readyColor : normalColor;
        amINext = mode;
    }
}
