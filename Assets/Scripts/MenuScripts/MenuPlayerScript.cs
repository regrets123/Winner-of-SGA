using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerScript : MonoBehaviour
{
    
    // Use this for initialization
    void Start()
    {
        GetComponent<Animator>().SetFloat("Speed", 0f);
    }

}
