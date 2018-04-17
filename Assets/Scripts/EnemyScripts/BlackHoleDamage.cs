using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleDamage : MonoBehaviour
{
    [SerializeField]
    int blackHoleDamage;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerControls>().TakeDamage(blackHoleDamage, DamageType.Physical);
        }
    }
		
}
