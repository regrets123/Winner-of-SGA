using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson && Andreas Nilsson*/

public enum DamageType
{
    Physical, Frost, Fire, Falling
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
    
    [SerializeField]
    protected float attackSpeed;

    [SerializeField]
    protected AttackMoves[] attacks;

    [SerializeField]
    protected DamageType dmgType;

    [SerializeField]
    protected AudioClip enemyHit1, enemyHit2, enemyHit3, swing1, swing2, thrust;

    protected bool canAttack = true;

    protected MovementType previousMovement;

    protected float currentSpeed;

    public float CurrentSpeed
    {
        get { return this.currentSpeed; }
        set { this.currentSpeed = value; StartCoroutine("ResetSpeed"); }
    }

    public bool CanAttack
    {
        get { return this.canAttack; }
    }

    public AttackMoves[] Attacks
    {
        get { return this.attacks; }
    }

    protected IKillable equipper;

    public IKillable Equipper
    {
        set { if (this.equipper == null) this.equipper = value; }
    }

    public IEnumerator AttackCooldown()
    {
        this.canAttack = false;
        if (equipper is PlayerControls)
        {
            previousMovement = player.CurrentMovementType;
            player.CurrentMovementType = MovementType.Attacking;
            yield return new WaitForSeconds(currentSpeed);
            player.CurrentMovementType = previousMovement;
        }
        else if (equipper is BaseEnemyScript)
        {
            previousMovement = (equipper as BaseEnemyScript).CurrentMovementType;
            (equipper as BaseEnemyScript).CurrentMovementType = MovementType.Attacking;
            yield return new WaitForSeconds(currentSpeed);
            (equipper as BaseEnemyScript).CurrentMovementType = previousMovement;
        }
        else
        {
            print("nu gick nåt åt helvete");
        }
            this.canAttack = true;
    }

    public float AttackSpeed
    {
        get { return this.attackSpeed; }
        //set { attackSpeed = AttackSpeed; }
    }

    //Deals damage to an object with IKillable on it.
    public virtual void DealDamage(IKillable target)
    {
        target.TakeDamage(damage, dmgType);
    }

    protected IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(1f);
        this.currentSpeed = attackSpeed;
    }

    protected override void Start()
    {
        base.Start();
        this.currentSpeed = attackSpeed;
        this.equipper = GetComponentInParent<IKillable>();
    }

    //When a weapon hits a killable target the script triggers and deals damage to target
    public void OnTriggerEnter(Collider other)
    {
        if ((equipper is BaseEnemyScript && (equipper as BaseEnemyScript).CurrentMovementType == MovementType.Attacking)
            || (equipper is PlayerControls && (equipper as PlayerControls).CurrentMovementType == MovementType.Attacking))
        {
            IKillable targetToHit = other.gameObject.GetComponent<IKillable>();

            if ((equipper is BaseEnemyScript && targetToHit is BaseEnemyScript) || (equipper is PlayerControls && targetToHit is PlayerControls))
            {
                return;
            }

            if (targetToHit != null)
            {
                DealDamage(targetToHit);
                ///SoundManager.instance.RandomizeSfx(enemyHit1, enemyHit2, enemyHit3);
            }
        }
    }
}