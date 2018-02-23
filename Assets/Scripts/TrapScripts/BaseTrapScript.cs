using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson*/

public class BaseTrapScript : MonoBehaviour {

    public GameObject trapObj;
    public Transform trapObjPos;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject, 1);
            Instantiate(trapObj, trapObjPos.position, trapObjPos.rotation);
        }
    }
}
