using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneUIItem : MonoBehaviour
{
    private RuneSO rune;
    [SerializeField] private Image lockImage;
    [SerializeField] private Image icon;
    [SerializeField] private Image bg;

    [SerializeField] private Color bronze;
    [SerializeField] private Color silver;
    [SerializeField] private Color gold;

    [SerializeField] private InfotipTrigger infotip;

    public void Init(RuneSO currentRune)
    {
        if(infotip == null) infotip = GetComponent<InfotipTrigger>();
        infotip.SetRune(currentRune);

        rune = currentRune;
        icon.sprite = rune.activeIcon;

        if(rune.level == 1) bg.color = bronze;
        if(rune.level == 2) bg.color = silver;
        if(rune.level == 3) bg.color = gold;
    }

}
