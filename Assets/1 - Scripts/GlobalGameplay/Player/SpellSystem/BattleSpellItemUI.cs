using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using static NameManager;

public class BattleSpellItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isSpellInStorage = true;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject locked;
    [SerializeField] private TMP_Text indexLabel;
    [SerializeField] private InfotipTrigger spellTip;
    [SerializeField] private TooltipTrigger emptyTip;
    [SerializeField] private TooltipTrigger lockTip;

    private SpellWindow spellWindow;
    private SpellSO currentSpell;
    private bool isLocked = false;

    public void Init(SpellWindow window, SpellSO spell)
    {
        if(spellWindow == null) spellWindow = window;

        if(isSpellInStorage == true)
        {
            locked.SetActive(false);
            indexLabel.gameObject.SetActive(false);
        }

        currentSpell = spell;

        icon.gameObject.SetActive(true);
        icon.sprite = currentSpell.icon;
        spellTip.enabled = true;
        spellTip.SetSpell(currentSpell);
    }

    public void SetIndexes(int index, bool unlockMode)
    {
        indexLabel.gameObject.SetActive(true);
        indexLabel.text = index.ToString();

        locked.SetActive(!unlockMode);
        lockTip.enabled = !unlockMode;
        emptyTip.enabled = unlockMode;

        icon.gameObject.SetActive(false);
        currentSpell = null;
        spellTip.enabled = false;
        isLocked = !unlockMode;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("LOCK " + isLocked);
        if(isLocked == true)
        {
            Debug.Log("LOCK");
            return;
        }

        if(currentSpell == null)
        {
            Debug.Log("NULL");
            return;
        }

        spellWindow.SwitchSpells(currentSpell.spell, isSpellInStorage);
    }
}
