using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

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

    public virtual void DealDamage(IKillable target)
    {
        target.TakeDamage(damage);
    }
}