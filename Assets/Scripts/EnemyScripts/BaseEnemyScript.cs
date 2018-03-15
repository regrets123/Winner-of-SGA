using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*By Björn Andersson*/

public class BaseEnemyScript : MonoBehaviour, IKillable, IPausable
{

    [SerializeField]
    protected float aggroRange, attackRange, invulnerabilityTime;

    [SerializeField]
    protected int maxHealth, strength, lifeForce;

    [SerializeField]
    protected string unitName;

    [SerializeField]
    protected GameObject aggroCenter, soul;

    [SerializeField]
    protected GameObject weapon;

    [SerializeField]
    protected Transform weaponPos;

    protected int health;

    protected MovementType currentMovementType;

    public MovementType CurrentMovementType
    {
        get { return this.currentMovementType; }
        set { this.currentMovementType = value; }
    }

    protected Collider aggroCollider;

    protected PauseManager pM;

    protected PlayerControls target;

    protected SphereCollider aggroBubble;

    protected NavMeshAgent nav;

    protected Animator anim;

    protected bool invulnerable = false, alive = true;

    protected virtual void Start()
    {
        this.nav = GetComponent<NavMeshAgent>();
        this.health = maxHealth;
        this.currentMovementType = MovementType.Idle;
        this.pM = FindObjectOfType<PauseManager>();
        this.anim = GetComponentInChildren<Animator>();
        pM.Pausables.Add(this);
        aggroBubble = aggroCenter.AddComponent<SphereCollider>();
        aggroBubble.isTrigger = true;
        aggroBubble.radius = aggroRange;
        if (this.weapon != null)
        {
            this.weapon = Instantiate(weapon, weaponPos);
        }
    }

    protected void Update()
    {
        if (alive && target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < aggroRange && weapon.GetComponent<BaseWeaponScript>().CanAttack)
            {
                Attack(/*Random.Range(0, weapon.Attacks.Length - 1)*/);
            }
            else
            {
                nav.SetDestination(target.transform.position);
            }
            anim.SetFloat("Speed", nav.velocity.magnitude);
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

    /*
    protected void OnTriggerStay(Collider other)
    {
        if (target == null && other.gameObject.tag == "Player")
        {
            if (Physics.Linecast(transform.position, other.transform.position, 2))
                Aggro(other.gameObject.GetComponent<PlayerControls>());
        }
    }
    */

    //Gör att fienden kan bli skadad
    public void TakeDamage(int incomingDamage)
    {
        if (invulnerable)
        {
            return;
        }

        int damage = ModifyDamage(incomingDamage);
        this.health -= damage;

        print(health);

        if (this.health <= 0)
        {
            Death();
        }
        else
        {
            StartCoroutine("Invulnerability");
        }
    }

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerable = false;
    }

    //Låter fienden attackera
    public void Attack()
    {
        this.currentMovementType = MovementType.Attacking;
        anim.SetTrigger("Attack");
        weapon.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
    }

    //Modifierar skadan fienden tar efter armor, resistance och liknande
    protected virtual int ModifyDamage(int damage)
    {
        return damage;
    }

    protected virtual void Death()
    {
        //play death animation, destroy
        PlayerControls player = FindObjectOfType<PlayerControls>();
        if (player.Inventory.EquippableAbilities != null && player.Inventory.EquippableAbilities.Count > 0)
        {
            Instantiate(soul).GetComponent<LifeForceTransmitterScript>().StartMe(player, lifeForce);
        }
        Destroy(gameObject, 7);
    }

    public void Kill()
    {
        alive = false;
        anim.SetTrigger("Death");
        Death();
    }

    protected virtual void Aggro(PlayerControls newTarget)
    {
        this.target = newTarget;
    }
}
