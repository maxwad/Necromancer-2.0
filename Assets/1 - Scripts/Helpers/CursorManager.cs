using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class CursorManager : MonoBehaviour
{
    private bool canIChange = true;

    //public GameObject cursor;
    //private SpriteRenderer sprite;

    public Texture2D cursorPict;
    public Texture2D cursorDefault;
    public Texture2D cursorBattle;
    public Texture2D cursorMovement;
    public Texture2D cursorAction;
    public Texture2D cursorEnter;

    private Vector2 position;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.SetCursor(cursorDefault, Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
    }

    //private void LateUpdate()
    //{
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mousePos.z = 0;
    //    cursor.transform.localPosition = mousePos;
    //}

    private void CursorMode(bool mode)
    {
        canIChange = mode;
    }

    public void ChangeCursor(CursorView newCursor)
    {
        return; // delete in build

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

        if(EventSystem.current.IsPointerOverGameObject() == true) cursorPict = cursorDefault;

        position = Vector2.zero;
        if(cursorPict == cursorMovement) position = new Vector2(cursorPict.width / 3.5f, cursorPict.height / 3.3f);

        Cursor.SetCursor(cursorPict, position, UnityEngine.CursorMode.ForceSoftware);
    }

    private void OnEnable()
    {
        EventManager.ChangePlayer += CursorMode;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer -= CursorMode;
    }
}
