using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Johanna Pettersson*/

public class MenuManager : MonoBehaviour {

    public GameObject settingsMenu;
    public GameObject pauseMenu;
	
    public void SettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

}
