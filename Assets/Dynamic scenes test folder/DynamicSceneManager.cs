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
        Load("Player_Final");
        Load(StartingArea);
        Load(FirstProps);
        Load(TerrainScene);
        Load(Canyons);
        Load(TempleMonument);
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
