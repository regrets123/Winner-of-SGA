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
        //Locks cursor in the middle of the screen and hides it out of the way when game starts
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetInputMode(InputMode newMode)
    {
        if (newMode != currentInputMode)
        {
            //If a menu of sorts is brought up like pause screen or inventory the cursor is visable and unlocked again. It's locked and becomes invisable if its the other way around
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
