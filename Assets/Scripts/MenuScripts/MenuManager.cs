using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*By Johanna Pettersson*/

public class MenuManager : MonoBehaviour
{

    InventoryManager iM;

    [SerializeField]
    Color inactiveColor, activeColor;

    public void ToggleMenu(GameObject menuToToggle)
    {
        menuToToggle.SetActive(!menuToToggle.activeSelf);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }


    public void Glow(Outline o)
    {
        if (iM == null)
            iM = FindObjectOfType<InventoryManager>();
        if (iM.CurrentChoice != null)
            NoGlow(iM.CurrentChoice.GetComponent<Outline>());
        if (iM.CurrentUpgrade != null)
            NoGlow(iM.CurrentUpgrade.GetComponent<Outline>());
        o.enabled = true;
        if (iM.ItemSelected)
            return;
        if (iM.Upgrading)
        {
            iM.CurrentUpgrade = o.GetComponent<Button>();
            iM.UpgradeIndex = Array.IndexOf(iM.UpgradeButtons, o.GetComponent<Button>());
        }
        else
        {
            iM.CurrentChoice = o.GetComponent<Button>();
            iM.CollectionIndex = Array.IndexOf(iM.InventoryButtons, o.GetComponent<Button>());
        }
    }

    public void NoGlow(Outline o)
    {
        o.enabled = false;
    }

    public void ToggleColor(Text textToChange)
    {
        if (textToChange.color == activeColor)
        {
            textToChange.color = inactiveColor;
        }
        else
        {
            textToChange.color = activeColor;
        }
    }
    
}
