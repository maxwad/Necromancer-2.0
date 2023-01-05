using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class CampUI : MonoBehaviour
{
    private CanvasGroup canvas;
    private CampManager campManager;
    private CampGame campGame;
    //private ResourcesManager resourcesManager;

    //private Dictionary<ResourceType, Sprite> resourcesIcons;

    [Header("Windows")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject mainWindow;
    [SerializeField] private GameObject miniWindow;
    [SerializeField] private GameObject emptyWindow;
    [SerializeField] private GameObject remoteWindow;

    [Header("Start Parameters")]
    [SerializeField] private TMP_Text totalCells;
    [SerializeField] private TMP_Text rewards;
    [SerializeField] private TMP_Text attempts;
    [SerializeField] private TMP_Text helpPoints;

    private GameObject currentCamp;
    private CampGameParameters currentParameters;

    private void Start()
    {
        campManager = GlobalStorage.instance.campManager;
        campGame = GetComponent<CampGame>();

        //resourcesManager = GlobalStorage.instance.resourcesManager;
        //resourcesIcons = resourcesManager.GetAllResourcesIcons();

        canvas = uiPanel.GetComponent<CanvasGroup>();
    }

    public void Open(bool modeClick, GameObject camp)
    {
        MenuManager.instance.MiniPause(true);
        uiPanel.SetActive(true);

        currentCamp = camp;
        Init(modeClick);

        Fading.instance.FadeWhilePause(true, canvas);
    }

    //Button
    public void Close()
    {
        mainWindow.SetActive(false);
        miniWindow.SetActive(false);

        MenuManager.instance.MiniPause(false);
        uiPanel.SetActive(false);
    }

    public void Init(bool modeClick)
    {
        bool campStatus = campManager.GetCampStatus(currentCamp);
        Refactoring(modeClick, campStatus);

        if(campStatus == false) return;

        currentParameters = campManager.GetStartParameters();
        FillParameters();

        campGame.ResetCells();
        campGame.PreparedToGame(currentParameters);
    }

    private void Refactoring(bool modeClick, bool statusMode)
    {
        mainWindow.SetActive(false);
        miniWindow.SetActive(false);
        emptyWindow.SetActive(false);
        remoteWindow.SetActive(false);

        if(modeClick == true)
        {
            miniWindow.SetActive(true);

            if(statusMode == true)
                remoteWindow.SetActive(true);
            else
                emptyWindow.SetActive(true);
        }
        else
        {
            if(statusMode == true)
            {
                mainWindow.SetActive(true);
            }
            else
            {
                miniWindow.SetActive(true);
                emptyWindow.SetActive(true);
            }
        }
    }

    public void FillParameters()
    {
        totalCells.text = currentParameters.cellsAmount.ToString();
        rewards.text = currentParameters.rewardsAmount.ToString();
        attempts.text = currentParameters.attempts.ToString();
        helpPoints.text = currentParameters.helps.ToString();
    }
}
