using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*By Björn Andersson*/

public class BaseEnemyScript : MonoBehaviour, IKillable, IPausable
{

    [SerializeField]
    protected float aggroRange, attackRange, invulnerabilityTime, attackSpeed, loseAggroTime, loseAggroDistance, maxAggroFollow, maxPoise, staggerTime, poiseCooldown;

    [SerializeField]
    protected int maxHealth, lifeForce;

    [SerializeField]
    protected string unitName;

    [SerializeField]
    protected GameObject aggroCenter, soul;

    [SerializeField]
    protected GameObject weapon;

    [SerializeField]
    protected Transform weaponPos;

    [SerializeField]
    protected DamageType[] resistances;

    protected bool canAttack = true, burning = false, frozen = false;

    protected int health;

    protected float poiseReset, poise, timeToBurn = 0f;

    protected Collider aggroCollider;

    protected PauseManager pM;

    protected PlayerControls target;

    protected SphereCollider aggroBubble;

    protected NavMeshAgent nav;

    // protected Animator anim;

    protected bool invulnerable = false, alive = true, losingAggro = false;

    protected Vector3 initialPos;

    public bool Alive
    {
        get { return this.alive; }
    }

    protected MovementType currentMovementType, previousMovementType;

    public MovementType CurrentMovementType
    {
        get { return this.currentMovementType; }
        set { this.currentMovementType = value; }
    }

    protected virtual void Start()
    {
        this.nav = GetComponent<NavMeshAgent>();
        this.health = maxHealth;
        this.currentMovementType = MovementType.Idle;
        this.pM = FindObjectOfType<PauseManager>();
        //this.anim = GetComponentInChildren<Animator>();
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
            gameObject.transform.LookAt(target.transform);
            gameObject.transform.rotation = new Quaternion(0f, gameObject.transform.rotation.y, 0f, gameObject.transform.rotation.w);

            if (canAttack && Vector3.Distance(transform.position, target.transform.position) < aggroRange && weapon.GetComponent<BaseWeaponScript>().CanAttack && !target.Dead)
            {
                if (currentMovementType != MovementType.Stagger)
                {
                    Attack();
                }
            }
            else if (target.Dead)
            {
                LoseAggro();
                return;
            }
            else if (Vector3.Distance(transform.position, target.transform.position) > attackRange)
            {
                if (currentMovementType != MovementType.Stagger)
                {
                    nav.SetDestination(target.transform.position);
                }
            }
            if (!losingAggro && Vector3.Distance(transform.position, target.transform.position) > loseAggroDistance)
            {
                StartCoroutine("LoseAggroTimer");
            }
            if (initialPos != null && Vector3.Distance(transform.position, initialPos) > maxAggroFollow)
            {
                LoseAggro();
            }
        }
        //anim.SetFloat("Speed", nav.velocity.magnitude);

        if (poiseReset > 0)
        {
            poiseReset -= Time.deltaTime;
        }

        if (poiseReset <= 0)
        {
            poise = maxPoise;
        }
    }

    public void PauseMe(bool pausing)
    {
        if (!alive)
            return;

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
        if (alive && other.gameObject.tag == "Player")
        {
            if (target == null)
                Aggro(other.gameObject.GetComponent<PlayerControls>());
            else if (target == other.gameObject.GetComponent<PlayerControls>())
            {
                StopCoroutine("LoseAggroTimer");
            }
        }
    }

    //Gör att fienden kan bli skadad
    public void TakeDamage(int incomingDamage, DamageType dmgType)
    {
        if (!alive || invulnerable)
        {
            return;
        }

        int damage = ModifyDamage(incomingDamage, dmgType);
        this.health -= damage;

        poise -= incomingDamage;

        switch (dmgType)
        {
            case DamageType.Fire:
                StopCoroutine("Burn");
                StartCoroutine(Burn(5f, damage / 5));
                break;

            case DamageType.Frost:
                StopCoroutine("Freeze");
                StartCoroutine(Freeze(5f));
                break;

            case DamageType.Leech:
                FindObjectOfType<PlayerControls>().Leech(damage);
                break;
        }

        if (incomingDamage < health && poise < incomingDamage)
        {
            StartCoroutine("Stagger");
        }

        if (this.health <= 0)
        {
            Death();
        }
        else
        {
            StartCoroutine("Invulnerability");
        }
    }

    protected IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        invulnerable = false;
    }

    //Låter fienden attackera
    public void Attack()
    {
        if (!alive)
            return;
        this.currentMovementType = MovementType.Attacking;
        //anim.SetTrigger("Attack");
        weapon.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
        StartCoroutine("AttackCooldown");
    }

    protected IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

    protected IEnumerator LoseAggroTimer()
    {
        losingAggro = true;
        yield return new WaitForSeconds(loseAggroTime);
        LoseAggro();
    }

    protected void LoseAggro()
    {
        target.EnemyAggro(this, false);
        target = null;
        losingAggro = false;
        nav.SetDestination(initialPos);
    }

    //Modifierar skadan fienden tar efter armor, resistance och liknande
    protected virtual int ModifyDamage(int damage, DamageType dmgType)
    {
        foreach (DamageType resistance in this.resistances)
        {
            if (dmgType == resistance)
            {
                damage /= 2;
                break;
            }
        }
        return damage;
    }

    protected virtual void Death()
    {
        alive = false;
        //anim.SetTrigger("Death");
        this.target = null;
        nav.isStopped = true;
        PlayerControls player = FindObjectOfType<PlayerControls>();
        if (player.Inventory.EquippableAbilities != null && player.Inventory.EquippableAbilities.Count > 0)
        {
            Instantiate(soul, transform.position, Quaternion.identity).GetComponent<LifeForceTransmitterScript>().StartMe(player, lifeForce, this);
        }
        Destroy(gameObject, 7);
    }

    public void Kill()
    {
        alive = false;
        Death();
    }

    protected virtual void Aggro(PlayerControls newTarget)
    {
        this.initialPos = transform.position;
        this.target = newTarget;
        target.EnemyAggro(this, true);
    }

    protected IEnumerator Burn(float burnDuration, int burnDamage)
    {
        burning = true;
        timeToBurn += burnDuration;
        while (timeToBurn > 0f)
        {
            yield return new WaitForSeconds(0.5f);
            this.health -= burnDamage;
            timeToBurn -= Time.deltaTime;
        }
        timeToBurn = 0f;
        burning = false;
    }

    protected IEnumerator Freeze(float freezeTime)
    {
        if (!frozen)
        {
            frozen = true;
            float originalSpeed = nav.speed;
            nav.speed /= 2f;
            yield return new WaitForSeconds(freezeTime);
            nav.speed = originalSpeed;
            frozen = false;
        }
    }

    protected IEnumerator Stagger()
    {
        if (currentMovementType != MovementType.Stagger)
            previousMovementType = currentMovementType;
        currentMovementType = MovementType.Stagger;
        //anim.SetTrigger("Stagger");
        poiseReset = poiseCooldown;
        yield return new WaitForSeconds(staggerTime);
        currentMovementType = previousMovementType;
    }
}
