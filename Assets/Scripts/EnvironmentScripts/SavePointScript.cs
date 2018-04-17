using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointScript : MonoBehaviour {



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SaveManager saver = FindObjectOfType<SaveManager>();
            saver.SaveGame();
        }
    }
}
