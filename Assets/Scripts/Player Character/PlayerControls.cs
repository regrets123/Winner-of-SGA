using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Idle, Walking, Sprinting, Attacking, Dodging, Dashing, Jumping, Running
}

public class PlayerControls : MonoBehaviour, IKillable, IPausable
{
    [SerializeField]
    float jumpSpeed, gravity, maxStamina, moveSpeed, slopeLimit, slideFriction, dodgeCost, invulnerablityTime, maxLifeForce, dodgeCooldown, dodgeDuration, dodgeSpeed;

    [SerializeField]
    int maxHealth, rotspeed;

    [SerializeField]
    GameObject[] weapons;

    [SerializeField]
    Transform weaponPosition;

    CharacterController charController;

    Vector3 move, dashVelocity, dodgeVelocity, hitNormal;

    Vector3? dashDir, dodgeDir;

    float yVelocity;

    float stamina, h, v;

    int health, lifeForce = 0;

    private Transform cam;

    private Vector3 camForward;

    bool inputEnabled = true, jumpMomentum = false, grounded, invulnerable = false, canDodge = true;

    MovementType currentMovementType, previousMovementType;

    PauseManager pM;

    InventoryManager inventory;

    InputManager iM;

    Rigidbody rB;

    public float Stamina
    {
        get { return this.stamina; }
        set { this.stamina = value; }
    }

    //Which moves are used depending on weapon equipped?
    BaseWeaponScript currentWeapon;

    Animator anim;

    //Only test not final product
    [SerializeField]
    Animation attackAnim;

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
        set { this.currentAbility = value; }
    }

    //Gets and sets the current weapon
    public BaseWeaponScript CurrentWeapon
    {
        get { return this.currentWeapon; }
        set { this.currentWeapon = value; }
    }

    public int LifeForce
    {
        get { return this.lifeForce; }
    }

    public float YVelocity
    {
        set { this.yVelocity = value; }
    }

    void Start()
    {
        //Just setting all the variables needed
        rB = GetComponent<Rigidbody>();
        iM = FindObjectOfType<InputManager>();
        charController = GetComponent<CharacterController>();
        cam = FindObjectOfType<Camera>().transform;
        this.health = maxHealth;
        this.stamina = maxStamina;
        currentMovementType = MovementType.Idle;
        EquipWeapon(0);
        pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
        inventory = new InventoryManager(this);
        slopeLimit = charController.slopeLimit;
        anim = GetComponentInChildren<Animator>();
    }

    public void RestoreHealth(int amount)
    {
        this.health = Mathf.Clamp(this.health + amount, 0, maxHealth);
    }

    public void Equip(GameObject equipment)
    {
        BaseEquippableObject equippable = Instantiate(equipment, weaponPosition).GetComponent<BaseEquippableObject>();
        if (equippable is BaseWeaponScript && currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = equippable as BaseWeaponScript;
        }
        else if (equippable is BaseAbilityScript && currentAbility != null)
        {
            Destroy(currentAbility);
            currentAbility = equippable as BaseAbilityScript;
        }
    }

    public void UnEquipWeapon()
    {
        Destroy(this.currentWeapon);
        this.currentWeapon = null;
    }

    private void Update()
    {
        if (inputEnabled)
        {
            //A sprint function which drains the stamina float upon activation
            bool sprinting = false;
            if (charController.isGrounded && Input.GetButton("Sprint") && stamina > 1f)
            {
                stamina -= 1f;
                sprinting = true;
            }
            else if (charController.isGrounded && stamina < maxStamina && Input.GetButton("Sprint"))
            {
                stamina += 1f;
                if (stamina > maxStamina)
                {
                    stamina = maxStamina;
                }
            }

            PlayerMovement(sprinting);

            if (Input.GetButtonDown("Interact"))
            {
                //interagera med vad det nu kan vara
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
            }
        }
    }

    public void ReceiveLifeForce(int value)
    {
        this.lifeForce = Mathf.Clamp(this.lifeForce + value, 0, 100);
    }

    //Damage to player
    public void TakeDamage(int incomingDamage)
    {
        if (invulnerable)
            return;
        health -= ModifyDamage(incomingDamage);
        print(health);
        if (health <= 0)
        {
            Death();
        }
        else
        {
            StartCoroutine("Invulnerability");
        }
    }

    public void PauseMe(bool pausing)
    {
        inputEnabled = !pausing;
    }

    IEnumerator Invulerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerablityTime);
        invulnerable = false;
    }

    //Code for equipping different weapons
    public void EquipWeapon(int weaponToEquip)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);
        //this.currentWeapon = Instantiate(weapons[weaponToEquip], weaponPosition).GetComponent<BaseWeaponScript>();
    }

    //Sets the current movement type as attacking and which attack move thats used
    public void Attack()
    {
        this.currentMovementType = MovementType.Attacking;

        anim.SetTrigger("Attack");
    }

    //Modifies damage depending on armor, resistance etc
    int ModifyDamage(int damage)
    {
        return damage;
    }

    public void Kill()
    {
        Death();
    }

    void Death()
    {
        //death animation och reload last saved state
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

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
            currentMovementType = sprinting ? MovementType.Sprinting : MovementType.Idle;

            move.Normalize();
            move *= moveSpeed;


            if (sprinting || jumpMomentum)
            {
                move *= 4;
            }
            //Changes the character models rotation to be in the direction its moving
            transform.rotation = Quaternion.LookRotation(move);
        }
        anim.SetFloat("Speed", CalculateSpeed(charController.velocity));

        //If the player character is on the ground you can jump
        if (charController.isGrounded)
        {
            if (Input.GetButtonDown("Jump") && grounded)
            {
                if (sprinting)
                {
                    jumpMomentum = true;
                }
                yVelocity = jumpSpeed;
                anim.SetTrigger("Jump");
            }
        }
        else
        {
            anim.SetBool("Falling", true);
            yVelocity -= gravity;
        }

        move.y += yVelocity;

        //If the player character is on the ground you may dodge/roll/evade as a way to avoid something
        if (charController.isGrounded)
        {
            if (Input.GetButtonDown("Dodge"))
            {
                if (stamina >= dodgeCost && canDodge)
                {
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
                move += dashVelocity;
                dashDir = move;
            }
            else
            {
                move = (Vector3)dashDir;
            }
        }

        if (!grounded && hitNormal.y >= 0f) //Får spelaren att glida ned för branta ytor
        {
            float xVal = -hitNormal.y * hitNormal.x * slideFriction;
            float zVal = -hitNormal.y * hitNormal.z * slideFriction;
            move.x += xVal;
            move.z += zVal;
        }

        //Lets the character move with the character controller
        charController.Move(move / 8);

        //If the angle of the object hit by the character controller collider is less or equal to the slopelimit you are grounded and wont slide down
        grounded = (Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit);

        if (charController.isGrounded)
        {
            anim.SetBool("Falling", false);
        }

        if (jumpMomentum && charController.isGrounded)
        {
            jumpMomentum = false;
        }

        if (sprinting && charController.velocity.magnitude > 0f)
        {
            anim.SetFloat("Speed", 20);
        }
    }

    float CalculateSpeed(Vector3 velocity)
    {
        Vector3 newVelocity = new Vector3(velocity.x, 0f, velocity.z);
        return newVelocity.magnitude;
    }

    IEnumerator DodgeCooldown()
    {
        canDodge = false;
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    //Enumerator smooths out the dodge/roll/evade so it doesn't happen instantaneously
    IEnumerator Dodge()
    {
        previousMovementType = currentMovementType;
        currentMovementType = MovementType.Dodging;
        yield return new WaitForSeconds(dodgeDuration);
        currentMovementType = previousMovementType;
        dodgeDir = null;
    }
}