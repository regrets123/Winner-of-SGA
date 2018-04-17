using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.transform.position = Vector3.Lerp(other.transform.position, transform.position, 0.025f);
        }
    }

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        other.transform.position = other.transform.position;
    //    }
    //}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
