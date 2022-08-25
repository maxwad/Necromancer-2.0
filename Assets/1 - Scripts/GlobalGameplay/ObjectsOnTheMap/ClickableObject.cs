using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class ClickableObject : MonoBehaviour
{
    public TypeOfObjectOnTheMap objectType;
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

    public void ActivateUIWindow(bool mode)
    {
        //mode = true - by rigth click; mode = false - by movement

        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(GlobalStorage.instance.isModalWindowOpen == false)
        {
            if(canBeOpenedByClick == false && mode == true) return;
            if(canBeOpenedByMove == false && mode == false) return;

            GlobalStorage.instance.isModalWindowOpen = true;

            CallManagerYouNeed(objectType, mode);            
        }
    }

    private void CallManagerYouNeed(TypeOfObjectOnTheMap type, bool mode)
    {
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
                GlobalStorage.instance.portalsManager.OpenWindow(mode, this);
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
