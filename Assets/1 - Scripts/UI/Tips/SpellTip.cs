using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;

public class SpellTip : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;

    [Header("Info Details")]
    [SerializeField] private TMP_Text spellTitle;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text manaCost;
    [SerializeField] private TMP_Text duration;
    [SerializeField] private TMP_Text reloading;
    [SerializeField] private TMP_Text description;

    public void Show(SpellSO spell)
    {
        spellTitle.text = spell.spellName;
        level.text = "Level " + spell.level;

        manaCost.text = spell.manaCost.ToString();
        duration.text = (spell.actionTime != 0) ? spell.actionTime.ToString() : "-";
        reloading.text = spell.reloading.ToString();

        description.text = spell.description
            .Replace("$V", spell.value.ToString())
            .Replace("$R", spell.radius.ToString())
            .Replace("$T", spell.actionTime.ToString());

        Fading.instance.FadeWhilePause(true, canvas);
    }
}
