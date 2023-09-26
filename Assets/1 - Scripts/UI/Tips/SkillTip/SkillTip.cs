using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class SkillTip : MonoBehaviour
{
    private MacroLevelUpManager macroLevelUpManager;

    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text fakeDescription;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject luck;

    [Inject]
    public void Construct(MacroLevelUpManager macroLevelUpManager)
    {
        this.macroLevelUpManager = macroLevelUpManager;
    }

    public void Init(MacroAbilitySO skill)
    {
        caption.text = skill.abilityName;
        icon.sprite = skill.activeIcon;
        cost.text = skill.cost.ToString();
        description.text = skill.description.Replace("$V", skill.value.ToString());
        fakeDescription.text = skill.fakeDescription;
        luck.SetActive(skill.luckDepending);

        border.SetActive(!CheckStatus(skill));
    }

    private bool CheckStatus(MacroAbilitySO skill)
    {
        return macroLevelUpManager.abilitiesStorage.CheckSkill(skill);
    }    
}
