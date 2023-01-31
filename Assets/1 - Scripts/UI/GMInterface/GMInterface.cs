using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class GMInterface : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;

    [Header("Parts")]
    [HideInInspector] public GMInterfaceCastle castlePart;
    [HideInInspector] public GMInterfaceResources resourcesPart;
    [HideInInspector] public GMInterfaceHero heroPart;
    [HideInInspector] public GMInterfaceMoves movesPart;    
    [HideInInspector] public GMInterfaceCalendar calendarPart;    
    [HideInInspector] public GMInterfaceTurn turnPart;

    private void Awake()
    {
        castlePart = GetComponent<GMInterfaceCastle>();
        resourcesPart = GetComponent<GMInterfaceResources>();
        heroPart = GetComponent<GMInterfaceHero>();
        movesPart = GetComponent<GMInterfaceMoves>();
        calendarPart = GetComponent<GMInterfaceCalendar>();

        turnPart = GetComponent<GMInterfaceTurn>();
    }

    private void Start()
    {
        EnableUI(true);
    }

    private void EnableUI(bool mode)
    {
        uiPanel.SetActive(mode);
    }

    public void ShowInterfaceElements(bool mode)
    {
        if(GlobalStorage.instance.isGlobalMode == false) return;

        calendarPart.ShowBlock(mode);
        movesPart.ShowBlock(mode);
        heroPart.ShowBlock(mode);
        castlePart.ShowBlock(mode);
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += EnableUI;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= EnableUI;
    }
}
