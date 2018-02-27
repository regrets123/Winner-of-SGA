using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class InventoryManager : MonoBehaviour
{

    [SerializeField]
    GameObject inventoryMenu;

    List<BaseEquippableScript> equippableWeapons, equippableAbilities, equippableItems;

    List<BaseEquippableScript>[] inventory = new List<BaseEquippableScript>[2];

    PlayerControls player;

    int displayCollection = 0, collectionIndex = 0;

    BaseEquippableScript currentChoice;

    public InventoryManager(PlayerControls player)
    {
        this.player = player;
        inventory[0] = new List<BaseEquippableScript>();
        inventory[1] = new List<BaseEquippableScript>();
        //inventory[2] = new List<BaseEquippableScript>();
        equippableWeapons = inventory[0];
        equippableAbilities = inventory[1];
        //equippableItems = inventory[2];

    }

    //Indikerar vilken equippable spelaren överväger att equippa
    public void HighlightNextEquippable(bool next)
    {
        if (next)
        {
            if (collectionIndex + 1 == inventory[displayCollection].Count)
            {
                collectionIndex = 0;
            }
            else
            {
                collectionIndex++;
            }
        }
        else
        {
            if (collectionIndex == 0)
            {
                collectionIndex = inventory[displayCollection].Count - 1;
            }
            else
            {
                collectionIndex--;
            }
        }
    }

    //Väljer vilka equippables som ska visas i inventorymenyn
    public void DisplayNextCollection(bool next)
    {
        if (next)
        {
            if (displayCollection + 1 == inventory.Length)
            {
                displayCollection = 0;
            }
            else
            {
                displayCollection++;
            }
        }
        else
        {
            if (displayCollection == 0)
            {
                displayCollection = inventory.Length - 1;
            }
            else
            {
                displayCollection--;
            }
        }
        //Byt meny
    }



    //Equippar ett föremål som finns i spelarens inventory
    public void Equip()
    {
        if (inventory[displayCollection] == null || collectionIndex > inventory[displayCollection].Count + 1 || inventory[displayCollection][collectionIndex] == null)
        {
            print("problem med inventory");
            return;
        }
        inventory[displayCollection][collectionIndex].Equip();
    }


    //Lägger till nya föremål i spelarens inventory
    public void AddEquippable(BaseEquippableScript equippable)
    {
        if (equippable is BaseWeaponScript)
        {
            inventory[0].Add(equippable);
        }
        else if (equippable is BaseAbilityScript)
        {
            inventory[1].Add(equippable);
        }
        else
        {
            print("trying to add nonspecified equippable, gör om gör rätt");
            return;
        }
    }

}