using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private List<KeyAction> keyActions;

    private List<InputAction> inputKeyActions = new List<InputAction>();
    private List<KeyActions> registeredActions = new List<KeyActions>();
    private List<IInputableAxies> inputAxiesActions = new List<IInputableAxies>();

    private bool isInputEnable = true;

    public void RegisterInputKeys(KeyActions keyAction, IInputableKeys objectToActivate)
    {
        if(registeredActions.Contains(keyAction) == true)
        {
            inputKeyActions.Remove(inputKeyActions.Where(i => i.keyAction == keyAction).First());
            registeredActions.Remove(keyAction);
        }

        inputKeyActions.Add(new InputAction(keyAction, objectToActivate));
        registeredActions.Add(keyAction);
    }

    public void RegisterInputAxies(IInputableAxies objectToActivate)
    {
        inputAxiesActions.Add(objectToActivate);
    }

    private void Update()
    {
        InputKeysHandling();
        InputAxiesHandling();
    }

    private void InputKeysHandling()
    {
        if(isInputEnable == true)
        {
            foreach(var key in keyActions)
            {
                if(Input.GetKeyDown(key.key) == true)
                {
                    foreach(var action in inputKeyActions)
                    {
                        if(action.keyAction == key.action)
                        {
                            action.objectToActivate.InputHandling(key.action);
                        }
                    }
                }
            }  
        }
    }

    private void InputAxiesHandling()
    {        
        foreach(var action in inputAxiesActions)
        {
            action.InputHandling(
                new AxiesData(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical"),
                    Input.GetAxisRaw("Rotation"),
                    Input.GetAxis("Mouse ScrollWheel")
                    ),
                new MouseData(
                    Input.mousePosition,
                    (isInputEnable == true) ? Input.GetMouseButton(0) : false,
                    Input.GetMouseButton(2),
                    (isInputEnable == true) ? Input.GetMouseButton(1) : false,
                    (isInputEnable == true) ? Input.GetMouseButtonDown(0) : false,
                    Input.GetMouseButtonDown(2),
                    (isInputEnable == true) ? Input.GetMouseButtonDown(1) : false
                    )
                );
        }
        
    }

    public void ActivateInput(bool activeMode)
    {
        isInputEnable = activeMode;
    }
}
