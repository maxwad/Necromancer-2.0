using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpCard : MonoBehaviour
{
    [SerializeField] private GameObject card;
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject cover;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image icon;
    [SerializeField] private Image border;

    private bool isVisible = true;

    public void Init(bool visibilityMode, MacroAbilitySO ability)
    {
        card.SetActive(true);
        if(cardRect == null) cardRect = GetComponent<RectTransform>();

        isVisible = visibilityMode;
        cover.SetActive(!isVisible);
        content.SetActive(isVisible);

        caption.text = ability.abilityName;
        icon.sprite = ability.activeIcon;
        description.text = ability.realDescription;
    }
}
