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
    GameObject inventoryMenu, upgradeOptions, equipButton, upgradeButton, favoriteButton, applyUpgradeButton;

    Image equippedWeaponImage, equippedAbilityImage, currentEquipableImage, currentUpgradeImage;

    List<GameObject> equippableWeapons, equippableAbilities, consumables, favoriteItems, itemUpgrades;

    List<GameObject>[] inventory = new List<GameObject>[4];

    PlayerControls player;

    InputManager inputManager;

    Button[] inventoryButtons = new Button[12];

    Button[] categoryButtons = new Button[4];

    Button[] upgradeButtons = new Button[8];

    PauseManager pM;

    int displayCollection = 0, collectionIndex = 0, upgradeIndex = 0;

    Sprite defaultIcon;

    Button currentChoice, currentCategory, currentUpgrade;

    MenuManager menuManager;

    Text equippableName, upgradeName, upgradeInfo;

    bool coolingDown = false, itemSelected = false, upgrading = false;

    public GameObject InventoryMenu
    {
        get { return this.inventoryMenu; }
    }

    public List<GameObject> EquippableAbilities
    {
        get { return this.equippableAbilities; }
    }

    public Button[] InventoryButtons
    {
        get { return this.inventoryButtons; }
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
        get { return this.currentChoice; }
        set { this.currentChoice = value; }
    }

    public int CollectionIndex
    {
        set { this.collectionIndex = value; }
    }

    private void Awake()
    {
        menuManager = FindObjectOfType<MenuManager>();
        defaultIcon = Resources.Load<Sprite>("EmptySlot");
        this.player = FindObjectOfType<PlayerControls>();
        pM = FindObjectOfType<PauseManager>();
        inputManager = FindObjectOfType<InputManager>();
        inventory[0] = new List<GameObject>();
        inventory[1] = new List<GameObject>();
        inventory[2] = new List<GameObject>();
        inventory[3] = new List<GameObject>();
        itemUpgrades = new List<GameObject>();
        equippableWeapons = inventory[0];
        equippableAbilities = inventory[1];
        consumables = inventory[2];
        favoriteItems = inventory[3];
        inventoryMenu = GameObject.Find("InventoryMenu");
        upgradeOptions = GameObject.Find("UpgradeOptions");
        upgradeButton = GameObject.Find("UpgradeButton");
        equipButton = GameObject.Find("EquipButton");
        favoriteButton = GameObject.Find("FavoriteButton");
        applyUpgradeButton = GameObject.Find("ApplyUpgradeButton");
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
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            upgradeButtons[i] = GameObject.Find("UpgradeSlot " + (i + 1).ToString()).GetComponent<Button>();
        }
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            categoryButtons[i] = GameObject.Find("Category " + (i + 1).ToString()).GetComponent<Button>();
        }
        currentCategory = categoryButtons[0];
        currentCategory.GetComponent<Outline>().enabled = true;
        upgradeOptions.SetActive(false);
        inventoryMenu.SetActive(false);
        upgradeButton.SetActive(false);
        favoriteButton.SetActive(false);
        equipButton.SetActive(false);
        applyUpgradeButton.SetActive(false);
    }

    void Update() //se till att rätt saker händer när rätt knappar trycks på               
    {
        if (Input.GetButtonDown("Inventory") && inputManager.CurrentInputMode != InputMode.Paused)
        {
            if (inventoryMenu.activeSelf)
            {
                HideInventory();
            }
            else
            {
                ShowInventory();
            }
        }
        else if (inventoryMenu.activeSelf && inputManager.CurrentInputMode == InputMode.Inventory && !coolingDown && !upgrading)
        {
            if (!itemSelected)
            {
                if (Input.GetAxis("NextInventoryRow") < 0f)
                {
                    ChangeInventoryRow(true);
                }
                else if (Input.GetAxis("NextInventoryRow") > 0f)
                {
                    ChangeInventoryRow(false);
                }
                else if (Input.GetAxisRaw("NextItem") < 0f)
                {
                    HighlightNextItem(false);
                }
                else if (Input.GetAxisRaw("NextItem") > 0f)
                {
                    HighlightNextItem(true);
                }
                if (Input.GetButtonDown("PreviousInventoryCategory"))
                {
                    if (displayCollection == 0)
                    {
                        DisplayNewCollection(inventory.Length - 1);
                    }
                    else
                        DisplayNewCollection(displayCollection - 1);
                }
                else if (Input.GetButtonDown("NextInventoryCategory"))
                {
                    DisplayNewCollection((displayCollection + 1) % inventory.Length);
                }
            }
            if (Input.GetButtonDown("SelectItem"))
            {
                //SelectItem(collectionIndex);
                currentChoice.onClick.Invoke();
            }
            else if (itemSelected)
            {
                if (Input.GetButtonDown("Favorite"))
                {
                    AddFavorite();
                }
                else if (Input.GetButtonDown("Upgrade"))
                {
                    UpgradeWeapon(true);
                }
            }
        }
        else if (upgrading && !coolingDown)
        {
            if (Input.GetAxisRaw("NextItem") > 0f)
            {
                StartCoroutine("MenuCooldown");
                upgradeIndex = (upgradeIndex + 1) % upgradeButtons.Length;
            }
            else if (Input.GetAxisRaw("NextItem") < 0f)
            {
                StartCoroutine("MenuCooldown");
                if (upgradeIndex == 0)
                {
                    upgradeIndex = upgradeButtons.Length - 1;
                }
                else
                {
                    upgradeIndex--;
                }
            }
            else if (Input.GetAxisRaw("NextInventoryRow") != 0f)
            {
                StartCoroutine("MenuCooldown");
                int nextUpgradeIndex = 2;
                if (Input.GetAxisRaw("NextInventoryRow") > 0f)
                {
                    nextUpgradeIndex *= -1;
                }
                if (upgradeIndex + nextUpgradeIndex < 0)
                {
                    upgradeIndex = (upgradeButtons.Length) + (upgradeIndex + nextUpgradeIndex);
                }
                else if (upgradeIndex + nextUpgradeIndex > upgradeButtons.Length - 1)
                {
                    upgradeIndex = (upgradeIndex + nextUpgradeIndex) - (upgradeButtons.Length);
                }
                else
                {
                    upgradeIndex += nextUpgradeIndex;
                }
            }
            currentUpgrade = upgradeButtons[upgradeIndex];
            if (Input.GetButtonDown("GoBack"))
            {
                ShowUpgradeOptions(false);
            }
            UpdateSprites();
        }
        else if (!inventoryMenu.activeSelf && inputManager.CurrentInputMode == InputMode.Playing && !coolingDown)
        {
            if (Input.GetAxisRaw("NextInventoryRow") > 0f)
            {

            }
            else if (Input.GetAxisRaw("NextInventoryRow") < 0f)
            {

            }
            else if (Input.GetAxisRaw("NextItem") > 0f)
            {

            }
            else if (Input.GetAxisRaw("NextItem") < 0f)
            {

            }
            /*
            //player.Anim.SetBool("WeaponDrawn", false);
            player.Equip(null);
            */
        }
        if (Input.GetKeyDown("r"))
        {
            player.Equip(null);
        }
    }

    void EquipFavorite(int favoriteIndex)
    {
        if (inventory[3] == null || inventory[3].Count <= favoriteIndex || inventory[3][favoriteIndex] == null)
            return;
        player.Equip(inventory[3][favoriteIndex]);
        StartCoroutine("DisplayEquippedFavorite");
    }

    IEnumerator DisplayEquippedFavorite(int favoriteIndex)
    {
        yield return null; //Lerpa bild in & ut
    }

    void UpgradeWeapon(bool upgrading)
    {
        this.upgrading = upgrading;
        upgradeOptions.SetActive(upgrading);
        if (upgrading)
        {
            currentUpgrade = upgradeButtons[0];
        }
        else
        {
            currentChoice = inventoryButtons[collectionIndex];
        }
    }

    void ChangeInventoryRow(bool next)
    {
        StartCoroutine("MenuCooldown");
        int nextRow = 4;
        if (!next)
        {
            nextRow *= -1;
        }
        if (collectionIndex + nextRow < 0)
        {
            collectionIndex = inventoryButtons.Length + (collectionIndex + nextRow);
        }
        else if (collectionIndex + nextRow >= inventoryButtons.Length)
        {
            collectionIndex = (collectionIndex + nextRow) - inventoryButtons.Length;
        }
        else
        {
            collectionIndex += nextRow;
        }
        menuManager.Glow(inventoryButtons[collectionIndex].GetComponent<Outline>());
    }

    void HighlightNextItem(bool next)
    {
        StartCoroutine("MenuCooldown");
        if (next)
            collectionIndex = (collectionIndex + 1) % inventoryButtons.Length;
        else
        {
            if (collectionIndex == 0)
                collectionIndex = inventoryButtons.Length - 1;
            else
                collectionIndex--;
        }
        menuManager.Glow(inventoryButtons[collectionIndex].GetComponent<Outline>());
    }

    void EquipFavorite(int index, bool controllerInput)
    {
        player.Equip(inventory[3][index]);
        if (controllerInput)
            StartCoroutine("HighlightControllerInput");
    }

    IEnumerator HighlightControllerInput()
    {
        yield return null;
    }

    IEnumerator MenuCooldown()
    {
        coolingDown = true;
        yield return new WaitForSecondsRealtime(0.15f);
        coolingDown = false;
    }

    void ShowInventory()
    {
        if (currentChoice == null)
        {
            currentChoice = inventoryButtons[0];
        }
        inventoryMenu.SetActive(true);
        equippableName.text = "";
        upgradeInfo.text = "";
        upgradeName.text = "";
        currentEquipableImage.sprite = defaultIcon;
        currentUpgradeImage.sprite = defaultIcon;
        pM.PauseAndUnpause(true);
        UpdateSprites();
        menuManager.Glow(currentChoice.GetComponent<Outline>());
    }

    public string[] ReportItems()
    {
        string[] items = new string[equippableAbilities.Count + equippableWeapons.Count];
        int index = 0;
        for (int i = 0; i < equippableWeapons.Count; i++)
        {
            items[index] = equippableWeapons[i].GetComponent<BaseEquippableObject>().ObjectName;
            index++;
        }
        for (int i = 0; i < equippableAbilities.Count; i++)
        {
            items[index] = equippableAbilities[index].GetComponent<BaseEquippableObject>().ObjectName;
            index++;
        }
        for (int i = 0; i < consumables.Count; i++)
        {
            items[index] = consumables[i].GetComponent<BaseEquippableObject>().ObjectName;
            index++;
        }
        return items;
    }

    public string[] ReportWeaponNames()
    {
        string[] weaponNames = new string[equippableWeapons.Count];
        for (int i = 0; i < equippableWeapons.Count; i++)
        {
            weaponNames[i] = equippableWeapons[i].GetComponent<BaseEquippableObject>().ObjectName;
        }
        return weaponNames;
    }

    public string[][] ReportWeaponUpgrades()
    {
        string[][] weaponUpgrades = new string[equippableWeapons.Count][];
        for (int i = 0; i < equippableWeapons.Count; i++)
        {
            weaponUpgrades[i] = new string[2];
            weaponUpgrades[i][0] = equippableWeapons[i].GetComponent<BaseEquippableObject>().ObjectName;
            weaponUpgrades[i][1] = equippableWeapons[i].GetComponent<BaseWeaponScript>().CurrentUpgrade.ToString() + equippableWeapons[i].GetComponent<BaseWeaponScript>().UpgradeLevel;
        }
        return weaponUpgrades;
    }

    public void DisplayNewCollection(int displayCollection)
    {
        this.displayCollection = displayCollection;
        currentCategory.GetComponent<Outline>().enabled = false;
        currentCategory = categoryButtons[displayCollection];
        currentCategory.GetComponent<Outline>().enabled = true;
        if (itemSelected)
        {
            itemSelected = false;
            ShowItemOptions(false);
        }
        UpdateSprites();
    }

    void ShowItemOptions(bool show)
    {
        //upgradeOptions.SetActive(show);
        if (!show || inventory[displayCollection][collectionIndex].GetComponent<BaseEquippableObject>() is BaseWeaponScript)
        {
            upgradeButton.SetActive(show);
        }
        equipButton.SetActive(show);
        favoriteButton.SetActive(show);
    }

    //Uppdaterar visuellt menyn av föremål och förmågor som spelaren kan välja mellan
    void UpdateSprites()
    {
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            if (i < inventory[displayCollection].Count && inventory[displayCollection][i] != null)
            {
                inventoryButtons[i].image.sprite = inventory[displayCollection][i].GetComponent<BaseEquippableObject>().InventoryIcon;
            }
            else
            {
                inventoryButtons[i].image.sprite = defaultIcon;
            }
        }
        if (upgradeOptions.activeSelf)
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                if (itemUpgrades.Count > i && itemUpgrades[i] != null)
                {
                    upgradeButtons[i].image.sprite = itemUpgrades[i].GetComponent<BaseEquippableObject>().InventoryIcon;
                }
                else
                {
                    upgradeButtons[i].image.sprite = defaultIcon;
                }
            }
            menuManager.Glow(currentUpgrade.GetComponent<Outline>());
        }
    }

    public void ShowUpgradeOptions(bool show)
    {
        itemSelected = show;
        upgradeOptions.SetActive(show);
        menuManager.NoGlow(currentChoice.GetComponent<Outline>());
        if (show)
        {
            currentUpgrade = upgradeButtons[0];
            menuManager.Glow(currentUpgrade.GetComponent<Outline>());
        }
        else
        {
            currentChoice = inventoryButtons[0];
            menuManager.Glow(currentChoice.GetComponent<Outline>());
        }
        UpdateSprites();
    }

    public void SelectItem(int index)
    {
        if (inventory[displayCollection] == null || index >= inventory[displayCollection].Count || inventory[displayCollection] == null || itemSelected)
            return;
        collectionIndex = index;
        itemSelected = true;
        ShowItemOptions(true);
        equippableName.text = inventory[displayCollection][index].GetComponent<BaseEquippableObject>().ObjectName;
        currentEquipableImage.sprite = inventory[displayCollection][index].GetComponent<BaseEquippableObject>().InventoryIcon;
    }

    public void ApplyUpgrade()
    {
        inventory[0][collectionIndex].GetComponent<BaseWeaponScript>().ApplyUpgrade(itemUpgrades[upgradeIndex].GetComponent<UpgradeScript>().MyUpgrade);
        itemUpgrades.Remove(itemUpgrades[upgradeIndex]);
        UpdateSprites();
    }

    void ClickCurrentButton()
    {
        currentChoice.onClick.Invoke();
    }

    public void SelectUpgrade(int upgradeIndex)
    {
        this.upgradeIndex = upgradeIndex;
        equipButton.SetActive(false);
        favoriteButton.SetActive(false);
        applyUpgradeButton.SetActive(true);
    }

    //Equippar ett föremål som finns i spelarens inventory
    public void Equip()
    {
        if (inventory[displayCollection] == null || collectionIndex > inventory[displayCollection].Count - 1 || inventory[displayCollection][collectionIndex] == null)
        {
            print(collectionIndex);
            Debug.Log("Not able to equip");
            return;
        }
        player.Equip(inventory[displayCollection][collectionIndex]);
    }

    public void AddFavorite()
    {
        if (favoriteItems != null && favoriteItems.Count < 4)
        {
            foreach (GameObject favorite in favoriteItems)
            {
                if (favorite == inventory[displayCollection][collectionIndex])
                    return;
            }
            inventory[3].Add(inventory[displayCollection][collectionIndex]);
        }
    }

    void AddUpgrade(GameObject newUpgrade)
    {
        itemUpgrades.Add(newUpgrade);
    }

    //Gömmer inventoryt
    public void HideInventory()
    {
        favoriteButton.SetActive(false);
        applyUpgradeButton.SetActive(false);
        ShowItemOptions(false);
        currentEquipableImage.sprite = defaultIcon;
        if (upgradeOptions.activeSelf)
        {
            upgradeOptions.SetActive(false);
        }
        pM.PauseAndUnpause(false);
        ShowUpgradeOptions(false);
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
                AddUpgrade(equippable);
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