using System.Collections;
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
    GameObject inventoryMenu;

    Image[] inventoryImages = new Image[3];

    GameObject[] inventoryArrows = new GameObject[4];

    Image equippedWeaponImage, equippedAbilityImage;

    List<GameObject> equippableWeapons, equippableAbilities/*, equippableItems*/;

    List<GameObject>[] inventory = new List<GameObject>[2];

    PlayerControls player;

    int displayCollection = 0, collectionIndex = 0;

    BaseEquippableObject currentChoice;

    InputManager inputManager;

    bool coolingDown = false;

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
        inputManager = FindObjectOfType<InputManager>();
        inventory[0] = new List<GameObject>();
        inventory[1] = new List<GameObject>();
        //inventory[2] = new List<BaseEquippableScript>();
        equippableWeapons = inventory[0];
        equippableAbilities = inventory[1];
        //equippableItems = inventory[2];
        inventoryMenu = GameObject.Find("InventoryMenu");
        equippedAbilityImage = GameObject.Find("EquippedAbilityImage").GetComponent<Image>();
        equippedWeaponImage = GameObject.Find("EquippedWeaponImage").GetComponent<Image>();
        for (int i = 0; i < inventoryImages.Length; i++)
        {
            inventoryImages[i] = GameObject.Find("InventoryImage" + i.ToString()).GetComponent<Image>();
        }
        for (int i = 0; i < inventoryArrows.Length; i++)
        {
            inventoryArrows[i] = GameObject.Find("Arrow" + i.ToString());
            inventoryArrows[i].SetActive(false);
        }
        inventoryMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (inventoryMenu.activeSelf)
            {
                //se till att rätt saker händer när rätt knappar trycks på
                Equip();
                HideInventory();
            }
            else
            {
                ShowInventory();
            }
        }
        else if (inventoryMenu.activeSelf && inputManager.CurrentInputMode == InputMode.Inventory)
        {
            if (!coolingDown)
            {
                if (Input.GetAxis("NextInventory") < 0f)
                {
                    DisplayNextCollection(false);
                }
                else if (Input.GetAxis("NextInventory") > 0f)
                {
                    DisplayNextCollection(true);
                }
                else if (Input.GetAxis("NextItem") < 0f)
                {
                    HighlightNextEquippable(false);
                }
                else if (Input.GetAxis("NextItem") > 0f)
                {
                    HighlightNextEquippable(true);
                }
            }
        }
    }

    IEnumerator MenuCooldown()
    {
        coolingDown = true;
        yield return new WaitForSeconds(0.5f);
        coolingDown = false;
    }

    void ShowInventory()
    {
        inventoryMenu.SetActive(true);
        inputManager.SetInputMode(InputMode.Inventory);
        for (int i = 0; i < inventoryImages.Length; i++)
        {
            inventoryImages[i].gameObject.SetActive(true);
        }
        UpdateSprites();
    }

    IEnumerator BlinkArrow(int arrowIndex)
    {
        inventoryArrows[arrowIndex].SetActive(true);
        yield return new WaitForSeconds(0.1f);
        inventoryArrows[arrowIndex].SetActive(true);
    }

    //Indikerar vilken equippable spelaren överväger att equippa
    void HighlightNextEquippable(bool next)
    {
        StartCoroutine("MenuCooldown");
        if (next)
        {
            //StartCoroutine("BlinkArrow");
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
        UpdateSprites();
    }

    public string[] ReportItems()
    {
        string[] items = new string[equippableAbilities.Count + equippableWeapons.Count];
        int index = 0;
        for (int i = 0; i < equippableAbilities.Count; i++)
        {
            print(equippableAbilities[i].GetComponent<BaseEquippableObject>().ObjectName);
            items[index] = equippableAbilities[index].GetComponent<BaseEquippableObject>().ObjectName;
            index++;
        }
        for (int i = 0; i < equippableWeapons.Count; i++)
        {
            print(equippableWeapons[i].GetComponent<BaseEquippableObject>().ObjectName);
            items[index] = equippableWeapons[i].GetComponent<BaseEquippableObject>().ObjectName;
            index++;
        }
        return items;
    }

    IEnumerator BlinkArrow(GameObject arrow)
    {
        arrow.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        arrow.SetActive(false);
    }

    //Väljer vilka equippables som ska visas i inventorymenyn
    void DisplayNextCollection(bool next)
    {
        StartCoroutine("MenuCooldown");
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
        UpdateSprites();
    }


    //Uppdaterar visuellt menyn av föremål och förmågor som spelaren kan välja mellan
    void UpdateSprites()
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
        if (displayCollection == 0)
        {
            equippedWeaponImage.sprite = inventory[displayCollection][collectionIndex].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        else
        {
            equippedAbilityImage.sprite = inventory[displayCollection][collectionIndex].GetComponent<BaseEquippableObject>().InventoryIcon;
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
        inputManager.SetInputMode(InputMode.Playing);
        inventoryMenu.SetActive(false);
    }

    //Lägger till nya föremål i spelarens inventory
    public void NewEquippable(GameObject equippable)
    {
        if (equippable.GetComponent<BaseEquippableObject>() is BaseWeaponScript)
        {
            AddEquippable(equippable, 0);
        }
        else if (equippable.GetComponent<BaseEquippableObject>() is BaseAbilityScript)
        {
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