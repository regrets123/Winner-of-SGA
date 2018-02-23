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

    bool paused = false;

    static List<IPausable> pausables = new List<IPausable>();

    public List<IPausable> Pausables
    {
        get { return pausables; }
    }

    private void Start()
    {
        aM = GetComponent<AudioManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            print("pausing");
            PauseAndUnpause();
        }
    }

    public void PauseAndUnpause()
    {
        Cursor.visible = !Cursor.visible;
        paused = !paused;
        pauseMenu.SetActive(paused);
        if (aM != null && aM.SoundGroups != null && aM.SoundGroups.Length > 0)
        {
            /*
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
            */
        }
        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
        foreach (IPausable pauseMe in pausables)
        {
            pauseMe.PauseMe(paused);
        }
    }
}
