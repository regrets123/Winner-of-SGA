using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson && Andreas Nilsson*/

public enum DamageType
{
    Physical, Frost, Fire, Falling, Leech
}

public enum AttackMoves
{
    QuickAttack, StrongAttack, Sideswipe, PiercingAttack
}

public enum AttackMoveSets
{
    LightWeapon, HeavyWeapon
}

public enum Upgrade
{
    None, DamageUpgrade, FireUpgrade, FrostUpgrade, LeechUpgrade
}

public class BaseWeaponScript : BaseEquippableObject
{
    [SerializeField]
    protected int origninalDamage;

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

    protected Upgrade currentUpgrade = Upgrade.None;

    protected float currentSpeed;

    protected int upgradeLevel = 0;

    public int UpgradeLevel
    {
        get { return this.upgradeLevel; }
    }

    public Upgrade CurrentUpgrade
    {
        get { return this.currentUpgrade; }
    }

    public float CurrentSpeed
    {
        get { return this.currentSpeed; }
        set { this.currentSpeed = value; StartCoroutine("ResetSpeed"); }
    }

    public bool CanAttack
    {
        get { return this.canAttack; }
    }

    public float AttackSpeed
    {
        get { return this.attackSpeed; }
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

    protected override void Start()
    {
        base.Start();
        this.currentSpeed = attackSpeed;
        this.damage = origninalDamage;
        print("weaponstart");
        this.equipper = GetComponentInParent<IKillable>();
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

    public void ApplyUpgrade(Upgrade upgrade)
    {
        print(this.dmgType);
        print(this.damage);
        this.currentUpgrade = upgrade;
        if (upgrade == Upgrade.DamageUpgrade && upgradeLevel < 3)
        {
            upgradeLevel++;
            this.damage += damage / 2;
        }
        else
        {
            switch (upgrade)
            {
                case Upgrade.FireUpgrade:
                    this.dmgType = DamageType.Fire;
                    break;

                case Upgrade.FrostUpgrade:
                    this.dmgType = DamageType.Frost;
                    break;

                case Upgrade.LeechUpgrade:
                    this.dmgType = DamageType.Leech;
                    break;

                default:
                    print("Nu blev nåt fel här");
                    break;
            }
            this.damage = origninalDamage;
            upgradeLevel = 0;
        }
        print("upgrade applied!");
        print(this.dmgType);
        print(this.damage);
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
                SoundManager.instance.RandomizeSfx(enemyHit1, enemyHit2, enemyHit3);
            }
        }
    }
}