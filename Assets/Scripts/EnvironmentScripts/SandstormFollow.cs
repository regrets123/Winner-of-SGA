using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandstormFollow : MonoBehaviour
{
    [SerializeField]
    GameObject effectFollowObj;
    [SerializeField]
    float effectFollowSpeed;
	
	// Update is called once per frame
	void LateUpdate ()
    {
        Transform target = effectFollowObj.transform;

        float step = effectFollowSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
