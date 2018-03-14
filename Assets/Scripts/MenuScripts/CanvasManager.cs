using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/*By Johanna Pettersson*/

public class CanvasManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject loadGameMenu;
    public GameObject newGameMenu;
    public GameObject gui;
    public GameObject pauseMenu;

    public GameObject backMainMenu;
    public GameObject backInGame;

    InputManager iM;
    PauseManager pM;

    static bool created = false;
    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

        iM = this.gameObject.GetComponent<InputManager>();
        pM = this.gameObject.GetComponent<PauseManager>();
        if (iM != null && pM != null)
        {
            iM.enabled = false;
            pM.enabled = false;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        SceneManager.sceneLoaded += OnSceneLoaded;

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == ("Prototype_MainMenu_JP"))
        {
            mainMenu.SetActive(true);
            backMainMenu.SetActive(true);
            backInGame.SetActive(false);
            pauseMenu.SetActive(false);
            gui.SetActive(false);
            if(iM != null && pM != null)
            {
                iM.enabled = false;
                pM.enabled = false;
            }

        }
        else if(sceneName == "Prototype_TestScene_JP")
        {
            loadGameMenu.SetActive(false);
            newGameMenu.SetActive(false);
            backInGame.SetActive(true);
            backMainMenu.SetActive(false);
            gui.SetActive(true);
            iM.enabled = true;
            pM.enabled = true;
        }

    }
}
