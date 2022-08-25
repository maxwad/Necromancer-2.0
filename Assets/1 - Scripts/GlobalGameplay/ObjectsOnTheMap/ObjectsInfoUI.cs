using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectsInfoUI : MonoBehaviour
{
    public TMP_Text caption;
    public TMP_Text status;

    public void Initialize(bool mode, ClickableObject obj)
    {
        //mode = true - by rigth click; mode = false - by movement and we need a player

        caption.text = obj.tooltip.header;        
        status.text = (mode == true) ? "opened by r-click" : "opened by movement";
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;

        gameObject.SetActive(false);
    }
}
