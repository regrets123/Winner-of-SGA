using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*By Andreas Nilsson*/

public class DynamicSceneManager : MonoBehaviour
{
    /*
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
    */
    public static DynamicSceneManager instance { get; set; }
    
    private void Awake()        //Laddar in de områden som ska laddas då spelet startar
    {
        /*
        instance = this;
        Load(StartingArea);
        Load(FirstProps);
        Load(TerrainScene);
        Load(Canyons);
        Load(TempleMonument);
        */
        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }


    public void Load(string sceneName)      //Laddar en scen additivt så att den är aktiv tillsammans med redan aktiva scener
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public void UnLoad(string sceneName)        //Stänger av en aktiv scen
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
