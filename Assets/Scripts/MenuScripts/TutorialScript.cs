using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour {
    
    public GameObject textTutorial;

    void OnTriggerEnter()
    {
        StartCoroutine(TextCountdown());
    }

    public IEnumerator TextCountdown()
    {
        textTutorial.SetActive(true);

        yield return new WaitForSeconds(5);

        textTutorial.SetActive(false);
        Destroy(this.gameObject);
    }
}
