﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*By Björn Andersson*/

public enum EquipableType
{
    Weapon, Ability
}

public class InventoryManager : MonoBehaviour
{

    [SerializeField]
    GameObject inventoryMenu;

    List<GameObject> equippableWeapons, equippableAbilities/*, equippableItems*/;

    List<GameObject>[] inventory = new List<GameObject>[2];

    PlayerControls player;

    int displayCollection = 0, collectionIndex = 0;

    BaseEquippableObject currentChoice;

    [SerializeField]
    Image[] inventoryImages = new Image[3];

    public List<GameObject> EquippableAbilities
    {
        get { return this.equippableAbilities; }
    }

    public List<GameObject> EquippableWeapons
    {
        get { return this.equippableWeapons; }
    }

    private void Awake()
    {
        this.player = FindObjectOfType<PlayerControls>();
        inventory[0] = new List<GameObject>();
        inventory[1] = new List<GameObject>();
        //inventory[2] = new List<BaseEquippableScript>();
        equippableWeapons = inventory[0];
        equippableAbilities = inventory[1];
        //equippableItems = inventory[2];

    }

    void Update()
    {
        //if (inventoryMenu.activeSelf)
        //{
        //se till att rätt saker händer när rätt knappar trycks på
        if (Input.GetKeyDown("r"))
        {
            Equip();
        }
        //}
    }

    //Indikerar vilken equippable spelaren överväger att equippa
    void HighlightNextEquippable(bool next)
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
        UpdateButtons();
    }

    public string[] ReportItems()
    {
        string[] items = new string[equippableAbilities.Count + equippableWeapons.Count];
        int index = 0;
        for(int i = 0; i < equippableWeapons.Count; i++)
        {
            items[index] = equippableWeapons[index].GetComponent<BaseEquippableObject>().ObjectName;
            index++;
        }
        for (int i = 0; i < equippableAbilities.Count; i++)
        {
            items[index] = equippableAbilities[index].GetComponent<BaseEquippableObject>().ObjectName;
            index++;
        }
        return items;
    }

    //Väljer vilka equippables som ska visas i inventorymenyn
    void DisplayNextCollection(bool next)
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
        collectionIndex = 0;
        UpdateButtons();
    }


    //Uppdaterar visuellt menyn av föremål och förmågor som spelaren kan välja mellan
    void UpdateButtons()
    {
        inventoryImages[1].sprite = inventory[displayCollection][collectionIndex].GetComponent<BaseEquippableObject>().InventoryIcon;
        if (collectionIndex == 0)
        {
            inventoryImages[0].sprite = inventory[displayCollection][inventory[displayCollection].Count - 1].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        else
        {
            inventoryImages[0].sprite = inventory[displayCollection][collectionIndex - 1].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        if (collectionIndex + 1 == inventory[displayCollection].Count)
        {
            inventoryImages[2].sprite = inventory[displayCollection][0].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        else
        {
            inventoryImages[2].sprite = inventory[displayCollection][collectionIndex + 1].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
    }


    //Equippar ett föremål som finns i spelarens inventory
    void Equip()
    {
        if (inventory[displayCollection] == null || collectionIndex > inventory[displayCollection].Count - 1 || inventory[displayCollection][collectionIndex] == null)
        {
            Debug.Log("problem med inventory");
            return;
        }
        player.Equip(inventory[displayCollection][collectionIndex]);
    }

    //Gömmer inventoryt
    void HideInventory()
    {
        inventoryMenu.SetActive(false);
    }

    //Lägger till nya föremål i spelarens inventory
    public void NewEquippable(GameObject equippable)
    {
        if (equippable.GetComponent<BaseEquippableObject>() is BaseWeaponScript)
        {
            print("nytt vapen");
            AddEquippable(equippable, 0);
        }
        else if (equippable.GetComponent<BaseEquippableObject>() is BaseAbilityScript)
        {
            print("ny ability");
            AddEquippable(equippable, 1);
        }
        else
        {
            Debug.Log("trying to add nonspecified equippable, gör om gör rätt");
            return;
        }
    }

    //Lägger till equippable i rätt collection
    void AddEquippable(GameObject equippable, int collection)
    {
        inventory[collection].Add(equippable);
    }
}