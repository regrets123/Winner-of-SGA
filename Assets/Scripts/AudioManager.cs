using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class AudioManager : MonoBehaviour {

    [SerializeField]
    List<AudioSource>[] soundGroups = new List<AudioSource>[2];

    List<AudioSource> environmentalAudio;
    List<AudioSource> soundFX;
    AudioSource music;

    public void ChangeVolume(int soundGroup, float volume)
    {
        foreach(AudioSource audio in soundGroups[soundGroup])
        {
            audio.volume = volume;
        }
    }

    public List<AudioSource>[] SoundGroups
    {
        get { return this.soundGroups; }
    }

    public AudioSource Music
    {
        get { return this.music; }
    }
    
}
