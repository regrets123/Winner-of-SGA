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

    public void PauseMe(bool pausing)
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

//    [SerializeField]
//    List<AudioSource> soundGroups;

//    [SerializeField]
//    AudioClip soundFX1, soundFX2, soundFX3;

//    AudioSource environmentalAudio;
//    AudioSource soundFX;
//    AudioSource music;


//    public void ChangeVolume(int soundGroup, float volume)
//    {
//        foreach (AudioSource audio in soundGroups)
//        {
//            audio.volume = volume;
//        }
//    }

//    public void PlayAudio()
//    {
//        soundFX.PlayOneShot(soundFX1);
//        soundFX.PlayOneShot(soundFX2);
//        soundFX.PlayOneShot(soundFX3);
//    }

//    public List<AudioSource> SoundGroups
//    {
//        get { return this.soundGroups; }
//    }

//    public AudioSource Music
//    {
//        get { return this.music; }
//    }

//}
}
