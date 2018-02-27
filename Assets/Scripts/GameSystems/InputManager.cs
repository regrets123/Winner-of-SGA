using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public enum InputMode
{
    None, Playing, Paused, Inventory
}

public class InputManager : MonoBehaviour
{

    static InputMode currentInputMode = InputMode.Playing;

    public InputMode CurrentInputMode
    {
        get { return currentInputMode; }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetInputMode(InputMode newMode)
    {
        if (newMode != currentInputMode)
        {
            currentInputMode = newMode;
            if (currentInputMode == InputMode.None || currentInputMode == InputMode.Paused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
