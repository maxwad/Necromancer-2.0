using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class AltarUI : MonoBehaviour
{
    private PlayerStats playerStats;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    private Altar currentAltar; 

    [Header("UI")]
    [SerializeField] private GameObject uiPanel;

    [SerializeField] private Color rigthColor;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
    }

    internal void Open(bool modeClick, Altar altar)
    {
        //if(playerStats.GetCurrentParameter( PlayersStats.MedicAltar) < 1)
        //{
        //    InfotipManager.ShowWarning("To use Altars, you need to unlock the corresponding skill!");
        //    return;
        //}

        //if(modeClick == true)
        //{
        //    InfotipManager.ShowWarning("Gifts for healing are accepted only in person.");
        //    return;
        //}

        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);

        currentAltar = altar;
        Init();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);

        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);

        uiPanel.SetActive(false);
    }

    private void Init()
    {
        Dictionary<ResourceType, float> currentCost = currentAltar.GetPrice();

    }

   
}
