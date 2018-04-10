﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrigger : MonoBehaviour
{
    [SerializeField]
    string [] loadNames, unloadNames;

	private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (loadNames != null)
            {
                foreach (string scene in loadNames)
                    DynamicSceneManager.instance.Load(scene);
            }

            if (unloadNames != null)
            {
                StartCoroutine("UnloadScene");
            }
        }
    }

    IEnumerator UnloadScene()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (string scene in unloadNames)
        {
            DynamicSceneManager.instance.UnLoad(scene);
        }
    }
}
