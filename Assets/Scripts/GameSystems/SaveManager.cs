using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.XPath;
using System.IO;

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

    XmlDocument currentGame;

    XPathNavigator xNav;

    PlayerControls player;

    private void Start()
    {
        currentGame = new XmlDocument();
        player = FindObjectOfType<PlayerControls>(); //Temporary
        XmlDocument pathHolder = new XmlDocument();
        if (File.Exists(Application.dataPath + "/SaveToLoad.xml"))
        {
            pathHolder.Load(Application.dataPath + "/SaveToLoad.xml");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pathHolder.SelectSingleNode("SavePath").Value);

            currentGame.Load(pathHolder.SelectSingleNode("SavePath").Value);
            File.Delete(Application.dataPath + "/SaveToLoad.xml");
            this.currentSave = new SavedGame(currentGame.SelectSingleNode("SpritePath").Value, currentGame.SelectSingleNode("SavePath").Value);
            //Ladda det sparade spelet
        }
        else
        {
            TextAsset newGame = Resources.Load("SavedStateXML") as TextAsset;
            currentGame.LoadXml(newGame.text);
        }
        xNav = currentGame.CreateNavigator();





        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").SetValue("9879"); //Funkar för att sätta värden
        print(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").Value); //Funkar för att hitta värden
    }

    void LoadGame()
    {

    }

    public void SaveGame()
    {
        string spritePath = Screenshot();
        string savePath = "";
        GetInfoToSave();   //Matar in all info som ska sparas i den virtuella XML-filen
        if (currentSave == null)
        {
            int saveNumber = 0;
            while (saveNumber < maxSaves)
            {
                if (!File.Exists(Application.dataPath + "/SavedGame" + saveNumber.ToString() + ".xml"))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    // Save the document to a file and auto-indent the output.
                    savePath = Application.dataPath + "/SavedGame" + saveNumber.ToString() + ".xml";
                    print(savePath);
                    XmlWriter writer = XmlWriter.Create(savePath, settings);
                    currentGame.Save(writer);
                    break;
                }
                saveNumber++;
            }
            currentSave = new SavedGame(spritePath, savePath);
            print(savePath + "      " + spritePath);
        }
        else
        {

        }
    }

    void GetInfoToSave()
    {
        //Lagra all relevant info i currentGame
        SavePlayerTransform();
        SavePlayerResources();
        SaveInventory();
    }

    void SavePlayerResources()
    {
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@Stamina").SetValue(player.Stamina.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@HP").SetValue(player.Health.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@LifeForce").SetValue(player.LifeForce.ToString());
    }

    void SaveInventory()
    {
        XPathNodeIterator nodes = xNav.Select("/PlayerInfo/Inventory//Item/@Name");
        XPathNavigator inventory = xNav.SelectSingleNode("//Inventory");
        foreach (string itemName in player.Inventory.ReportItems())
        {
            if (nodes.Count == 0)
            {
                inventory.AppendChild("<Item Name=\"" + itemName + "\"/>");
            }
            foreach (XPathNavigator node in nodes)
            {
                print("node");
                if (itemName == node.Value)
                {
                    print("hittat");
                    break;
                }
                else if (nodes.CurrentPosition == nodes.Count)
                {
                    inventory.AppendChild("<Item Name=\"" + itemName + "\"/>");
                    print(currentGame);
                }
            }
        }
    }

    void SavePlayerTransform() //Sparar spelarens transform i XML
    {
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@X").SetValue(player.transform.position.x.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").SetValue(player.transform.position.y.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Z").SetValue(player.transform.position.z.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@X").SetValue(player.transform.rotation.x.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@Y").SetValue(player.transform.rotation.y.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@Z").SetValue(player.transform.rotation.z.ToString());
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
                if (!File.Exists(Application.dataPath + "/SaveSprite" + saveNumber.ToString() + ".png"))
                {
                    path += "/SaveSprite" + saveNumber.ToString() + ".png";
                    break;
                }
                saveNumber++;
            }
        }
        UnityEngine.ScreenCapture.CaptureScreenshot(path);
        return path;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SaveGame();
        }
    }
}