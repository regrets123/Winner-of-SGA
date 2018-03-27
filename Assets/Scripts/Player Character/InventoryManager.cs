using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*By Björn Andersson*/

public enum EquipableType
{
    Weapon, Ability, Consumable, ItemUpgrade
}

public class InventoryManager : MonoBehaviour
{
    GameObject inventoryMenu;

    GameObject[] inventoryArrows = new GameObject[4];

    Image equippedWeaponImage, equippedAbilityImage, currentEquipableImage, currentUpgradeImage;

    List<GameObject> equippableWeapons, equippableAbilities, consumables, itemUpgrades;

    List<GameObject>[] inventory = new List<GameObject>[4];

    PlayerControls player;

    InputManager inputManager;

    Button[] inventoryButtons = new Button[12];

    Button[] upgradeButtons = new Button[8];

    PauseManager pM;

    int displayCollection = 0;

    Sprite defaultIcon;

    Button currentChoice;

    MenuManager menuManager;

    Text equippableName, upgradeName, upgradeInfo;




    bool coolingDown = false;




    public List<GameObject> EquippableAbilities
    {
        get { return this.equippableAbilities; }
    }

    public List<GameObject> EquippableWeapons
    {
        get { return this.equippableWeapons; }
    }

    public List<GameObject> Consumable
    {
        get { return this.consumables; }
    }

    public List<GameObject> ItemUpgrades
    {
        get { return this.itemUpgrades; }
    }

    public Button CurrentChoice
    {
        set { this.currentChoice = value; }
    }

    private void Awake()
    {
        menuManager = FindObjectOfType<MenuManager>();
        defaultIcon = Resources.Load("EmptySlot") as Sprite;
        this.player = FindObjectOfType<PlayerControls>();
        pM = FindObjectOfType<PauseManager>();
        inputManager = FindObjectOfType<InputManager>();
        inventory[0] = new List<GameObject>();
        inventory[1] = new List<GameObject>();
        inventory[2] = new List<GameObject>();
        inventory[3] = new List<GameObject>();
        equippableWeapons = inventory[0];
        equippableAbilities = inventory[1];
        consumables = inventory[2];
        itemUpgrades = inventory[2];
        inventoryMenu = GameObject.Find("InventoryMenu");
        equippedAbilityImage = GameObject.Find("EquippedAbilityImage").GetComponent<Image>();
        equippedWeaponImage = GameObject.Find("EquippedWeaponImage").GetComponent<Image>();
        currentEquipableImage = GameObject.Find("Equipable Image").GetComponent<Image>();
        currentUpgradeImage = GameObject.Find("CurrentUpgradeImage").GetComponent<Image>();
        equippableName = GameObject.Find("Equipable Name").GetComponent<Text>();
        upgradeName = GameObject.Find("UpgradeName").GetComponent<Text>();
        upgradeInfo = GameObject.Find("UpgradeInfo").GetComponent<Text>();
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            inventoryButtons[i] = GameObject.Find("Slot " + (i + 1).ToString()).GetComponent<Button>();
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
                HideInventory();
            }
            else
            {
                ShowInventory();
            }
        }
        else if (inventoryMenu.activeSelf && inputManager.CurrentInputMode == InputMode.Inventory && !coolingDown)
        {
            /*
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
            else if (Input.GetButtonDown("Jump"))
            {
                Equip();
                HideInventory();
            }
            */
        }
        else if ((Input.GetAxisRaw("NextInventory") < 0f || Input.GetAxisRaw("NextItem") < 0f || Input.GetKeyDown("r")) && !inventoryMenu.activeSelf && inputManager.CurrentInputMode == InputMode.Playing && !coolingDown && player.CurrentWeapon != null)
        {
            /*
            print("tja");
            StartCoroutine("MenuCooldown");
            //player.Anim.SetBool("WeaponDrawn", false);
            player.Equip(null);
            */
        }
        if (Input.GetKeyDown("r"))
        {
            Equip(0);
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
        pM.PauseAndUnpause();
        UpdateSprites();
        menuManager.Glow(currentChoice.GetComponent<Outline>());
    }

    IEnumerator BlinkArrow(int arrowIndex)
    {
        inventoryArrows[arrowIndex].SetActive(true);
        yield return new WaitForSeconds(0.2f);
        inventoryArrows[arrowIndex].SetActive(false);
    }



    //Indikerar vilken equippable spelaren överväger att equippa
    void HighlightNextEquippable(bool next)
    {
        StartCoroutine("MenuCooldown");
        /*
        if (next)
        {
            StartCoroutine(BlinkArrow(0));
        }
        else
        {
            StartCoroutine(BlinkArrow(1));
        }
        UpdateSprites();
        */
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

    public void DisplayNewCollection(int displayCollection)
    {
        this.displayCollection = displayCollection;
        UpdateSprites();
    }

    //Uppdaterar visuellt menyn av föremål och förmågor som spelaren kan välja mellan
    void UpdateSprites()
    {
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            if (inventory[displayCollection][i] != null)
            {
                inventoryButtons[i].image.sprite = inventory[displayCollection][i].GetComponent<BaseEquippableObject>().InventoryIcon;
            }
            else
            {
                inventoryButtons[i].image.sprite = defaultIcon;
            }
        }
        /*
        inventoryButtons[1].sprite = inventory[displayCollection][collectionIndex].GetComponent<BaseEquippableObject>().InventoryIcon;
        if (collectionIndex == 0)
        {
            inventoryButtons[0].sprite = inventory[displayCollection][inventory[displayCollection].Count - 1].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        else
        {
            inventoryButtons[0].sprite = inventory[displayCollection][collectionIndex - 1].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        if (collectionIndex + 1 == inventory[displayCollection].Count)
        {
            inventoryButtons[2].sprite = inventory[displayCollection][0].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        else
        {
            inventoryButtons[2].sprite = inventory[displayCollection][collectionIndex + 1].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        if (displayCollection == 0)
        {
            equippedWeaponImage.sprite = inventory[displayCollection][collectionIndex].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        else
        {
            equippedAbilityImage.sprite = inventory[displayCollection][collectionIndex].GetComponent<BaseEquippableObject>().InventoryIcon;
        }
        */
    }

    public void SelectItem(int index)
    {
        if (inventory[displayCollection] == null || index >= inventory[displayCollection].Count || inventory[displayCollection] == null)
            return;
        equippableName.text = inventory[displayCollection][index].GetComponent<BaseEquippableObject>().ObjectName;
        currentEquipableImage.sprite = inventory[displayCollection][index].GetComponent<BaseEquippableObject>().InventoryIcon;
    }


    //Equippar ett föremål som finns i spelarens inventory
    public void Equip(int collectionIndex)
    {
        if (inventory[displayCollection] == null || collectionIndex > inventory[displayCollection].Count - 1 || inventory[displayCollection][collectionIndex] == null)
        {
            Debug.Log("Not able to equip");
            return;
        }
        /*
        if (displayCollection == 0 && player.CurrentWeapon != null)
        {
            player.Equip(inventory[displayCollection][collectionIndex]);
        }
        else
        */
        HideInventory();
        player.Equip(inventory[displayCollection][collectionIndex]);
    }

    //Gömmer inventoryt
    void HideInventory()
    {
        pM.PauseAndUnpause();
        inventoryMenu.SetActive(false);
    }

    //Lägger till nya föremål i spelarens inventory
    public void NewEquippable(GameObject equippable)
    {
        switch (equippable.GetComponent<BaseEquippableObject>().MyType)
        {
            case EquipableType.Weapon:
                AddEquippable(equippable, 0);
                break;

            case EquipableType.Ability:
                AddEquippable(equippable, 1);
                break;

            case EquipableType.Consumable:
                AddEquippable(equippable, 2);
                break;

            case EquipableType.ItemUpgrade:
                AddEquippable(equippable, 3);
                break;

            default:
                Debug.Log("trying to add nonspecified equippable, gör om gör rätt");
                break;
        }
    }

    //Lägger till equippable i rätt collection
    void AddEquippable(GameObject equippable, int collection)
    {
        inventory[collection].Add(equippable);
    }
}