using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemperCommonUIManager : MonoBehaviour
{
    public GameObject uiPanel;
    public TMP_Text caption;
    public TMP_Text status;
    public GameObject uiPanelMini;
    public TMP_Text captionMini;
    public TMP_Text statusMini;

    public void OpenWindow(bool modeClick, bool modeNormalUISize, ClickableObject obj)
    {
        GlobalStorage.instance.isModalWindowOpen = true;

        if(modeNormalUISize == true)
            uiPanel.SetActive(true);
        else
            uiPanelMini.SetActive(true);

        Initialize(modeClick, modeNormalUISize, obj);
    }

    public void Initialize(bool modeClick, bool modeNormalUISize, ClickableObject obj)
    {
        //mode = true - by rigth click; mode = false - by movement and we need a player
        string captionText = obj.gameObject.GetComponent<TooltipTrigger>().header;
        if(captionText.Length == 0) captionText = obj.gameObject.name;

        if(modeNormalUISize == true)
        {
            caption.text = captionText;
            status.text = (modeClick == true) ? "opened by r-click" : "opened by movement";
        }
        else
        {
            captionMini.text = captionText;
            statusMini.text = (modeClick == true) ? "opened by r-click" : "opened by movement";
        }
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;

        uiPanel.SetActive(false);
        uiPanelMini.SetActive(false);
    }

}
