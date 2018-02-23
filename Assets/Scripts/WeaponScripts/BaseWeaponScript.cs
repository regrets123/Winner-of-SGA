using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson & Andreas Nilsson*/

public enum DamageType
{
    Physical, Magical
}

public class BaseWeaponScript : MonoBehaviour {

    [SerializeField]
    protected int damage;

    [SerializeField]
    protected string weaponName;

    [SerializeField]
    protected DamageType damageType;

    [SerializeField]
    float attackSpeed, repeatRate = 1.0f;

    [SerializeField]
    protected IKillable targetToHit;

    public Animation[] attackMoves;

    public virtual void DealDamage(IKillable target)
    {
        target.TakeDamage(damage);
    }

    public void OnTriggerEnter(Collider other)
    {
        //targetToHit = other.gameObject.GetComponent<IKillable>();

        Debug.Log("Collider triggered");

        if (other.gameObject.GetComponent<IKillable>() != null)
        {
            //if(other.gameObject.tag == "Enemy")
            //{
            Debug.Log("You hit the target!");

            //InvokeRepeating("BaseWeaponScript", 0.5f, repeatRate);
            //gameObject.GetComponent<BoxCollider>().enabled = false;

            DealDamage(other.gameObject.GetComponent<IKillable>());

        }
    }
}