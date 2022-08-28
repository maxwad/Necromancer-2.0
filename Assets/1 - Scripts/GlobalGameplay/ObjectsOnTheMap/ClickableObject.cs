using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class ClickableObject : MonoBehaviour
{
    public TypeOfObjectOnTheMap objectType;
    public bool isNormalUIWindow = true;
    public bool canBeOpenedByClick = true;
    public bool canBeOpenedByMove = true;

    [HideInInspector] public TooltipTrigger tooltip;
    private CursorManager cursorManager;
    public CursorView cursor = CursorView.Default;

    private void Start()
    {        
        cursorManager = GlobalStorage.instance.cursorManager;
        tooltip = GetComponent<TooltipTrigger>();
    }

    public void ActivateUIWindow(bool modeClick)
    {
        //mode = true - by rigth click; mode = false - by movement

        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(GlobalStorage.instance.isModalWindowOpen == false)
        {
            if(canBeOpenedByClick == false && modeClick == true) return;
            if(canBeOpenedByMove == false && modeClick == false) return;

            CallManagerYouNeed(objectType, modeClick, isNormalUIWindow);            
        }
    }

    private void CallManagerYouNeed(TypeOfObjectOnTheMap type, bool modeClick, bool modeUISize)
    {
        bool isThereManager = false;
        switch(type)
        {
            case TypeOfObjectOnTheMap.PlayersCastle:
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

            case TypeOfObjectOnTheMap.Portal:
                GlobalStorage.instance.portalsManager.OpenWindow(modeClick, this);
                isThereManager = true;
                break;

            case TypeOfObjectOnTheMap.RoadPointer:
                break;

            case TypeOfObjectOnTheMap.Arena:
                break;

            case TypeOfObjectOnTheMap.Tomb:
                break;

            default:
                break;
        }

        if(isThereManager == false) GlobalStorage.instance.commonUIManager.OpenWindow(modeClick, modeUISize, this);
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
