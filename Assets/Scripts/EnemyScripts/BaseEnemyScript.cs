using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class BaseEnemyScript : MonoBehaviour, IKillable, IPausable {

    [SerializeField]
    protected float aggroRange;

    [SerializeField]
    protected int maxHealth, strength;

    [SerializeField]
    protected string unitName;

    protected int health;

    MovementType currentMovement;

    Collider aggroCollider;

    PauseManager pM;

    protected virtual void Start()
    {
        this.health = maxHealth;
        this.currentMovement = MovementType.Idle;
        this.pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
    }

    public void PauseMe(bool pausing)
    {

    }

    public void TakeDamage(int incomingDamage)
    {
        int damage = ModifyDamage(incomingDamage);
        this.health -= damage;
        if (this.health <= 0)
        {
            Death();
        }
    }

    public void Attack(int attackMove)
    {
        this.currentMovement = MovementType.Attacking;
    }

    protected virtual int ModifyDamage(int damage)
    {
        return damage;
    }

    protected virtual void Death()
    {
        //play death animation, destroy
    }

    public void Kill()
    {
        Death();
    }

    protected virtual void Aggro()
    {

    }
}
