using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicSceneManager : MonoBehaviour
{

    [SerializeField]
    private string StartingArea;
    [SerializeField]
    private string FirstProps;
    [SerializeField]
    private string TerrainScene;
    [SerializeField]
    private string Canyons;
    [SerializeField]
    private string TempleMonument;

    public static DynamicSceneManager instance { get; set; }

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Load(StartingArea);
        Load(FirstProps);
        Load(TerrainScene);
        Load(Canyons);
        Load(TempleMonument);
        Application.backgroundLoadingPriority = ThreadPriority.High;
    }


    public void Load(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
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
