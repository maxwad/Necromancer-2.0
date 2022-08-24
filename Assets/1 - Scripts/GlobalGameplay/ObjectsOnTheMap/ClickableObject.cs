using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class ClickableObject : MonoBehaviour
{
    public TypeOfObjectOnTheMap objectType;
    public bool openByClick = true;
    public bool openByMove = true;
    public GameObject uiPanel;

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
        //mode = true - by rigth click
        //mode = false - by movement
        if(uiPanel == null) return;

        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(GlobalStorage.instance.isModalWindowOpen == false)
        {
            if(openByClick == false && mode == true) return;
            if(openByMove == false && mode == false) return;

            GlobalStorage.instance.isModalWindowOpen = true;

            Invoke("OpenWindow", 0.1f);
            uiPanel.GetComponent<ObjectsInfoUI>()?.Initialize(mode, this);            
        }
    }

    private void OpenWindow()
    {
        // we need this delay for right value isOpenedByMovement in ObjectsInfoUI script
        uiPanel.SetActive(true);
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
