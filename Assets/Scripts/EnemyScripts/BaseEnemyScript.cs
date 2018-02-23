using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class BaseEnemyScript : MonoBehaviour, IKillable, IPausable
{

    [SerializeField]
    protected float aggroRange;

    [SerializeField]
    protected int maxHealth, strength;

    [SerializeField]
    protected string unitName;

    protected int health;

    MovementType currentMovementType;

    public MovementType CurrentMovementType
    {
        get { return this.currentMovementType; }
    }
    
    Collider aggroCollider;

    PauseManager pM;

    PlayerControls target;

    protected virtual void Start()
    {
        this.health = maxHealth;
        this.currentMovementType = MovementType.Idle;
        this.pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
    }

    public void PauseMe(bool pausing)
    {
        if (pausing)
        {

        }
        else
        {

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {

            Aggro(other.gameObject.GetComponent<PlayerControls>());
        }
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
        this.currentMovementType = MovementType.Attacking;
    }

    protected virtual int ModifyDamage(int damage)
    {
        return damage;
    }

    protected virtual void Death()
    {
        //play death animation, destroy
        Destroy(gameObject);
    }

    public void Kill()
    {
        Death();
    }

    protected virtual void Aggro(PlayerControls newTarget)
    {
        this.target = newTarget;

    }
}
