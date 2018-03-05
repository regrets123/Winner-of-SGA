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
    static List<IPausable> pausables = new List<IPausable>();

    InputMode previousInputMode = InputMode.None;

    public List<IPausable> Pausables
    {
        get { return pausables; }
    }

    private void Start()
    {
        iM = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseAndUnpause();
        }
    }

    //Pausar/unpausar spelet och tar fram/döljer pausmenyn
    public void PauseAndUnpause()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0f;
            previousInputMode = iM.CurrentInputMode;
            iM.SetInputMode(InputMode.Paused);
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
    
    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ViewOptions()
    {
        //Toggle options menu
    }

    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }
}
