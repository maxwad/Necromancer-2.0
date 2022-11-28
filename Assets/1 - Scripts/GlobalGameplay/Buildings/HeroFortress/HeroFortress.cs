using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class HeroFortress : MonoBehaviour
{
    private GMInterface gmInterface;
    private CanvasGroup canvas;

    [SerializeField] private GameObject uiPanel;


    private bool isSiege = false;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        //TEMPER
        CloseWindow();
    }

    public void OpenWindow(bool mode)
    {
        gmInterface.ShowInterfaceElements(false);

        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        if(canvas == null) canvas = uiPanel.GetComponent<CanvasGroup>();
        Fading.instance.Fade(true, canvas);
    }

    public void CloseWindow()
    {
        gmInterface.ShowInterfaceElements(true);

        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }

}
