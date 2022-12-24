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

    public void ResetColumn(AltarMiniGame altarMiniGame, int ind)
    {
        game = altarMiniGame;
        index = ind;

        foreach(var gift in gifts)
            gift.gameObject.SetActive(false);

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
}
