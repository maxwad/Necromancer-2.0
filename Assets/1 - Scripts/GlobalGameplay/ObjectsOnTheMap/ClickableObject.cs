using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using static NameManager;

public class ClickableObject : MonoBehaviour
{
    public TypeOfObjectOnTheMap objectType;
    public bool isItDoor = false;

    public bool isNormalUIWindow = true;
    public bool canBeOpenedByClick = true;
    public bool canBeOpenedByMove = true;

    //[HideInInspector] public TooltipTrigger tooltip;
    private CursorManager cursorManager;
    public CursorView cursor = CursorView.Default;

    private void Start()
    {        
        cursorManager = GlobalStorage.instance.cursorManager;
        //tooltip = GetComponent<TooltipTrigger>();
    }

    public void ActivateUIWindow(bool modeClick)
    {
        //mode = true - by rigth click; mode = false - by movement
        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(GlobalStorage.instance.isModalWindowOpen == false)
        {
            if(canBeOpenedByClick == false && modeClick == true) return;
            if(canBeOpenedByMove == false && modeClick == false) return;

            if(isItDoor == false)
            {
                CallManagerYouNeed(modeClick, isNormalUIWindow);
            }
            else
            {
                OpenDoorTo(modeClick);
            }                      
        }
    }

    private void CallManagerYouNeed(bool modeClick, bool modeUISize)
    {
        bool isThereManager = false;

        switch(objectType)
        {
            case TypeOfObjectOnTheMap.Portal:
                GlobalStorage.instance.portalsManager.OpenWindow(modeClick, this);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.RoadPointer:
                break;

            case TypeOfObjectOnTheMap.Resource:
                GlobalStorage.instance.bonusOnTheMapUI.OpenWindow(this);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.BoxBonus:
                GlobalStorage.instance.bonusOnTheMapUI.OpenWindow(this);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Enemy:
                GlobalStorage.instance.enemyArmyUI.OpenWindow(modeClick, gameObject.GetComponent<EnemyArmyOnTheMap>());
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.Player:
                GlobalStorage.instance.playerMilitaryWindow.OpenWindow(PlayersWindow.PlayersArmy);
                isThereManager = true;
                break;


            default:
                break;
        }

        if(isThereManager == false) GlobalStorage.instance.commonUIManager.OpenWindow(modeClick, modeUISize, this);
    }

    private void OpenDoorTo(bool modeClick)
    {
        bool isThereManager = false;

        switch(objectType)
        {
            case TypeOfObjectOnTheMap.PlayersCastle:
                GlobalStorage.instance.heroFortress.Open(modeClick, this);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.NecromancerCastle:
                break;

            case TypeOfObjectOnTheMap.Castle:
                break;

            case TypeOfObjectOnTheMap.ResoursesFarm:
                break;

            case TypeOfObjectOnTheMap.ResoursesQuarry:
                break;

            case TypeOfObjectOnTheMap.ResoursesMine:
                break;

            case TypeOfObjectOnTheMap.ResoursesSawmill:
                break;

            case TypeOfObjectOnTheMap.Outpost:
                break;

            case TypeOfObjectOnTheMap.Camp:
                break;

            case TypeOfObjectOnTheMap.Altar:
                break;

            case TypeOfObjectOnTheMap.Arena:
                break;

            case TypeOfObjectOnTheMap.Tomb:
                break;

            default:
                break;
        }

        if(isThereManager == false) GlobalStorage.instance.commonUIManager.OpenWindow(modeClick, true, this);
    }

    private void OnMouseEnter()
    {
        cursorManager.SetCurrentObjectUnderMouse(this);
    }

    private void OnMouseExit()
    {
        cursorManager.SetCurrentObjectUnderMouse(null);
    }
}
