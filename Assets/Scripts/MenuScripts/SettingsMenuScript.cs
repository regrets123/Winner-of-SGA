using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Xml;
using System.Xml.XPath;
using System.IO;

/*By Johanna Pettersson and Björn Andersson*/

public class SettingsMenuScript : MonoBehaviour
{
    [SerializeField]
    AudioMixer mainMixer;

    [SerializeField]
    Slider brightnessSlider, environmentalSlider, musicSlider, fxSlider, sensitivitySlider;

    [SerializeField]
    GameObject cam, settingsMenu;

    CameraFollow camFollow;

    XmlDocument settingsXML;

    XPathNavigator xNav;

    float musicVolume, SFXVolume, environmentalVolume, camSensitivity, startingEnvironmental, startingMusic, startingFX, startingBrightness, startingSense;

    void Awake()
    {
        camFollow = cam.GetComponent<CameraFollow>();
        if (sensitivitySlider == null)
        {
            this.sensitivitySlider = GameObject.Find("SensitivitySlider").GetComponent<Slider>();
        }

        if (File.Exists(Application.dataPath + "/Settings.xml"))
        {
            settingsXML = new XmlDocument();
            print("settings finns");
            settingsXML.Load(Application.dataPath + "/Settings.xml");
            xNav = settingsXML.CreateNavigator();
            SetMusicVolume(float.Parse(xNav.SelectSingleNode("/Settings/Volumes/@Music").Value));
            startingMusic = musicVolume;
            musicSlider.value = musicVolume;
            SetEnvironmentalVolume(float.Parse(xNav.SelectSingleNode("/Settings/Volumes/@Environmental").Value));
            startingEnvironmental = environmentalVolume;
            environmentalSlider.value = environmentalVolume;
            SetSFXVolume(float.Parse(xNav.SelectSingleNode("/Settings/Volumes/@FX").Value));
            startingFX = SFXVolume;
            fxSlider.value = SFXVolume;
            brightnessSlider.value = float.Parse(xNav.SelectSingleNode("/Settings/Visual/@Gamma").Value);
            startingBrightness = brightnessSlider.value;
            sensitivitySlider.value = float.Parse(xNav.SelectSingleNode("/Settings/Camera/@Sensitivity").Value);
            camSensitivity = sensitivitySlider.value;
            print(sensitivitySlider.value);
            print(camSensitivity);
            startingSense = sensitivitySlider.value;
        }
        else
        {
            TextAsset newSettings = Resources.Load("SettingsXML") as TextAsset;
            settingsXML = new XmlDocument();
            settingsXML.LoadXml(newSettings.text);
            xNav = settingsXML.CreateNavigator();
        }
        RenderSettings.ambientLight = new Color(brightnessSlider.value, brightnessSlider.value, brightnessSlider.value, 1);
        print(this.sensitivitySlider);
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

    public void SetCamSensitivity()
    {
       /* if(this.sensitivitySlider == null)
        {
            this.sensitivitySlider = GameObject.Find("SensitivitySlider").GetComponent<Slider>();
        }*/
        this.camSensitivity = sensitivitySlider.value;
        if (camFollow != null)
            camFollow.InputSensitivity = this.camSensitivity;
    }

    public void SetSFXVolume(float SFXVolume)
    {
        mainMixer.SetFloat("SFX", SFXVolume);
        this.SFXVolume = SFXVolume;
    }

    public void ApplySettings()
    {
        if (xNav == null)
        {
            print("bajsmacka");
        }
        startingSense = sensitivitySlider.value;
        startingMusic = musicVolume;
        startingFX = SFXVolume;
        startingEnvironmental = environmentalVolume;
        startingBrightness = brightnessSlider.value;
        xNav.SelectSingleNode("/Settings/Volumes/@Music").SetValue(musicVolume.ToString());
        xNav.SelectSingleNode("/Settings/Volumes/@Environmental").SetValue(environmentalVolume.ToString());
        xNav.SelectSingleNode("/Settings/Volumes/@FX").SetValue(SFXVolume.ToString());
        xNav.SelectSingleNode("/Settings/Visual/@Gamma").SetValue(brightnessSlider.value.ToString());
        xNav.SelectSingleNode("/Settings/Camera/@Sensitivity").SetValue(sensitivitySlider.value.ToString());
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Application.dataPath + "/Settings.xml", writerSettings);
        settingsXML.Save(writer);
    }

    public void GoBack()
    {
        brightnessSlider.value = startingBrightness;
        SetBrightness();
        SetSFXVolume(startingFX);
        SetMusicVolume(startingMusic);
        SetEnvironmentalVolume(startingEnvironmental);
        sensitivitySlider.value = startingSense;
        SetCamSensitivity();
        settingsMenu.SetActive(false);
    }

    public void SetBrightness()
    {
        RenderSettings.ambientLight = new Color(brightnessSlider.value, brightnessSlider.value, brightnessSlider.value, 1);
    }
}
