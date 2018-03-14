using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour, IInteractable
{
    Animator anim;

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public void Interact(PlayerControls player)
    {
        anim.SetTrigger("OpenDoor");
    }
}
