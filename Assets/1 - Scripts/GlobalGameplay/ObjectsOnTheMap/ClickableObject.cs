using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static NameManager;

public class ClickableObject : MonoBehaviour
{
    public TypeOfObjectOnTheMap objectType;
    public bool openByClick = true;
    public bool openByMove = true;
    private ObjectsInfoUI objectsInfo;

    [HideInInspector] public TooltipTrigger tooltip;
    private CursorManager cursorManager;
    public CursorView cursor = CursorView.Default;

    private void Start()
    {        
        cursorManager = GlobalStorage.instance.cursorManager;
        tooltip = GetComponent<TooltipTrigger>();
        objectsInfo = GlobalStorage.instance.objectsInfoUI;
    }

    public void ActivateUIWindow(bool mode)
    {
        //mode = true - by rigth click
        //mode = false - by movement

        if(EventSystem.current.IsPointerOverGameObject()) return;
        if(GlobalStorage.instance.isModalWindowOpen == false)
        {
            if(openByClick == false && mode == true) return;
            if(openByMove == false && mode == false) return;

            objectsInfo?.OpenWindow(mode, this);
        }
    }

    private void OnMouseEnter()
    {
        cursorManager.SetObject(this);
    }

    private void OnMouseExit()
    {
        cursorManager.SetObject(null);
    }
}
