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
    Idle, Walking, Sprinting, Attacking, Dodging, Dashing, Jumping, Running, Interacting
}

public class PlayerControls : MonoBehaviour, IKillable, IPausable
{
    [SerializeField]
    float jumpSpeed, gravity, maxStamina, moveSpeed, slopeLimit, slideFriction, dodgeCost, invulnerablityTime, maxLifeForce, dodgeCooldown, dodgeDuration, dodgeSpeed, attackMoveLength, attackCooldown;

    [SerializeField]
    int maxHealth, rotspeed;

    [SerializeField]
    Transform weaponPosition;

    [SerializeField]
    AudioClip swordSheathe, swordUnsheathe;

    CharacterController charController;

    Vector3 move, dashVelocity, dodgeVelocity, hitNormal;

    Vector3? dashDir, dodgeDir;

    float yVelocity, stamina, h, v, secondsUntilResetClick, attackCountdown = 0f, interactTime;

    int health, lifeForce = 0, nuOfClicks = 0;

    private Transform cam;

    private Vector3 camForward;

    bool inputEnabled = true, jumpMomentum = false, grounded, invulnerable = false, canDodge = true, dead = false;

    MovementType currentMovementType, previousMovementType;

    PauseManager pM;

    InventoryManager inventory;

    IInteractable currentInteractable;

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

    public int Health
    {
        get { return this.health; }
    }

    public int LifeForce
    {
        get { return this.lifeForce; }
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

    [SerializeField]
    GameObject dashTest;

    bool canSheathe = true;

    void Start()
    {
        //Just setting all the variables needed
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
        this.currentAbility = Instantiate(dashTest).GetComponent<MagicDash>();
    }

    void SheatheAndUnsheathe()
    {
        if (!dead && canSheathe)
        {
            anim.SetBool("WeaponDrawn", !anim.GetBool("WeaponDrawn"));
            anim.SetTrigger("SheatheAndUnsheathe");
            if (!anim.GetBool("WeaponDrawn"))
            {
                SoundManager.instance.RandomizeSfx(swordSheathe, swordSheathe);
            }
            StartCoroutine("SheathingTimer");
        }
    }

    IEnumerator SheathingTimer()
    {
        if (!dead)
        {
            canSheathe = false;
            yield return new WaitForSeconds(0.4f);
            if (currentWeapon != null)
            {
                UnEquipWeapon();
            }
            else
            {
                //Equip(weapons[0]);
                EquipWeapon(0);
                SoundManager.instance.RandomizeSfx(swordUnsheathe, swordUnsheathe);
            }
            canSheathe = true;
        }
    }

    public void RestoreHealth(int amount)
    {
        this.health = Mathf.Clamp(this.health + amount, 0, maxHealth);
    }

    public void Equip(GameObject equipment)
    {
        /*
        BaseEquippableObject equippable = Instantiate(equipment, weaponPosition).GetComponent<BaseEquippableObject>();
        if (equippable is BaseWeaponScript && currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
            currentWeapon = equippable as BaseWeaponScript;
        }
        else if (equippable is BaseAbilityScript && currentAbility != null)
        {
            
        }
        */

        if (dead)
            return;

        switch (equipment.GetComponent<BaseEquippableObject>().MyType)
        {
            case EquipableType.Ability:
                Destroy(currentAbility.gameObject);
                currentAbility = equipment.GetComponent<BaseEquippableObject>() as BaseAbilityScript;
                break;

            case EquipableType.Weapon:
                SheatheAndUnsheathe();
                break;

            default:
                print("unspecified object type, gör om gör rätt");
                break;
        }
    }

    //Code for equipping different weapons
    public void EquipWeapon(int weaponToEquip)
    {
        if (dead)
            return;
        if (currentWeapon != null)
        {
            print("destroying");
            Destroy(currentWeapon.gameObject);
        }
        this.currentWeapon = Instantiate(inventory.EquippableWeapons[weaponToEquip], weaponPosition).GetComponent<BaseWeaponScript>();
        this.currentWeapon.Equipper = this;
    }

    public void UnEquipWeapon()
    {
        Destroy(this.currentWeapon.gameObject);
        this.currentWeapon = null;
    }

    private void Update()
    {
        if (inputEnabled && !dead)
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

            if (currentMovementType != MovementType.Attacking && currentMovementType != MovementType.Interacting)
            {
                PlayerMovement(sprinting);
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
                //interagera med vad det nu kan vara
            }

            if (Input.GetButtonDown("Fire1") && this.currentWeapon != null && this.currentWeapon.CanAttack
                && (currentMovementType == MovementType.Idle || currentMovementType == MovementType.Running || currentMovementType == MovementType.Sprinting || currentMovementType == MovementType.Walking))
            {
                Attack();
            }

            if (secondsUntilResetClick > 0)
            {
                secondsUntilResetClick -= Time.deltaTime;
            }
            if (attackCountdown > 0)
            {
                attackCountdown -= Time.deltaTime;
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
        if (dead)
            return;
        if (invulnerable)
        {
            return;
        }
        else
        {
            health -= ModifyDamage(incomingDamage);
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

    public void PauseMe(bool pausing)
    {
        inputEnabled = !pausing;
    }

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnerablityTime);
        invulnerable = false;
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
            Death();
    }

    void Death()
    {
        dead = true;
        if (hitNormal.y > 0)
        {
            //death animation och reload last saved state
            anim.SetTrigger("RightDead");
        }
        else if (hitNormal.y < 0)
        {
            anim.SetTrigger("LeftDead");
        }
        FindObjectOfType<SaveManager>().ReloadGame(); //Temporary
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
            move *= moveSpeed;

            if (sprinting || jumpMomentum)
            {
                move *= 4;
            }
            //Changes the character models rotation to be in the direction its moving
            transform.rotation = Quaternion.LookRotation(move);
        }

        float charSpeed = CalculateSpeed(charController.velocity);

        anim.SetFloat("Speed", charSpeed);
        if (currentMovementType != MovementType.Dodging && currentMovementType != MovementType.Dashing)
        {
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
            if (Input.GetButtonDown("Jump") && grounded)
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
                move.y = 0;
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
            previousMovementType = currentMovementType;
            currentMovementType = MovementType.Dodging;
            yield return new WaitForSeconds(dodgeDuration);
            currentMovementType = previousMovementType;
            dodgeDir = null;
        }
    }

    IEnumerator NonMovingInteract()
    {
        yield return new WaitForSeconds(interactTime);
        currentMovementType = previousMovementType;
    }

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
}