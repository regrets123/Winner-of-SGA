using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*By Andreas Nilsson && Björn Andersson*/


//Interface som används av spelaren och alla fiender samt eventuella förstörbara objekt
public interface IKillable
{
    void Attack();
    void TakeDamage(int damage);
    void Kill();
}

public enum MovementType
{
    Idle, Walking, Sprinting, Attacking, Dodging, Dashing, Jumping, Running, Interacting, SuperJumping, Stagger
}

public class PlayerControls : MonoBehaviour, IKillable, IPausable
{
    #region Serialized Variables

    [Header("Player Stats")]

    [Space(5)]

    [SerializeField]
    int maxHealth;

    [SerializeField]
    float maxStamina;

    [SerializeField]
    float maxLifeForce;

    [SerializeField]
    float maxPoise;

    [SerializeField]
    float staminaRegen;

    [SerializeField]
    float invulnerablityTime;

    [SerializeField]
    float staggerTime;

    [SerializeField]
    float poiseCooldown;

    [Space(10)]

    [Header("Player Movement")]

    [Space(5)]

    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float sprintSpeed;

    [SerializeField]
    float jumpSpeed;

    [SerializeField]
    int rotspeed;

    [Space(10)]

    [Header("Player Actions")]

    [Space(5)]

    [SerializeField]
    float dodgeCost;

    [SerializeField]
    float dodgeCooldown;

    [SerializeField]
    float dodgeDuration;

    [SerializeField]
    float dodgeSpeed;

    [SerializeField]
    float attackMoveLength;

    [SerializeField]
    float attackCooldown;

    [SerializeField]
    float abilityCooldown;

    [Space(10)]

    [Header("Player Physics")]

    [Space(5)]

    [SerializeField]
    float gravity;

    [SerializeField]
    float slopeLimit;

    [SerializeField]
    float slideFriction;

    [Space(10)]

    [Header("Player Sounds")]

    [Space(5)]

    [SerializeField]
    AudioClip swordSheathe;

    [SerializeField]
    AudioClip swordUnsheathe;

    [Space(10)]

    [Header("Player Items")]

    [Space(5)]

    [SerializeField]
    Transform weaponPosition;

    [SerializeField]
    SpriteRenderer currentRune;

    [Space(10)]

    [Header("Player Canvas")]

    [Space(5)]

    [SerializeField]
    Slider healthBar;

    [SerializeField]
    Slider staminaBar;

    [SerializeField]
    Slider lifeForceBar;

    [SerializeField]
    GameObject aggroIndicator;

    [SerializeField]
    GameObject deathScreen;

    #endregion

    #region Non-Serialized Variables
    CharacterController charController;

    Vector3 move, dashVelocity, dodgeVelocity, hitNormal;

    Vector3? dashDir, dodgeDir;

    MovementType currentMovementType, previousMovementType;

    PauseManager pM;

    InventoryManager inventory;

    IInteractable currentInteractable;

    InputManager iM;

    //Which moves are used depending on weapon equipped?
    BaseWeaponScript currentWeapon;

    GameObject weaponToEquip;

    Animator anim;

    float yVelocity, stamina, h, v, secondsUntilResetClick, attackCountdown = 0f, interactTime, dashedTime, poiseReset, poise;

    int health, lifeForce = 0, nuOfClicks = 0, abilityNo = 0;

    private Transform cam;

    private Vector3 camForward;

    List<BaseEnemyScript> enemiesAggroing = new List<BaseEnemyScript>();

    bool inputEnabled = true, jumpMomentum = false, grounded, invulnerable = false, canDodge = true, dead = false, canSheathe = true;
    #endregion

    #region Properties
    //Describes which kind of movement that is currently being used
    public MovementType CurrentMovementType
    {
        get { return this.currentMovementType; }
        set { this.currentMovementType = value; }
    }

    public InventoryManager Inventory
    {
        get { return this.inventory; }
    }

    BaseAbilityScript currentAbility;

    public BaseAbilityScript CurrentAbility
    {
        get { return this.currentAbility; }
    }

    public Slider LifeforceBar
    {
        get { return lifeForceBar; }
        set { lifeForceBar = LifeforceBar; }
    }

    public Slider HealthBar
    {
        get { return healthBar; }
        set { healthBar = HealthBar; }
    }

    //Gets and sets the current weapon
    public BaseWeaponScript CurrentWeapon
    {
        get { return this.currentWeapon; }
    }

    public float Stamina
    {
        get { return this.stamina; }
        set { this.stamina = value; staminaBar.value = stamina; }
    }

    public int Health
    {
        get { return this.health; }
        set { this.health = value; if (this.health <= 0f) { Death(); } }
    }

    public int LifeForce
    {
        get { return this.lifeForce; }
        set { lifeForce = value; }
    }

    public float YVelocity
    {
        set { this.yVelocity = value; }
    }

    public float InteractTime
    {
        set { interactTime = value; }
    }

    public Animator Anim
    {
        get { return this.anim; }
    }

    public bool Dead
    {
        get { return dead; }
        set { Dead = dead; }
    }
    #endregion

    #region Main Methods

    void Awake()
    {
        //Just setting all the variables needed
        iM = FindObjectOfType<InputManager>();
        charController = GetComponent<CharacterController>();
        cam = FindObjectOfType<Camera>().transform;
        this.health = maxHealth;
        this.stamina = maxStamina;
        currentMovementType = MovementType.Idle;
        pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
        inventory = gameObject.AddComponent<InventoryManager>();
        slopeLimit = charController.slopeLimit;
        anim = GetComponentInChildren<Animator>();
        healthBar.maxValue = maxHealth;
        staminaBar.maxValue = maxStamina;
        lifeForceBar.maxValue = maxLifeForce;
        healthBar.value = health;
        staminaBar.value = stamina;
        lifeForceBar.value = lifeForce;
    }

    private void Update()
    {
        if (inputEnabled && !dead)
        {
            //A sprint function which drains the stamina float upon activation
            bool sprinting = false;

            if (charController.isGrounded && Input.GetButton("Sprint") && stamina > 1f && move != Vector3.zero)
            {
                stamina -= 0.01f;
                staminaBar.value = stamina;
                sprinting = true;
            }
            else if (charController.isGrounded && stamina < maxStamina && !Input.GetButton("Sprint"))
            {
                stamina += staminaRegen;
                staminaBar.value = stamina;

                if (stamina > maxStamina)
                {
                    stamina = maxStamina;
                }
            }

            if (currentMovementType != MovementType.Attacking && currentMovementType != MovementType.Interacting && currentMovementType != MovementType.Stagger)
            {
                PlayerMovement(sprinting);
            }

            if (!charController.isGrounded)
            {
                anim.SetBool("Falling", true);
                yVelocity -= gravity;
            }
            //Lets the character move with the character controller
            charController.Move(move / 8);

            if (currentInteractable != null && Input.GetButtonDown("Interact"))
            {
                previousMovementType = currentMovementType;
                currentMovementType = MovementType.Interacting;
                currentInteractable.Interact(this);
                this.currentInteractable = null;
                move = Vector3.zero;
                StartCoroutine("NonMovingInteract");
            }

            if (charController.isGrounded && Input.GetButtonDown("Fire1") && this.currentWeapon != null && this.currentWeapon.CanAttack
                && (currentMovementType == MovementType.Idle || currentMovementType == MovementType.Running || currentMovementType == MovementType.Sprinting || currentMovementType == MovementType.Walking || currentMovementType != MovementType.Stagger))
            {
                currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = false;
                Attack();
                currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            }

            if (secondsUntilResetClick > 0)
            {
                secondsUntilResetClick -= Time.deltaTime;
            }

            if (attackCountdown > 0)
            {
                attackCountdown -= Time.deltaTime;
            }

            if (poiseReset > 0)
            {
                poiseReset -= Time.deltaTime;
            }

            if (poiseReset <= 0)
            {
                poise = maxPoise;
            }
        }
        else if (!inputEnabled || currentMovementType == MovementType.Stagger)
        {
            move = Vector3.zero;
        }
    }
    #endregion

    #region Resources
    public void RestoreHealth(int amount)
    {
        this.health = Mathf.Clamp(this.health + amount, 0, maxHealth);
        healthBar.value = health;
    }

    public void ReceiveLifeForce(int value)
    {
        this.lifeForce = Mathf.Clamp(this.lifeForce + value, 0, 100);
        lifeForceBar.value = lifeForce;
        print(lifeForce);
    }
    #endregion

    #region Equipment
    void SheatheAndUnsheathe()
    {
        if (!dead && canSheathe)
        {
            bool equip = weaponToEquip == null ? false : true;
            anim.SetBool("WeaponDrawn", equip);
            anim.SetTrigger("SheatheAndUnsheathe");
            if (!anim.GetBool("WeaponDrawn"))
            {
                //SoundManager.instance.RandomizeSfx(swordSheathe, swordSheathe);
            }
            StartCoroutine("SheathingTimer");
        }
    }

    public void Equip(GameObject equipment)
    {
        iM.SetInputMode(InputMode.Playing);
        if (dead)
        {
            return;
        }
        if (equipment == null)
        {
            this.weaponToEquip = equipment;
            SheatheAndUnsheathe();
            return;
        }
        switch (equipment.GetComponent<BaseEquippableObject>().MyType)
        {
            case EquipableType.Ability:
                if (currentAbility != null)
                    Destroy(currentAbility.gameObject);
                currentAbility = Instantiate(equipment).GetComponent<BaseEquippableObject>() as BaseAbilityScript;
                currentRune.sprite = equipment.GetComponent<BaseAbilityScript>().MyRune;
                break;

            case EquipableType.Weapon:
                this.weaponToEquip = equipment;
                SheatheAndUnsheathe();
                break;

            default:
                print("unspecified object type, gör om gör rätt");
                break;
        }
    }

    public void EnemyAggro(BaseEnemyScript enemy, bool aggroing)
    {
        if (aggroing)
        {
            enemiesAggroing.Add(enemy);
        }
        else
        {
            enemiesAggroing.Remove(enemy);
        }
        if (enemiesAggroing.Count > 0)
        {
            aggroIndicator.SetActive(true);
        }
        else
            aggroIndicator.SetActive(false);
    }

    //Code for equipping different weapons
    public void EquipWeapon(GameObject weaponToEquip)
    {
        if (dead)
            return;
        if (currentWeapon != null)
        {
            print("destroying");
            Destroy(currentWeapon.gameObject);
        }
        this.currentWeapon = Instantiate(weaponToEquip, weaponPosition).GetComponent<BaseWeaponScript>();
        this.currentWeapon.Equipper = this;
    }

    public void UnEquipWeapon()
    {
        Destroy(this.currentWeapon.gameObject);
        this.currentWeapon = null;
    }
    #endregion

    #region Combat
    //Damage to player
    public void TakeDamage(int incomingDamage)
    {
        if (dead || invulnerable)
        {
            return;
        }
        else
        {
            health -= ModifyDamage(incomingDamage);
            healthBar.value = health;

            poise -= incomingDamage;

            if (incomingDamage < health && poise < incomingDamage)
            {
                StartCoroutine("Stagger");
            }
        }

        if (health <= 0)
        {
            Death();
        }
        else
        {
            StartCoroutine("Invulnerability");
        }
    }

    //Sets the current movement type as attacking and which attack move thats used
    public void Attack()
    {
        if (charController.isGrounded && grounded && attackCountdown <= 0f)
        {
            this.currentWeapon.StartCoroutine("AttackCooldown");

            attackCooldown = 0.5f;

            currentWeapon.CurrentSpeed = 0.5f;

            if (secondsUntilResetClick <= 0)
            {
                nuOfClicks = 0;
            }

            Mathf.Clamp(nuOfClicks, 0, 3);

            nuOfClicks++;

            if (nuOfClicks == 1)
            {
                anim.SetTrigger("LightAttack1");
                secondsUntilResetClick = 1.5f;
            }

            if (nuOfClicks == 2)
            {
                anim.SetTrigger("LightAttack2");
                secondsUntilResetClick = 1.5f;
            }

            if (nuOfClicks == 3)
            {
                anim.SetTrigger("LightAttack3");
                nuOfClicks = 0;
                attackCooldown = 1f;
                currentWeapon.CurrentSpeed = 1f;
            }

            move = Vector3.zero;
            move += transform.forward * attackMoveLength;
            attackCountdown = attackCooldown;
        }
    }

    //Modifies damage depending on armor, resistance etc
    int ModifyDamage(int damage)
    {
        return damage;
    }

    public void Kill()
    {
        if (!dead)
        {
            Death();
        }
    }

    void Death()
    {
        dead = true;
        healthBar.value = 0f;
        if (hitNormal.y > 0)
        {
            //death animation och reload last saved state
            anim.SetTrigger("RightDead");
        }
        else if (hitNormal.y < 0)
        {
            anim.SetTrigger("LeftDead");
        }
        iM.SetInputMode(InputMode.Paused);
        deathScreen.SetActive(true);
    }

    #endregion

    #region Systems
    public void PauseMe(bool pausing)
    {
        inputEnabled = !pausing;
    }
    #endregion

    #region Movement
    public void PlayerMovement(bool sprinting)
    {
        // Gets the movement axis' for character controller and assigns them to variables
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //Creates a vector3 to change the character controllers forward to the direction of the camera
        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1).normalized);

        move = v * camForward + h * cam.right;

        if (move.magnitude > 0.0000001f && currentMovementType != MovementType.Dashing && currentMovementType != MovementType.Dodging)
        {
            dashDir = null;
            move *= moveSpeed;

            if (sprinting || jumpMomentum)
            {
                move *= sprintSpeed;
            }
            //Changes the character models rotation to be in the direction its moving
            transform.rotation = Quaternion.LookRotation(move);
        }

        float charSpeed = CalculateSpeed(charController.velocity);

        if (currentMovementType != MovementType.Dodging && currentMovementType != MovementType.Dashing && currentMovementType != MovementType.SuperJumping)
        {
            anim.SetFloat("Speed", charSpeed);
            if (charSpeed < 1 && currentMovementType != MovementType.Jumping)
            {
                currentMovementType = MovementType.Idle;
            }
            else if (charSpeed >= 1 && charSpeed < 5 && currentMovementType != MovementType.Jumping)
            {
                currentMovementType = MovementType.Walking;
            }
            else if (charSpeed >= 5 && charSpeed < 15 && currentMovementType != MovementType.Jumping)
            {
                currentMovementType = MovementType.Running;
            }
        }

        //If the player character is on the ground you can jump
        if (charController.isGrounded)
        {
            if (Input.GetButtonDown("Jump") && grounded && iM.CurrentInputMode == InputMode.Playing)
            {
                if (sprinting)
                {
                    jumpMomentum = true;
                }
                yVelocity = jumpSpeed;
                anim.SetTrigger("Jump");
                currentMovementType = MovementType.Jumping;
            }
        }
        move.y += yVelocity;
        //If the player character is on the ground you may dodge/roll/evade as a way to avoid something
        if (charController.isGrounded)
        {
            if (Input.GetButtonDown("Dodge"))
            {
                if (stamina >= dodgeCost && canDodge)
                {
                    anim.SetTrigger("Dodge");
                    StartCoroutine("Dodge");
                    StartCoroutine("DodgeCooldown");
                    stamina -= dodgeCost;
                }
            }
        }

        //Using the character transforms' forward direction to assign which direction to dash and then moves the character in the direction with higher velocity than normal
        if (currentMovementType == MovementType.Dodging)
        {
            if (dodgeDir == null)
            {
                dodgeVelocity = transform.forward * dodgeSpeed;
                move += dodgeVelocity;
                dodgeDir = move;
            }
            else
            {
                move = (Vector3)dodgeDir;
            }
        }

        //Using the character transforms' forward direction to assign which direction to dash and then moves the character in the direction with high velocity
        if (currentMovementType == MovementType.Dashing)
        {
            if (dashDir == null)
            {
                dashVelocity = transform.forward * 3;
                move.y = 0;
                move += dashVelocity;
                dashDir = move;
                dashedTime = 0f;
            }
            else
            {
                move = (Vector3)dashDir;
                if (dashedTime < 2f)
                {
                    print(dashedTime);
                    dashedTime += Time.deltaTime;
                }
                else
                {
                    currentMovementType = MovementType.Idle;
                }
            }
        }

        if (!grounded && hitNormal.y >= 0f) //Får spelaren att glida ned för branta ytor
        {
            float xVal = -hitNormal.y * hitNormal.x * slideFriction;
            float zVal = -hitNormal.y * hitNormal.z * slideFriction;
            move.x += xVal;
            move.z += zVal;
        }

        //If the angle of the object hit by the character controller collider is less or equal to the slopelimit you are grounded and wont slide down
        grounded = (Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit);

        if (charController.isGrounded && currentMovementType != MovementType.Dodging && currentMovementType != MovementType.Dashing)
        {
            anim.SetBool("Falling", false);
            jumpMomentum = false;
            currentMovementType = MovementType.Idle;
        }

        if (sprinting && charController.velocity.magnitude > 0f && currentMovementType != MovementType.Jumping && currentMovementType != MovementType.Dodging && currentMovementType != MovementType.Dashing)
        {
            anim.SetFloat("Speed", 20);
            currentMovementType = MovementType.Sprinting;
        }
    }

    float CalculateSpeed(Vector3 velocity)
    {
        Vector3 newVelocity = new Vector3(velocity.x, 0f, velocity.z);
        return newVelocity.magnitude;
    }
    #endregion

    #region Colliders
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IInteractable>() != null)
            currentInteractable = other.gameObject.GetComponent<IInteractable>();
    }

    void OnTriggerExit(Collider other)
    {
        IInteractable otherInteractable = other.gameObject.GetComponent<IInteractable>();
        if (otherInteractable != null && currentInteractable == otherInteractable)
            currentInteractable = null;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }
    #endregion

    #region Coroutines
    public IEnumerator AbilityCooldown()
    {
        BaseAbilityScript.CoolingDown = true;
        yield return new WaitForSeconds(abilityCooldown);
        BaseAbilityScript.CoolingDown = false;
    }

    IEnumerator DodgeCooldown()
    {
        if (!dead)
        {
            canDodge = false;
            yield return new WaitForSeconds(dodgeCooldown);
            canDodge = true;
        }
    }

    //Enumerator smooths out the dodge/roll/evade so it doesn't happen instantaneously
    IEnumerator Dodge()
    {
        if (!dead)
        {
            currentMovementType = MovementType.Dodging;
            yield return new WaitForSeconds(dodgeDuration);
            currentMovementType = MovementType.Running;
            dodgeDir = null;
        }
    }

    IEnumerator NonMovingInteract()
    {
        yield return new WaitForSeconds(interactTime);
        currentMovementType = MovementType.Idle;
    }

    IEnumerator SheathingTimer()
    {
        if (!dead)
        {
            canSheathe = false;
            yield return new WaitForSeconds(0.4f);
            if (weaponToEquip != null)
            {
                //SoundManager.instance.RandomizeSfx(swordUnsheathe, swordUnsheathe);
                EquipWeapon(weaponToEquip);
            }
            else if (currentWeapon != null)
            {
                UnEquipWeapon();
            }
            canSheathe = true;
        }
    }

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerablityTime);
        invulnerable = false;
    }

    IEnumerator Stagger()
    {
        if (currentMovementType != MovementType.Stagger)
            previousMovementType = currentMovementType;
        currentMovementType = MovementType.Stagger;
        anim.SetTrigger("Stagger");
        poiseReset = poiseCooldown;
        yield return new WaitForSeconds(staggerTime);
        currentMovementType = previousMovementType;
    }
    #endregion
}