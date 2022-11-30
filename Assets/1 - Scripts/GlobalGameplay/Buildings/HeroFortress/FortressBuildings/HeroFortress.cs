using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class HeroFortress : MonoBehaviour
{
    private OpeningBuildingWindow door;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private List<FBuilding> allBuildings;
    [SerializeField] private List<FBuilding> activeBuildings;

    private bool isSiege = false;

    private void Start()
    {
        door = GlobalStorage.instance.fortressBuildingDoor;
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        //TEMPER
        Close();
    }

    public void Open(bool openMode, ClickableObject building)
    {
        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);

        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        door.Close();
        uiPanel.SetActive(false);
    }
}
