using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class AudioManager : MonoBehaviour, IPausable {

    [SerializeField]
    List<AudioSource>[] soundGroups;

    List<AudioSource> environmentalAudio, soundFX;
    AudioSource music;
    PauseManager pM;

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

    void Start()
    {
        pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
    }

    public void PauseMe(bool pausing)       //Pausar och återupptar ljuduppspelning
    {       


        foreach (List<AudioSource> soundGroup in SoundGroups)
        {
            foreach (AudioSource audio in soundGroup)
            {
                if (pausing)
                {
                    audio.Pause();
                }
                else
                {
                    audio.UnPause();
                }
            }
        }
    }
}
