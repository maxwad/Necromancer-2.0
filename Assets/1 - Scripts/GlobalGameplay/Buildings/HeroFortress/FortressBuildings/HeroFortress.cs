using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class HeroFortress : MonoBehaviour
{
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private List<FBuilding> allBuildings;
    [SerializeField] private List<FBuilding> activeBuildings;

    private bool isSiege = false;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        //TEMPER
        Close();
    }

    public void Open(bool openMode, ClickableObject building)
    {
        gmInterface.ShowInterfaceElements(false);

        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        Fading.instance.Fade(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);

        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }
}
