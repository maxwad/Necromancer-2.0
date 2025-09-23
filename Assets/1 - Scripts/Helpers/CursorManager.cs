using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using static Enums;

public class CursorManager : MonoBehaviour
{
    private bool canIChange = true;

    CursorView cursorView = CursorView.Default;
    //public GameObject cursor;
    //private SpriteRenderer sprite;

    public Texture2D cursorPict;
    public Texture2D cursorDefault;
    public Texture2D cursorBattle;
    public Texture2D cursorMovement;
    public Texture2D cursorAction;
    public Texture2D cursorEnter;

    private Vector2 position;

    private GlobalMapTileManager gmManager;
    private ClickableObject currentObject = null;

    [Inject]
    public void Construct(GlobalMapTileManager gmManager)
    {
        this.gmManager = gmManager;
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.SetCursor(cursorDefault, Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
    }

    private void LateUpdate()
    {
        return; // delete in build

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int positionOnTileMap;

        positionOnTileMap = gmManager.mapBG.WorldToCell(mousePosition);
        if(gmManager.mapBG.HasTile(positionOnTileMap) == true) cursorView = CursorView.Default;

        positionOnTileMap = gmManager.roadMap.WorldToCell(mousePosition);
        if(gmManager.roadMap.HasTile(positionOnTileMap) == true) cursorView = CursorView.Movement;

        if(currentObject != null) cursorView = currentObject.cursor;

        positionOnTileMap = gmManager.fogMap.WorldToCell(mousePosition);
        if(gmManager.fogMap.HasTile(positionOnTileMap) == true) cursorView = CursorView.Default;

        if(EventSystem.current.IsPointerOverGameObject() == true) cursorView = CursorView.Default;

        ChangeCursor(cursorView);
    }

    private void CursorMode(bool mode)
    {
        canIChange = mode;
    }

    public void SetCurrentObjectUnderMouse(ClickableObject obj)
    {
        currentObject = obj;
    }

    public void ChangeCursor(CursorView newCursor)
    {
        if(canIChange == true)
        {
            switch(newCursor)
            {
                case CursorView.Default:
                    cursorPict = cursorDefault;
                    break;

                case CursorView.Battle:
                    cursorPict = cursorBattle;
                    break;

                case CursorView.Movement:
                    cursorPict = cursorMovement;
                    break;

                case CursorView.Action:
                    cursorPict = cursorAction;
                    break;

                case CursorView.Enter:
                    cursorPict = cursorEnter;
                    break;

                default:
                    break;
            }
        }
        else
        {
            cursorPict = cursorDefault;
        }        

        position = Vector2.zero;
        if(cursorPict == cursorMovement) position = new Vector2(cursorPict.width / 3.5f, cursorPict.height / 3.3f);

        Cursor.SetCursor(cursorPict, position, UnityEngine.CursorMode.ForceSoftware);
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += CursorMode;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= CursorMode;
    }
}
