using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*By Björn Andersson*/

public class BaseEnemyScript : MonoBehaviour, IKillable, IPausable
{

    [SerializeField]
    protected float aggroRange, attackRange;

    [SerializeField]
    protected int maxHealth, strength;

    [SerializeField]
    protected string unitName;

    [SerializeField]
    protected GameObject aggroCenter;

    [SerializeField]
    BaseWeaponScript weapon;

    protected int health;

    protected MovementType currentMovementType;

    public MovementType CurrentMovementType
    {
        get { return this.currentMovementType; }
    }

    protected Collider aggroCollider;

    protected PauseManager pM;

    protected PlayerControls target;

    protected SphereCollider aggroBubble;

    protected NavMeshAgent nav;

    protected virtual void Start()
    {
        this.nav = GetComponent<NavMeshAgent>();
        this.health = maxHealth;
        this.currentMovementType = MovementType.Idle;
        this.pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
        aggroBubble = aggroCenter.AddComponent<SphereCollider>();
        aggroBubble.isTrigger = true;
        aggroBubble.radius = aggroRange;
    }

    protected void Update()
    {
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < aggroRange)
            {
                Attack(Random.Range(0, weapon.Attacks.Length - 1));
            }
            else
            {
                nav.SetDestination(target.transform.position);
            }
        }
    }

    public void PauseMe(bool pausing)
    {
        nav.isStopped = !nav.isStopped;
        if (pausing)
        {

        }
        else
        {

        }
    }


    //Får fienden att anfalla spelaren när spelaren kommer tillräckligt nära
    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Aggro(other.gameObject.GetComponent<PlayerControls>());
        }
    }


    //
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
