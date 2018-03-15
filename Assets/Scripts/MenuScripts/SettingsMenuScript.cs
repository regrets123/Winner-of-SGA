using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Xml;
using System.Xml.XPath;
using System.IO;

/*By Johanna Pettersson*/

public class SettingsMenuScript : MonoBehaviour
{
    [SerializeField]
    AudioMixer mainMixer;

    [SerializeField]
    Slider brightnessSlider;

    XmlDocument settingsXML;

    XPathNavigator xNav;

    float musicVolume, SFXVolume, environmentalVolume;

    void Start()
    {
        if (File.Exists(Application.dataPath + "Settings.xml"))
        {
            settingsXML = new XmlDocument();
            print("settings finns");
            settingsXML.Load(Application.dataPath + "Settings.xml");
            xNav = settingsXML.CreateNavigator();
            SetMusicVolume(float.Parse(xNav.SelectSingleNode("/Settings/Volumes/@Music").Value));
            SetEnvironmentalVolume(float.Parse(xNav.SelectSingleNode("/Settings/Volumes/@Environmental").Value));
            SetSFXVolume(float.Parse(xNav.SelectSingleNode("/Settings/Volumes/@FX").Value));
            brightnessSlider.value = float.Parse(xNav.SelectSingleNode("/Settings/Visual/@Gamma").Value);
        }
        else
        {
            TextAsset newSettings = Resources.Load("SettingsXML") as TextAsset;
            settingsXML = new XmlDocument();
            settingsXML.LoadXml(newSettings.text);
            xNav = settingsXML.CreateNavigator();
        }
        RenderSettings.ambientLight = new Color(brightnessSlider.value, brightnessSlider.value, brightnessSlider.value, 1);
    }

    public void SetMusicVolume(float musicVolume)
    {
        mainMixer.SetFloat("Music", musicVolume);
        this.musicVolume = musicVolume;
    }

    public void SetEnvironmentalVolume(float environmentalVolume)
    {
        mainMixer.SetFloat("Environmental", environmentalVolume);
        this.environmentalVolume = environmentalVolume;
    }

    public void SetSFXVolume(float SFXVolume)
    {
        mainMixer.SetFloat("SFX", SFXVolume);
        this.SFXVolume = SFXVolume;
    }

    public void ApplySettings()
    {

    }

    public void SaveSettings()
    {
        if (xNav == null)
        {
            print("bajsmacka");
        }
        xNav.SelectSingleNode("/Settings/Volumes/@Music").SetValue(musicVolume.ToString());
        xNav.SelectSingleNode("/Settings/Volumes/@Environmental").SetValue(environmentalVolume.ToString());
        xNav.SelectSingleNode("/Settings/Volumes/@FX").SetValue(SFXVolume.ToString());
        xNav.SelectSingleNode("/Settings/Visual/@Gamma").SetValue(brightnessSlider.value.ToString());
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Application.dataPath + "Settings.xml", writerSettings);
        settingsXML.Save(writer);
    }

    public void SetBrightness()
    {
        RenderSettings.ambientLight = new Color(brightnessSlider.value, brightnessSlider.value, brightnessSlider.value, 1);

    }
}
