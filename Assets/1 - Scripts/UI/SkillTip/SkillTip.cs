using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkillTip : MonoBehaviour
{
    private MacroLevelUpManager macroLevelUpManager;

    [SerializeField] private RectTransform cardRect;
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text fakeDescription;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject luck;

    private float offset = 20f;

    public void Init(MacroAbilitySO skill)
    {
        if(macroLevelUpManager == null) macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;

        caption.text = skill.abilityName;
        icon.sprite = skill.activeIcon;
        description.text = skill.realDescription;
        fakeDescription.text = skill.fakeDescription;
        luck.SetActive(skill.luckDepending);

        border.SetActive(!CheckStatus(skill));
    }

    private bool CheckStatus(MacroAbilitySO skill)
    {
        return macroLevelUpManager.abilitiesStorage.CheckSkill(skill);
    }

    private void LateUpdate()
    {
        CorrectSquadtipPosition();
    }

    private void CorrectSquadtipPosition()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = 0;
        float pivotY = 1;
        float offsetX = offset;
        float offsetY = 0;

        if(Screen.width - position.x < cardRect.rect.width)
        {
            pivotX = 1f;
            offsetX = -offsetX;
        }

        if(position.y - cardRect.rect.height < 0)
        {
            pivotY = 0f;
        }

        cardRect.pivot = new Vector2(pivotX, pivotY);
        transform.position = position + new Vector2(offsetX, offsetY);
    }
}
