using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;

public class TombUI : MonoBehaviour
{
    private CanvasGroup canvas;
    private TombsManager tombsManager;

    [SerializeField] private GameObject uiPanel;
    private GameObject tomb;
    private ObjectOwner owner;
    private SpellSO spell;

    [Header("Blocks")]
    [SerializeField] private GameObject defaultBlock;
    [SerializeField] private GameObject emptyBlock;
    [SerializeField] private GameObject infoBlock;

    [Header("Info Details")]
    [SerializeField] private TMP_Text title;

    private void Start()
    {       
        tombsManager = GlobalStorage.instance.tombsManager;
        canvas = uiPanel.GetComponent<CanvasGroup>();
    }

    public void Open(bool clickMode, GameObject tomb)
    {
        MenuManager.instance.MiniPause(true);
        uiPanel.SetActive(true);

        this.tomb = tomb;
        Init(clickMode);

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        MenuManager.instance.MiniPause(false);
        uiPanel.SetActive(false);
    }

    public void Init(bool clickMode)
    {
        defaultBlock.SetActive(false);
        emptyBlock.SetActive(false);
        infoBlock.SetActive(false);

        owner = tomb.GetComponent<ObjectOwner>();
        spell = tombsManager.GetSpell(tomb);
        bool defaultMode = false;

        if(owner.GetVisitStatus() == false)
        {
            if(clickMode == true)
            {
                defaultMode = true;
            }
            else
            {
                tombsManager.UnlockSpell(spell);
                owner.SetVisitStatus();
            }
        }       

        ShowSpell(defaultMode);
    }

    private void ShowSpell(bool defaultMode)
    {
        if(defaultMode == true)
        {
            defaultBlock.SetActive(true);
        }
        else
        {
            if(spell == null)
            {
                emptyBlock.SetActive(true);
            }
            else
            {
                ShowDetails();
            }
        }
    }

    private void ShowDetails()
    {
        infoBlock.SetActive(true);
    }
}
