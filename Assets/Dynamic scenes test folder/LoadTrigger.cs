using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    [SerializeField]
    string loadName, unloadName;

	private void OnTriggerEnter(Collider col)
    {
        if(loadName != "")
        {
            DynamicSceneManager.instance.Load(loadName);
        }

        if(unloadName != "")
        {
            StartCoroutine("UnloadScene");
        }
    }

    IEnumerator UnloadScene()
    {
        yield return new WaitForSeconds(0.1f);
        DynamicSceneManager.instance.UnLoad(unloadName);
    }
}
