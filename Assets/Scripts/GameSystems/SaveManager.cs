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

    [SerializeField]
    GameObject camBase;

    XmlDocument currentGame;

    XPathNavigator xNav;

    PlayerControls player;

    private void Start()
    {
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
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").SetValue(player.transform.position.y.ToString()); //Funkar för att sätta värden
        print(xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Position/@Y").Value); //Funkar för att hitta värden
    }

    //Laddar ett sparat spel
    void LoadGame()
    {
        if (currentSave == null)
        {
            XmlDocument pathHolder = new XmlDocument();
            pathHolder.Load(Application.dataPath + "/SaveToLoad.xml");
            this.currentSave = new SavedGame(pathHolder.SelectSingleNode("SavedGame/@SpritePath").Value, pathHolder.SelectSingleNode("SavedGame/@SavePath").Value);
            File.Delete(Application.dataPath + "/SaveToLoad.xml");
        }
        this.currentGame.Load(currentSave.SavePath);
        MovePlayer();
        MoveCamera();
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

    public void SaveGame()
    {
        string spritePath = Screenshot();
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
                    // Save the document to a file and auto-indent the output.
                    savePath = Application.dataPath + "/SavedGame_" + saveNumber.ToString() + ".xml";
                    print(savePath);
                    XmlWriter writer = XmlWriter.Create(savePath, settings);
                    currentGame.Save(writer);
                    XmlDocument referenceXML = new XmlDocument();
                    TextAsset saveReference = Resources.Load("ReferenceXML") as TextAsset;
                    referenceXML.LoadXml(saveReference.text);
                    XPathNavigator refNav = referenceXML.CreateNavigator();
                    refNav.SelectSingleNode("/ReferenceXML/SavedGame/@SpritePath").SetValue(spritePath);
                    refNav.SelectSingleNode("/ReferenceXML/SavedGame/@SavePath").SetValue(savePath);
                    XmlWriter refWriter = XmlWriter.Create(Application.dataPath + "/SaveRef_" + saveNumber + ".xml", settings);
                    referenceXML.Save(refWriter);
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
        SaveInventory();
    }

    void SavePlayerResources()      //Sparar spelarens resurser till XML
    {
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@Stamina").SetValue(player.Stamina.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@HP").SetValue(player.Health.ToString());
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Resources/@LifeForce").SetValue(player.LifeForce.ToString());
    }

    void SaveInventory()        //Sparar alla föremål i spelarens inventory till XML
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
                    print(currentGame.InnerText);
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
        xNav.SelectSingleNode("/SavedState/PlayerInfo/Transform/Rotation/@W").SetValue(player.transform.rotation.w.ToString());
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
        print(path);
        UnityEngine.ScreenCapture.CaptureScreenshot(path);
        return path;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))  //Temporary
        {
            SaveGame();
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            LoadGame();
        }
    }
}