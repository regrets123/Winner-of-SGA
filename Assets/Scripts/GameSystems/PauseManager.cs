using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public interface IPausable
{

    void PauseMe(bool pausing);

}

public class PauseManager : MonoBehaviour
{

    [SerializeField]
    GameObject pauseMenu;

    AudioManager aM;

    InputManager iM;

    bool paused = false;

    static List<IPausable> pausables = new List<IPausable>();

    InputMode previousInputMode = InputMode.None;

    public List<IPausable> Pausables
    {
        get { return pausables; }
    }

    private void Start()
    {
        aM = GetComponent<AudioManager>();
        iM = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseAndUnpause();
        }
    }

    public void PauseAndUnpause()
    {
        paused = !paused;
        if (paused)
        {
            previousInputMode = iM.CurrentInputMode;
            iM.SetInputMode(InputMode.Paused);
        }
        else
        {
            iM.SetInputMode(previousInputMode);
        }
        pauseMenu.SetActive(paused);
        if (aM != null && aM.SoundGroups != null && aM.SoundGroups.Length > 0)
        {
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
        }
        foreach (IPausable pauseMe in pausables)
        {
            if (pauseMe != null)
                pauseMe.PauseMe(paused);
        }
    }
}
