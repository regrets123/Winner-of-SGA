using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
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

    private void Start()
    {
        XmlDocument pathHolder = new XmlDocument();
        if (File.Exists(Application.persistentDataPath + @"\SaveToLoad.xml"))
        {
            pathHolder.LoadXml(Application.persistentDataPath + @"\SaveToLoad.xml");
            currentGame.LoadXml(pathHolder.SelectSingleNode("SavePath").Value);
            File.Delete(Application.persistentDataPath + @"\SaveToLoad.xml");
        }
        else
        {

        }
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
                if (!File.Exists(Application.persistentDataPath + @"\SavedGames\SavedGame" + saveNumber.ToString() + @".xml"))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    // Save the document to a file and auto-indent the output.
                    savePath = Application.persistentDataPath + @"\SavedGames\SavedGame" + saveNumber.ToString() + @".xml";
                    XmlWriter writer = XmlWriter.Create(savePath, settings);
                    currentGame.Save(writer);
                    break;
                }
                saveNumber++;
            }
            currentSave = new SavedGame(spritePath, savePath);
        }
        else
        {

        }
    }

    void GetInfoToSave()
    {
        //lagra all relevant info i currentGame
        
    }

    string Screenshot()
    {
        string path = Application.persistentDataPath;
        if (currentSave != null)
        {
            path = currentSave.SpritePath;
        }
        else
        {
            int saveNumber = 0;
            while (saveNumber < maxSaves)
            {
                if (!File.Exists(Application.persistentDataPath + @"\SaveSprites\SaveSprite" + saveNumber.ToString() + ".png"))
                {
                    path += @"\SaveSprites\SaveSprite" + saveNumber.ToString() + ".png";
                    break;
                }
                saveNumber++;
            }
        }
        UnityEngine.ScreenCapture.CaptureScreenshot(path);
        return Application.persistentDataPath + path;
    }
}