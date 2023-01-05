using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static NameManager;

public class BonfireItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CampUI campUI;
    private CampGame campGame;

    [SerializeField] private Button button;
    [SerializeField] private GameObject coverIcon;
    [SerializeField] private GameObject lightning;
    [SerializeField] private GameObject realIcon;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TooltipTrigger tooltip;

    private int index = 0;
    private bool isActivated = false;

    public void Init(CampUI ui, CampGame game, int ind)
    {
        campUI = ui;
        campGame = game;
        index = ind;
    }

    public void ResetCell()
    {
        button.interactable = true;
        coverIcon.SetActive(true);
        realIcon.SetActive(false);
        lightning.SetActive(false);
    }

    public void ActivateCell()
    {
        Debug.Log("Activate Button");
        button.interactable = false;
        coverIcon.SetActive(false);
        realIcon.SetActive(true);
        campGame.GetResult(index);
        isActivated = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(campGame.CanIOpenCell() == true)
        {
            ActivateCell();
        }
        else
        {
            InfotipManager.ShowWarning("You have no more attempts to get a reward.");
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
}
