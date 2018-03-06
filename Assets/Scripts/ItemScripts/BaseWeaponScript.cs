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

public enum AttackMoveSets
{
    LightWeapon, HeavyWeapon
}

public class BaseWeaponScript : BaseEquippableObject
{
    [SerializeField]
    protected int damage;

    /*
    [SerializeField]
    protected DamageType damageType;
    */

    [SerializeField]
    protected float attackSpeed, repeatRate = 1.0f;
    
    [SerializeField]
    protected AttackMoves[] attacks;

    [SerializeField]
    AudioClip[] attackSounds, impactSounds;

    public AttackMoves[] Attacks
    {
        get { return this.attacks; }
    }

    protected IKillable equipper;

    public IKillable Equipper
    {
        set { if (this.equipper == null) this.equipper = value; }
    }

    //Change the players current weapon
    public override void Equip()
    {
        base.Equip();
        player.CurrentWeapon = this;
    }

    public float AttackSpeed
    {
        get { return this.attackSpeed; }
    }

    //Deals damage to an object with IKillable on it.
    public virtual void DealDamage(IKillable target)
    {
        target.TakeDamage(damage);
    }

    //When a weapon hits a killable target the script triggers and deals damage to target
    public void OnTriggerEnter(Collider other)
    {
        if ((equipper is BaseEnemyScript && (equipper as BaseEnemyScript).CurrentMovementType == MovementType.Attacking)
            || (equipper is PlayerControls && (equipper as PlayerControls).CurrentMovementType == MovementType.Attacking))
        {
            IKillable targetToHit = other.gameObject.GetComponent<IKillable>();

            if(equipper is BaseEnemyScript && targetToHit is BaseEnemyScript)
            {
                return;
            }

            if (targetToHit != null)
            {
                DealDamage(targetToHit);
            }
        }
    }
}