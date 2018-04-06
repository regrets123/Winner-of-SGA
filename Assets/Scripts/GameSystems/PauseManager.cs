using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*By Björn Andersson*/

//Interface som implementeras av allt som ska kunna pausas
public interface IPausable
{
    void PauseMe(bool pausing);
}

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    InputManager iM;

    bool paused = false;

    //Lista av allt som kan pausas
    List<IPausable> pausables = new List<IPausable>();

    InputMode previousInputMode = InputMode.None;

    InventoryManager playerInventory;

    public List<IPausable> Pausables
    {
        get { return pausables; }
    }

    private void Awake()
    {
        iM = GetComponent<InputManager>();
        iM.SetInputMode(InputMode.Playing);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (playerInventory == null)
            {
                playerInventory = FindObjectOfType<InventoryManager>();
            }
            if (playerInventory.InventoryMenu.activeSelf)
            {
                playerInventory.HideInventory();
            }
            else
                PauseAndUnpause(false);
        }
    }

    //Pausar/unpausar spelet och tar fram/döljer pausmenyn
    public void PauseAndUnpause(bool inventory)
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0f;
            previousInputMode = iM.CurrentInputMode;
            if (!inventory)
                iM.SetInputMode(InputMode.Paused);
            else
                iM.SetInputMode(InputMode.Inventory);
        }
        else
        {
            Time.timeScale = 1f;
            iM.SetInputMode(previousInputMode);
        }
        pauseMenu.SetActive(paused);
        foreach (IPausable pauseMe in pausables)
        {
            if (pauseMe != null)
                pauseMe.PauseMe(paused);
        }
    }

    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }
}
