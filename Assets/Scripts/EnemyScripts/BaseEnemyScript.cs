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
    protected BaseWeaponScript weapon;

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

    protected float lastAttack = 0f;

    bool invulnerable = false;

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
            if (Vector3.Distance(transform.position, target.transform.position) < aggroRange && Time.time > lastAttack + weapon.AttackSpeed)
            {
                Attack(/*Random.Range(0, weapon.Attacks.Length - 1)*/);
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

    protected void OnTriggerStay(Collider other)
    {
        if (target == null && other.gameObject.tag == "Player")
        {
            if (Physics.Linecast(transform.position, other.transform.position, 2))
                Aggro(other.gameObject.GetComponent<PlayerControls>());
        }
    }


    //Gör att fienden kan bli skadad
    public void TakeDamage(int incomingDamage)
    {
        if (invulnerable)
        {
            return;
        }

        int damage = ModifyDamage(incomingDamage);
        this.health -= damage;

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
        lastAttack = Time.time;
        this.currentMovementType = MovementType.Attacking;
        //trigga rätt attackanimation
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
