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

    void Awake()
    {
        DontDestroyOnLoad(this);
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
        }
        else if(sceneName == "Prototype_TestScene_JP")
        {
            loadGameMenu.SetActive(false);
            newGameMenu.SetActive(false);
            backInGame.SetActive(true);
            backMainMenu.SetActive(false);
            gui.SetActive(true);
        }

    }
}
