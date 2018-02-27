using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson && Andreas Nilsson*/

public enum DamageType
{
    Physical, Magical
}

public enum AttackMoves
{
    QuickAttack, StrongAttack, Sideswipe, PiercingAttack
}


public class BaseWeaponScript : BaseEquippableScript
{
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

    [SerializeField]
    AttackMoves[] attacks;

    public AttackMoves[] Attacks
    {
        get { return this.attacks; }
    }

    public Animation[] attackMoves;

    IKillable equipper;

    //Script for switching between weapons
    public IKillable Equipper
    {
        set { if (this.equipper == null) this.equipper = value; }
    }


    public virtual void DealDamage(IKillable target)
    {
        target.TakeDamage(damage);
    }

    //When a weapon hits a killable target the script triggers and deals damage to target
    public void OnTriggerEnter(Collider other)
    {
        if (equipper is BaseEnemyScript && (equipper as BaseEnemyScript).CurrentMovementType == MovementType.Attacking)
        {
            targetToHit = other.gameObject.GetComponent<IKillable>();

            if (targetToHit != null)
            {
                DealDamage(targetToHit);

            }

        }
    }
}