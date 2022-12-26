using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class AltarGiftColumnUI : MonoBehaviour
{
    private AltarMiniGame game;
    private int index = 0;
    [SerializeField] private List<AltarResourceButton> gifts;
    [SerializeField] private TMP_Text tip;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private GameObject selectBtn;
    [SerializeField] private GameObject veil;

    public void ResetColumn(AltarMiniGame altarMiniGame, int ind)
    {
        game = altarMiniGame;
        index = ind;
        selectBtn.SetActive(true);
        Fade(false);

        foreach(var gift in gifts)
        {
            gift.SetBGColor(Color.white);
            gift.gameObject.SetActive(false);
        }

        ResetChoiceBtn();
    }

    public void ResetChoiceBtn()
    {
        tip.gameObject.SetActive(true);
        resourceIcon.gameObject.SetActive(false);
    }

    //Button 
    public void ShowSelectBlock()
    {
        game.ShowChoiceBlock(index);
    }

    public void SetChoiceIcon(Sprite icon)
    {
        tip.gameObject.SetActive(false);
        resourceIcon.gameObject.SetActive(true);
        resourceIcon.sprite = icon;
    }

    internal void ShowTry(int currentTry, Sprite resourceIcon)
    {
        gifts[currentTry].gameObject.SetActive(true);
        gifts[currentTry].SetResource(resourceIcon);
    }

    internal void SetChoiceBtnColor(int currentTry, Color tryColor)
    {
        gifts[currentTry].SetBGColor(tryColor);
    }

    internal void SelectButtonDisable()
    {
        selectBtn.SetActive(false);
    }

    internal void Fade(bool fadingMode)
    {
        veil.SetActive(fadingMode);
    }
}
