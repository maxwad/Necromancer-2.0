using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class BoostInBattleUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;

    private RunesManager runesManager;

    [SerializeField] private Image bg;
    [SerializeField] private Image icon;

    [SerializeField] private RectTransform iconRect;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private TooltipTrigger tip;

    [SerializeField] private Color negativeColor;
    [SerializeField] private Color positiveColor;

    [SerializeField] private Color runeColor;
    [SerializeField] private Color spellColor;
    [SerializeField] private Color enemyColor;

    [SerializeField] private float normalSize = 34f;
    [SerializeField] private float bigSize = 64f;

    [SerializeField] private float normalFont = 16f;
    [SerializeField] private float bigFont = 24f;

    [SerializeField] private float lifeTime = 3f;

    private Coroutine fadeCoroutine;

    public void Init(RunesType runeType, float value, bool constEffect = true, EffectType effectType = EffectType.Rune)
    {
        if(runesManager == null) runesManager = GlobalStorage.instance.runesManager;

        Sprite pict = runesManager.runesStorage.GetRuneIcon(runeType);
        string descr = runesManager.runesStorage.GetRuneDescription(runeType);
        bool isInvertedRune = runesManager.runesStorage.GetRuneInvertion(runeType);

        icon.sprite = pict;

        string before = "";
        if (value > 0) before = "+";
        string after = "%";
        amount.text = before + value + after;

        Color color;

        if(isInvertedRune == false)
            color = (value > 0) ? positiveColor : negativeColor;
        else
            color = (value > 0) ? negativeColor : positiveColor;

        if(value == 0) color = Color.white;

        amount.color = color;

        tip.content = descr.Replace("$", value.ToString());

        Refactoring(constEffect, effectType);
    }

    private void Refactoring(bool constEffect, EffectType effectType)
    {
        amount.fontSize = (constEffect == true) ? normalFont : bigFont;
        float width = (constEffect == true) ? normalSize : bigSize;
        float heigth = (constEffect == true) ? normalSize : bigSize;

        iconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        iconRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heigth);

        Color bgColor = Color.white;
        if(effectType == EffectType.Rune) bgColor = runeColor;
        if(effectType == EffectType.Spell) bgColor = spellColor;
        if(effectType == EffectType.Enemy) bgColor = enemyColor;
        bg.color = bgColor;

        if(constEffect == false)
            fadeCoroutine = StartCoroutine(FadeOut());
        else
            canvas.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        Fading.instance.Fade(true, canvas);

        yield return new WaitForSeconds(lifeTime);

        Fading.instance.Fade(false, canvas, 0.3f, activeMode: false);
    }

    private void OnDisable()
    {
        if(fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        gameObject.SetActive(false);
    }
}
