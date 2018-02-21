using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class SaveManager : MonoBehaviour {
    
    public void SaveGame()
    {
        Screenshot();
    }

    void Screenshot()
    {
        UnityEngine.ScreenCapture.CaptureScreenshot(Application.persistentDataPath + @"\prutt.png");
    }
}