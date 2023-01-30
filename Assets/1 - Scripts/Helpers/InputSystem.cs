using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private List<KeyAction> keyActions;

    private List<InputAction> inputKeyActions = new List<InputAction>();
    private List<InputAction> inputAxiesActions = new List<InputAction>();
    private List<InputAction> inputMouseActions = new List<InputAction>();

    public void RegisterInput(KeyActions keyAction, IInputable objectToActivate)
    {
        inputKeyActions.Add(new InputAction(keyAction, objectToActivate));
    }

    private void Update()
    {
        InputHandling();
    }

    private void InputHandling()
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
