using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    [HideInInspector] public MacroAbilitySO skill;
    private int index;

    [SerializeField] private Image icon;
    [SerializeField] private GameObject link;
    [SerializeField] private GameObject fakeLink;
    [SerializeField] private InfotipTrigger infotip;


    public void Init(MacroAbilitySO ability, int ind, float width)
    {
        skill = ability;
        index = ind;
        icon.sprite = skill.inActiveIcon;
        link.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        fakeLink.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        infotip.SetSkill(skill);

        if(index == 0)
        {
            link.SetActive(false);
            fakeLink.SetActive(false);
        }
        else
        {
            link.SetActive(false);
        }

    }

    public void MarkAsOpened()
    {
        icon.sprite = skill.activeIcon;

        if(index != 0)
        {
            link.SetActive(true);
            fakeLink.SetActive(false);
        }
    }
}
