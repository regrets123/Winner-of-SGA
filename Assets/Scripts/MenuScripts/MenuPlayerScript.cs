using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class MenuPlayerScript : MonoBehaviour
{
    void Start()
    {
        GetComponent<Animator>().SetFloat("Speed", 0f);     //Ser till så att spelarmodellen i huvudmenyn endast gör sin idle animation
    }

}
