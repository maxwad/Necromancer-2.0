using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTip : MonoBehaviour
{
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text fakeDescription;
    [SerializeField] private Image icon;
    [SerializeField] private Image border;
    [SerializeField] private GameObject luck;

    private float offset = 20f;

    public void Init(MacroAbilitySO skill)
    {
        caption.text = skill.abilityName;
        icon.sprite = skill.activeIcon;
        description.text = skill.realDescription;
        fakeDescription.text = skill.fakeDescription;
        luck.SetActive(skill.luckDepending);
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
