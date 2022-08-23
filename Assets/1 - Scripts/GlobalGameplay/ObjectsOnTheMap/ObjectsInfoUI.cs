using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectsInfoUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text caption;
    public TMP_Text status;

    public void OpenWindow(bool mode, ClickableObject obj)
    {
        //mode = true - by rigth click
        //mode = false - by movement and we need a player
        GlobalStorage.instance.isModalWindowOpen = true;
        panel.SetActive(true);

        caption.text = obj.tooltip.header;        
        status.text = (mode == true) ? "opened by r-click" : "opened by movement";

        switch(obj.objectType)
        {
            case NameManager.TypeOfObjectOnTheMap.PlayersCastle:
                break;
            case NameManager.TypeOfObjectOnTheMap.NecromancerCastle:
                break;
            case NameManager.TypeOfObjectOnTheMap.Castle:
                break;
            case NameManager.TypeOfObjectOnTheMap.ResoursesFarm:
                break;
            case NameManager.TypeOfObjectOnTheMap.ResoursesQuarry:
                break;
            case NameManager.TypeOfObjectOnTheMap.ResoursesMine:
                break;
            case NameManager.TypeOfObjectOnTheMap.ResoursesSawmill:
                break;
            case NameManager.TypeOfObjectOnTheMap.Outpost:
                break;
            case NameManager.TypeOfObjectOnTheMap.Camp:
                break;
            case NameManager.TypeOfObjectOnTheMap.Altar:
                break;
            case NameManager.TypeOfObjectOnTheMap.Portal:
                break;
            case NameManager.TypeOfObjectOnTheMap.RoadPointer:
                break;
            case NameManager.TypeOfObjectOnTheMap.Arena:
                break;
            case NameManager.TypeOfObjectOnTheMap.Tomb:
                break;
            default:
                break;
        }
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;

        panel.SetActive(false);
    }

    private void OpenPortalsWindow()
    {

    }
}
