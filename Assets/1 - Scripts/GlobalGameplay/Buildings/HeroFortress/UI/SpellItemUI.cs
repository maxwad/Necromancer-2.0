using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using static NameManager;

public class SpellItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject lightning;
    [SerializeField] private Image spellIcon;
    [SerializeField] private TMP_Text spellLevel;
    [SerializeField] private InfotipTrigger tip;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color inactiveColor;


    private SpellWorkroom workroom;
    private SpellSO spell;

    public void Init(SpellWorkroom room, SpellSO newSpell)
    {
        if(workroom == null) workroom = room;

        gameObject.SetActive(true);

        spell = newSpell;
        spellIcon.sprite = spell.icon;
        int level = workroom.GetSpellLevel(spell.spell);
        int maxLevel = workroom.GetSpellMaxLevel(spell.spell);
        spellIcon.color = (level > 0) ? normalColor : inactiveColor;
        spellLevel.text = level + "/" + maxLevel;

        tip.SetSpell(spell);
    }

    public void ResetItem()
    {
        spell = null;
        EnableLightning(false);
        gameObject.SetActive(false);
    }

    public void EnableLightning(bool enableMode)
    {
        lightning.SetActive(enableMode);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        workroom.SetCurrentSpell(spell);
        EnableLightning(true);
    }

    public bool CheckSpell(Spells checkSpell)
    {
        return spell.spell == checkSpell;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

}
