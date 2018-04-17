using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using UnityEngine.SceneManagement;

/*By Björn Andersson*/

public class SavedGame  //Referensklass som håller koll på ett sparat spels och dess menysprites sökvägar
{
    string spritePath, savePath;

    public string SpritePath
    {
        get { return this.spritePath; }
    }

    public string SavePath
    {
        get { return this.savePath; }
    }

    public SavedGame(string spritePath, string savePath)
    {
        this.spritePath = spritePath;
        this.savePath = savePath;
    }
}

public class SaveManager : MonoBehaviour
{
    SavedGame currentSave;

    [SerializeField]
    int maxSaves;

    [SerializeField]
    GameObject camBase;

    [SerializeField]
    GameObject[] allItems;

    [SerializeField]
    Material saveMat;

    [SerializeField]
    GameObject[] savePoints;

    List<int> usedSavePoints = new List<int>();

    XmlDocument currentGame;

    XPathNavigator xNav;

    PlayerControls player;

    InputManager inputManager;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        currentGame = new XmlDocument();
        player = FindObjectOfType<PlayerControls>(); //Temporary
        if (File.Exists(Application.dataPath + "/SaveToLoad.xml"))
        {
            LoadGame();
        }
        else
        {
            TextAsset newGameText = Resources.Load("SavedStateXML") as TextAsset;
            currentGame.LoadXml(newGameText.text);
        }
        if (xNav == null)
            xNav = currentGame.CreateNavigator();



        /*Exempel för att hitta i XML*/
        // xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").SetValue(player.transform.position.y.ToString()); //Funkar för att sätta värden
        //  print(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").Value); //Funkar för att hitta värden
    }

    //Laddar ett sparat spel
    void LoadGame()
    {
        print("loading game");
        if (currentSave == null)
        {
            XmlDocument pathHolder = new XmlDocument();
            pathHolder.Load(Application.dataPath + "/SaveToLoad.xml");
            this.currentSave = new SavedGame(pathHolder.SelectSingleNode("/ReferenceXML/SavedGame/@SpritePath").Value, pathHolder.SelectSingleNode("/ReferenceXML/SavedGame/@SavePath").Value);
            File.Delete(Application.dataPath + "/SaveToLoad.xml");
        }
        this.currentGame.Load(currentSave.SavePath);
        this.xNav = currentGame.CreateNavigator();
        MovePlayer();
        MoveCamera();
        LoadInventory();
        ReskinSavePoints();
    }

    void ReskinSavePoints()
    {
        XPathNodeIterator nodes = xNav.Select("/SavedState/UsedSavePoints//SavePoint/@Index");
        foreach (XPathNavigator node in nodes)
        {
            savePoints[int.Parse(node.Value)].GetComponent<SavePointScript>().Reskin(saveMat);
        }
    }

    void LoadInventory()
    {
        XPathNodeIterator nodes = xNav.Select("/SavedState/PlayerInfo/Inventory//Item/@Name");
        XPathNodeIterator upgrades = xNav.Select("/SavedState/PlayerInfo/Inventory/Upgrades//Upgrade");
        XPathNodeIterator favorites = xNav.Select("/SavedState/PlayerInfo/Inventory/Favorites//Favorite/@Name");
        foreach (XPathNavigator node in nodes)
        {
            foreach (GameObject item in allItems)
            {
                if (node.Value == item.GetComponent<BaseEquippableObject>().ObjectName)
                {
                    GameObject newItem = Instantiate(item);
                    player.Inventory.NewEquippable(newItem);
                    if (newItem.GetComponent<BaseEquippableObject>() is BaseWeaponScript)
                        while (upgrades.MoveNext())
                        {
                            XPathNavigator thisUpgrade = upgrades.Current.CreateNavigator();
                            string weaponToUpgrade = thisUpgrade.GetAttribute("Weapon", "");
                            if (newItem.GetComponent<BaseWeaponScript>().ObjectName == weaponToUpgrade)
                            {
                                player.Inventory.SetWeaponUpgrade(weaponToUpgrade, thisUpgrade.GetAttribute("Name", ""), int.Parse(thisUpgrade.GetAttribute("Level", "")));
                                break;
                            }
                        }
                    while (favorites.MoveNext())
                    {
                        if (favorites.Current.Value == newItem.GetComponent<BaseEquippableObject>().ObjectName)
                        {
                            player.Inventory.AddFavorite(newItem);
                            break;
                        }
                    }
                    //Destroy(newItem);
                }
            }
        }
    }

    public void ReloadGame()
    {
        if (currentSave != null)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            TextAsset xmlText = Resources.Load("ReferenceXML") as TextAsset;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlText.text);
            XPathNavigator saveNav = doc.CreateNavigator();
            using (XmlWriter writer = XmlWriter.Create(Application.dataPath + "/SaveToLoad.xml", settings))
            {
                saveNav.SelectSingleNode("/ReferenceXML/SavedGame/@SpritePath").SetValue(currentSave.SpritePath);
                saveNav.SelectSingleNode("/ReferenceXML/SavedGame/@SavePath").SetValue(currentSave.SavePath);
                doc.Save(writer);
            }
        }
        inputManager.SetInputMode(InputMode.Playing);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Master_Scene");
    }

    void MovePlayer()
    {
        Vector3 newPos = new Vector3(float.Parse(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@X").Value), float.Parse(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").Value), float.Parse(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Z").Value));
        Quaternion newRot = new Quaternion(float.Parse(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@X").Value), float.Parse(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@Y").Value), float.Parse(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@Z").Value), float.Parse(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@W").Value));
        player.transform.position = newPos;
        player.transform.rotation = newRot;
    }

    void MoveCamera()
    {
        Vector3 newPos = new Vector3(float.Parse(xNav.SelectSingleNode("/SavedState/CameraTransform/Position/@X").Value), float.Parse(xNav.SelectSingleNode("/SavedState/CameraTransform/Position/@Y").Value), float.Parse(xNav.SelectSingleNode("/SavedState/CameraTransform/Position/@Z").Value));
        Quaternion newRot = new Quaternion(float.Parse(xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@X").Value), float.Parse(xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@Y").Value), float.Parse(xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@Z").Value), float.Parse(xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@W").Value));
        camBase.transform.position = newPos;
        camBase.transform.rotation = newRot;
    }

    public void SaveGame(GameObject savePoint)
    {
        string spritePath = Screenshot();
        usedSavePoints.Add(Array.IndexOf(savePoints, savePoint) + 1);
        savePoint.GetComponent<SavePointScript>().Reskin(saveMat);
        GetInfoToSave();   //Matar in all info som ska sparas i den virtuella XML-filen
        if (currentSave == null)
        {
            print("ingen currentSave");
            string savePath = "";
            int saveNumber = 0;
            while (saveNumber < maxSaves)
            {
                if (!File.Exists(Application.dataPath + "/SavedGame_" + saveNumber.ToString() + ".xml"))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.CloseOutput = true;
                    // Save the document to a file and auto-indent the output.
                    savePath = Application.dataPath + "/SavedGame_" + saveNumber.ToString() + ".xml";
                    print(savePath);
                    using (XmlWriter writer = XmlWriter.Create(savePath, settings))
                    {
                        currentGame.Save(writer);
                    }
                    XmlDocument referenceXML = new XmlDocument();
                    TextAsset saveReference = Resources.Load("ReferenceXML") as TextAsset;
                    referenceXML.LoadXml(saveReference.text);
                    XPathNavigator refNav = referenceXML.CreateNavigator();
                    refNav.SelectSingleNode("/ReferenceXML/SavedGame/@SpritePath").SetValue(spritePath);
                    refNav.SelectSingleNode("/ReferenceXML/SavedGame/@SavePath").SetValue(savePath);
                    using (XmlWriter refWriter = XmlWriter.Create(Application.dataPath + "/SaveRef_" + saveNumber + ".xml", settings))
                    {
                        referenceXML.Save(refWriter);
                    }
                    break;
                }
                saveNumber++;
            }
            currentSave = new SavedGame(spritePath, savePath);
            print(savePath + "      " + spritePath);
        }
        else
        {
            currentGame.Save(currentSave.SavePath);
        }
    }

    void GetInfoToSave()
    {
        //Lagrar all relevant info i currentGame
        SavePlayerTransform();
        SavePlayerResources();
        SaveCamTransform();
        SaveInventory();
        SaveUsedSavePoints();
    }

    void SaveUsedSavePoints()
    {
        XPathNavigator savePointsNode = xNav.SelectSingleNode("/SavedState/UsedSavePoints");
        XmlNodeList oldSavePoints = currentGame.SelectNodes("//SavePoint");
        if (oldSavePoints.Count > 0)
            for (int i = oldSavePoints.Count - 1; i > -1; i--)
            {
                oldSavePoints[i].ParentNode.RemoveChild(oldSavePoints[i]);
            }
        foreach (int index in usedSavePoints)
        {
            savePointsNode.AppendChild("<SavePoint Index=\"" + index + "\"/>");
        }
    }

    void SavePlayerResources()      //Sparar spelarens resurser till XML
    {
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@Stamina").SetValue(player.Stamina.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@HP").SetValue(player.Health.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@LifeForce").SetValue(player.LifeForce.ToString());
    }

    void SaveCamTransform()
    {
        xNav.SelectSingleNode("/SavedState/CameraTransform/Position/@X").SetValue(Math.Round(camBase.transform.position.x, 4).ToString());
        xNav.SelectSingleNode("/SavedState/CameraTransform/Position/@Y").SetValue(Math.Round(camBase.transform.position.y, 4).ToString());
        xNav.SelectSingleNode("/SavedState/CameraTransform/Position/@Z").SetValue(Math.Round(camBase.transform.position.z, 4).ToString());
        xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@X").SetValue(Math.Round(camBase.transform.rotation.x, 4).ToString());
        xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@Y").SetValue(Math.Round(camBase.transform.rotation.y, 4).ToString());
        xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@Z").SetValue(Math.Round(camBase.transform.rotation.z, 4).ToString());
        xNav.SelectSingleNode("/SavedState/CameraTransform/Rotation/@W").SetValue(Math.Round(camBase.transform.rotation.w, 4).ToString());
    }

    void SaveInventory()        //Sparar alla föremål i spelarens inventory till XML
    {
        XPathNodeIterator nodes = xNav.Select("/SavedState/PlayerInfo/Inventory//Item/@Name");
        XPathNavigator inventory = xNav.SelectSingleNode("//Inventory");
        foreach (string itemName in player.Inventory.ReportItems())
        {
            if (nodes.Count == 0)
            {
                inventory.AppendChild("<Item Name=\"" + itemName + "\"/>");
                continue;
            }
            foreach (XPathNavigator node in nodes)
            {
                if (itemName == node.Value)
                {
                    break;
                }
                else if (nodes.CurrentPosition == nodes.Count)
                {
                    inventory.AppendChild("<Item Name=\"" + itemName + "\"/>");
                }
            }
        }
        XPathNavigator favoritesNode = xNav.SelectSingleNode("//Favorites");
        XPathNodeIterator oldFavorites = favoritesNode.SelectChildren(XPathNodeType.All);
        XmlNodeList allOldFavorites = currentGame.SelectNodes("//Favorite");
        if (oldFavorites.Count > 0)
        {
            for (int i = allOldFavorites.Count - 1; i > -1; i--)
            {
                allOldFavorites[i].ParentNode.RemoveChild(allOldFavorites[i]);
            }
        }
        foreach (string favName in player.Inventory.ReportFavorites())
        {
            favoritesNode.AppendChild("<Favorite Name =\"" + favName + "\"/>");
        }
        nodes = xNav.Select("/SavedState/PlayerInfo/Inventory//Item/@Name");
        string[] weaponNames = player.Inventory.ReportWeaponNames();
        XPathNavigator upgradesNode = xNav.SelectSingleNode("//Upgrades");
        XPathNodeIterator oldUpgrades = upgradesNode.SelectChildren(XPathNodeType.All);
        XmlNodeList allOldUpgrades = currentGame.SelectNodes("//Upgrade");
        if (oldUpgrades.Count > 0)
        {
            for (int i = allOldUpgrades.Count - 1; i > -1; i--)
            {
                allOldUpgrades[i].ParentNode.RemoveChild(allOldUpgrades[i]);
            }
        }
        upgradesNode = xNav.SelectSingleNode("//Upgrades");
        string[][] newUpgrades = player.Inventory.ReportWeaponUpgrades();
        int index = 0;
        foreach (string[] upgradeInfo in newUpgrades)
        {
            char upgradeLevel = upgradeInfo[1][upgradeInfo[1].Length - 1];
            string newName = upgradeInfo[1].Remove(upgradeInfo[1].Length - 1, 1);
            for (int i = index; i < weaponNames.Length; i++)
            {
                if (weaponNames[index] == newUpgrades[index][0])
                {
                    index++;
                    print(newName + " " + upgradeLevel);
                    upgradesNode.AppendChild("<Upgrade Weapon=\"" + newUpgrades[i][0] + "\" Name=\"" + newName + "\" Level=\"" + upgradeLevel + "\"/>");
                    break;
                }
            }
        }
    }

    void SavePlayerTransform() //Sparar spelarens transform i XML
    {
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@X").SetValue(Math.Round(player.transform.position.x, 4).ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").SetValue(Math.Round(player.transform.position.y, 4).ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Z").SetValue(Math.Round(player.transform.position.z, 4).ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@X").SetValue(Math.Round(player.transform.rotation.x, 4).ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@Y").SetValue(Math.Round(player.transform.rotation.y, 4).ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@Z").SetValue(Math.Round(player.transform.rotation.z, 4).ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@W").SetValue(Math.Round(player.transform.rotation.w, 4).ToString());
    }

    string Screenshot() //Tar ett screenshot som sedan används som en sprite för sparfilen i loadmenyn
    {
        string path = Application.dataPath;
        if (currentSave != null)
        {
            path = currentSave.SpritePath;
        }
        else
        {
            int saveNumber = 0;
            while (saveNumber < maxSaves)
            {
                if (!File.Exists(Application.dataPath + "/SaveSprite_" + saveNumber.ToString() + ".png"))
                {
                    path += "/SaveSprite_" + saveNumber.ToString() + ".png";
                    break;
                }
                saveNumber++;
            }
        }
        ScreenCapture.CaptureScreenshot(path);
        return path;
    }
}