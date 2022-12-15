using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class ResourceBuildingDoor : MonoBehaviour
{
    private ResourceBuilding currentBuilding;
    private ResourcesManager resourcesManager;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    private bool isWindowOpen = false;
    private bool isHeroInside = false;

    [Header("UI")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_Text caption;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
    }

    public void Open(bool openByClick, ResourceBuilding rBuilding)
    {
        currentBuilding = rBuilding;
        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        isWindowOpen = true;

        isHeroInside = !openByClick;

        Init();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);

        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        isWindowOpen = false;

        uiPanel.SetActive(false);
    }

    private void Init()
    {

    }


}
