using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemperCommonUIManager : MonoBehaviour
{
    public GameObject uiPanel;
    public TMP_Text caption;
    public TMP_Text status;

    public void OpenWindow(bool mode, ClickableObject obj)
    {
        GlobalStorage.instance.isModalWindowOpen = true;
        uiPanel.SetActive(true);

        Initialize(mode, obj);
    }

    public void Initialize(bool mode, ClickableObject obj)
    {
        //mode = true - by rigth click; mode = false - by movement and we need a player
        //
        string captionText = obj.gameObject.GetComponent<TooltipTrigger>().header;
        if(captionText.Length == 0) captionText = obj.gameObject.name;

        caption.text = captionText;
        status.text = (mode == true) ? "opened by r-click" : "opened by movement";
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;

        uiPanel.SetActive(false);
    }

}
