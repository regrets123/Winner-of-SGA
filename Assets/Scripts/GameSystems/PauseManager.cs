using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public interface IPausable
{

    void PauseCoroutines();

}

public class PauseManager : MonoBehaviour
{

    [SerializeField]
    GameObject pauseMenu;

    AudioManager aM;

    bool paused = false;

    private void Start()
    {
        aM = GetComponent<AudioManager>();
    }

    public void PauseAndUnpause()
    {
        paused = !paused;
        pauseMenu.SetActive(paused);
        foreach (List<AudioSource> soundGroup in aM.SoundGroups)
        {
            foreach (AudioSource audio in soundGroup)
            {
                if (paused)
                {
                    audio.Pause();
                }
                else
                {
                    audio.UnPause();
                }
            }
        }
        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
