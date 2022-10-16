using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTip : MonoBehaviour
{
    private MacroLevelUpManager macroLevelUpManager;
    private CanvasGroup canvas;

    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text fakeDescription;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject luck;

    public void Init(MacroAbilitySO skill)
    {
        if(macroLevelUpManager == null) 
        { 
            macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;
            canvas = GetComponent<CanvasGroup>();
        }

        caption.text = skill.abilityName;
        icon.sprite = skill.activeIcon;
        cost.text = skill.cost.ToString();
        description.text = skill.realDescription;
        fakeDescription.text = skill.fakeDescription;
        luck.SetActive(skill.luckDepending);

        border.SetActive(!CheckStatus(skill));
        //Fading.instance.FadeWhilePause(true, canvas);
    }

    private bool CheckStatus(MacroAbilitySO skill)
    {
        return macroLevelUpManager.abilitiesStorage.CheckSkill(skill);
    }    
}
