using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*By Andreas Nilsson*/

public class DynamicSceneManager : MonoBehaviour
{
    [SerializeField]
    private string masterSceneTriggers;
    [SerializeField]
    private string terrainScene;
    [SerializeField]
    private string startArea;
    [SerializeField]
    private string bridgeScene;

    public static DynamicSceneManager instance { get; set; }
    
    private void Awake()        //Laddar in de områden som ska laddas då spelet startar
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        instance = this;

        StartCoroutine(Load(masterSceneTriggers));
        StartCoroutine(Load(terrainScene));
        StartCoroutine(Load(startArea));
        StartCoroutine(Load(bridgeScene));
    }

    public IEnumerator Load(string sceneName)      //Laddar en scen additivt så att den är aktiv tillsammans med redan aktiva scener
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public IEnumerator UnLoad(string sceneName)        //Stänger av en aktiv scen
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
