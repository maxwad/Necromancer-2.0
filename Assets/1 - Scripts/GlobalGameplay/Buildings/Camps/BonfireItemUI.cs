using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;

public class BonfireItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CampGame campGame;

    [SerializeField] private Button button;
    [SerializeField] private GameObject coverIcon;
    [SerializeField] private GameObject lightning;
    [SerializeField] private GameObject realIcon;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private Image coverIconImage;
    [SerializeField] private TooltipTrigger tooltip;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color transparentColor;

    private int index = 0;
    private bool isActivated = false;

    public void Init(CampGame game, int ind)
    {
        campGame = game;
        index = ind;
    }

    public void ResetCell()
    {
        button.interactable = true;
        coverIcon.SetActive(true);
        realIcon.SetActive(false);
        lightning.SetActive(false);

        coverIconImage.color = normalColor;
        isActivated = false;
    }

    public void ActivateCell(bool tipMode)
    {
        isActivated = true;

        button.interactable = false;
        coverIcon.SetActive(false);
        realIcon.SetActive(true);
        campGame.ShowResult(index, tipMode);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isActivated == true) return;

        if(campGame.CanIOpenCell() == true)
        {
            ActivateCell(false);
        }
        else
        {
            InfotipManager.ShowWarning("You have no more attempts to get a reward.");
            return;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        lightning.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isActivated == false) lightning.SetActive(true);
    }

    public void SetReward(CampBonus currentBonus)
    {
        rewardIcon.sprite = currentBonus.icon;
        tooltip.content = currentBonus.name;
    }

    public bool GetCellStatus()
    {
        return isActivated;
    }

    public void ShowCell(CampBonus bonus)
    {
        coverIconImage.color = transparentColor;
        realIcon.SetActive(true);

        SetReward(bonus);
    }
}
