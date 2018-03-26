using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerControls player);
}

public class PickUpable : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject item;

    public void Interact(PlayerControls player)
    {
        player.InteractTime = 2f;
        player.Anim.SetTrigger("PickUp");
        player.Inventory.NewEquippable(item);
        print("Add item");
        Destroy(this.gameObject, 2);
    }
}
