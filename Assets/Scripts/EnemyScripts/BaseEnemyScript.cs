using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class BaseEnemyScript : MonoBehaviour, IKillable {

    [SerializeField]
    protected float aggroRange;

    [SerializeField]
    protected int maxHealth, strength;

    [SerializeField]
    protected string unitName;

    protected int health;

    protected virtual void Start()
    {
        this.health = maxHealth;
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
