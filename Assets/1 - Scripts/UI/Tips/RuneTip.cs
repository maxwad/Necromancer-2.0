using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;

public class RuneTip : MonoBehaviour
{
    private CanvasGroup canvas;

    public TMP_Text caption;
    public TMP_Text level;
    public TMP_Text description;
    public Image bg;

    [SerializeField] private Color bronze;
    [SerializeField] private Color silver;
    [SerializeField] private Color gold;
    [SerializeField] private Color negative;

    [SerializeField] private bool isPositive = true;

    public void Show(RuneSO rune)
    {
        if(canvas == null) canvas = GetComponent<CanvasGroup>();

        caption.text = rune.runeName;
        level.text = "Level " + rune.level;
        string tempDescr = (isPositive == true) ? rune.positiveDescription : rune.negativeDescription;

        description.text = tempDescr.Replace("$", rune.value.ToString());

        Color bgColor = Color.white;

        if(isPositive == true)
        {
            if(rune.level == 1) bgColor = bronze;
            if(rune.level == 2) bgColor = silver;
            if(rune.level == 3) bgColor = gold;
        }
        else
        {
            bgColor = negative;
        }

        bg.color = bgColor;
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void SetNegative()
    {
        isPositive = false;
    }
}
