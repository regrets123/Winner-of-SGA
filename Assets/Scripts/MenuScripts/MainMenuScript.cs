using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*By Johanna Pettersson*/

public class MainMenuScript : MonoBehaviour {

    void Start()
    {
        // Find Saved Games and inactivate loadgamesMenu
    }

    public void ToggleMenu(GameObject menuToToggle)
    {
        menuToToggle.SetActive(!menuToToggle.activeSelf);

    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame()
    {
        //Check if there's a saved game. Start new game, load scene
    }

    public void LoadGame()
    {
        //
    }


    public void ExitApplication()
    {
        Application.Quit();
    }


}
