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

    InventoryManager playerInventory;

    public InputMode CurrentInputMode
    {
        get { return currentInputMode; }
    }

    public InventoryManager PlayerInventory
    {
        set { if (this.playerInventory == null) this.playerInventory = value; }
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
            if (currentInputMode == InputMode.None || currentInputMode == InputMode.Paused || currentInputMode == InputMode.Inventory)
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
    
    public void ApplyUpgrade()
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.ApplyUpgrade();
    }

    public void AddFavorite()
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.AddFavorite();
    }

    public void SelectUpgrade(int upgradeIndex)
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.SelectUpgrade(upgradeIndex);
    }

    public void SelectEquipable(int index)
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.SelectItem(index);
    }

    public void ViewInventoryCollection(int displayCollection)
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.DisplayNewCollection(displayCollection);
    }

    public void Equip()
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.Equip();
    }

    public void ShowUpgradeOptions(bool show)
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.ShowUpgradeOptions(show);
    }

    public void ChangeInventoryCollection(int collection)
    {
        if (playerInventory == null)
            this.playerInventory = FindObjectOfType<InventoryManager>();
        playerInventory.DisplayNewCollection(collection);
    }

    public void GoToMenu()
    {
        SetInputMode(InputMode.None);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
