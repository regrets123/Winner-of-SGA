using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicSceneManager : MonoBehaviour
{

    public static DynamicSceneManager instance { get; set; }

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Load("Player Scene");
        Load("Dynamic Scene 1");
        Load("Dynamic Scene 2");
    }

    public void Load(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    public void UnLoad(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
