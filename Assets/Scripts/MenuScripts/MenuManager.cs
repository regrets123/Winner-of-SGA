using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*By Johanna Pettersson*/

public class MenuManager : MonoBehaviour {

    InventoryManager iM;

    public void ToggleMenu(GameObject menuToToggle)
    {
        menuToToggle.SetActive(!menuToToggle.activeSelf);
        print("Toggle");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        print("Load Scene");
    }

    public void ExitApplication()
    {
        Application.Quit();
        print("Quit");
    }

    public void Glow(Outline o)
    {
        o.enabled = true;
        if (iM == null)
            iM = FindObjectOfType<InventoryManager>();
        iM.CurrentChoice = o.GetComponent<Button>();
    }

    public void NoGlow(Outline o)
    {
        o.enabled = false;
    }
}
