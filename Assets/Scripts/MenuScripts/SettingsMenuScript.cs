using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/*By Johanna Pettersson*/

public class SettingsMenuScript : MonoBehaviour {

    public AudioMixer mainMixer;

    public Slider brightnessSlider;

    void Start()
    {
        RenderSettings.ambientLight = new Color(brightnessSlider.value, brightnessSlider.value, brightnessSlider.value, 1);
    }

    public void SetMusicVolume(float musicVolume)
    {
        mainMixer.SetFloat("Music", musicVolume);
    }

    public void SetEnvironmentalVolume(float environmentalVolume)
    {
        mainMixer.SetFloat("Environmental", environmentalVolume);
    }

    public void SetSFXVolume(float SFXVolume)
    {
        mainMixer.SetFloat("SFX", SFXVolume);
    }

    public void ApplySettings()
    {

    }

    public void SetBrightness()
    {
        RenderSettings.ambientLight = new Color(brightnessSlider.value, brightnessSlider.value, brightnessSlider.value, 1);
        
    }
}
