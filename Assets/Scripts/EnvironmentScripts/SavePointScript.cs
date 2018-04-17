using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class SavePointScript : MonoBehaviour, IInteractable
{
    public void Reskin(Material newMat)
    {
        GetComponent<Renderer>().material = newMat;
    }

    public void Interact(PlayerControls player)
    {
        SaveManager saver = FindObjectOfType<SaveManager>();
        saver.SaveGame(this.gameObject);
    }
}
