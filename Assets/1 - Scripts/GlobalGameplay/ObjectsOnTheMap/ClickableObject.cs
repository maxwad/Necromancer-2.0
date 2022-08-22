using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class ClickableObject : MonoBehaviour
{
    private GlobalMapPathfinder gmPathFinder;
    private CursorManager cursorManager;
    public CursorView cursor = CursorView.Default;

    private void Start()
    {
        gmPathFinder = GlobalStorage.instance.gmManager.GetComponent<GlobalMapPathfinder>();
        cursorManager = GlobalStorage.instance.cursorManager;
    }

    private void OnMouseDown()
    {
        if(EventSystem.current.IsPointerOverGameObject()) return;

        //gmPathFinder.HandleClick(gameObject);
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
