using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson & Björn Andersson*/


//Interface som används av spelaren och alla fiender samt eventuella förstörbara objekt
public interface IKillable
{
    void Attack(int attackMove);
    void TakeDamage(int damage);
    void Kill();
}

public enum MovementType
{
    Idle, Walking, Sprinting, Attacking, Dodging, Dashing
}

public class PlayerControls : MonoBehaviour, IKillable, IPausable
{
    CharacterController charController;

    [SerializeField]
    float jumpSpeed = 20.0f;

    [SerializeField]
    float gravity = 1.0f;

    [SerializeField]
    int maxHealth, rotspeed;

    [SerializeField]
    float maxStamina;

    [SerializeField]
    float moveSpeed = 5.0f;

    public float yVelocity = 0.0f;

    float stamina, h, v;

    int health;

    private Transform cam;

    private Vector3 camForward;

    bool inputEnabled = true;

    MovementType currentMovementType;

    PauseManager pM;

    [SerializeField]
    GameObject[] weapons;

    

    //Which moves are used depending on weapon equipped?
    public MovementType CurrentMovementType
    {
        get { return this.currentMovementType; }
    }

    BaseWeaponScript currentWeapon;

    [SerializeField]
    Transform weaponPosition;

    //Which weapon is equipped?
    BaseWeaponScript CurrentWeapon
    {
        get { return this.currentWeapon; }
    }

    bool jumpMomentum = false;

    public float YVelocity
    {
        get { return yVelocity; }
        set { yVelocity = YVelocity; }
    }

    void Start()
    {
        charController = GetComponent<CharacterController>();
        cam = FindObjectOfType<Camera>().transform;
        this.health = maxHealth;
        this.stamina = maxStamina;
        currentMovementType = MovementType.Idle;
        EquipWeapon(0);
        pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
    }

    private void Update()
    {
        if (inputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EquipWeapon(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EquipWeapon(1);
            }
            bool sprinting = false;
            if (charController.isGrounded && Input.GetButton("Sprint") && stamina > 1f)
            {
                stamina -= 1f;
                sprinting = true;
            }
            else if (stamina < maxStamina && Input.GetButton("Sprint"))
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
        }
    }
    //Damage to player
    public void TakeDamage(int incomingDamage)
    {
        int damage = ModifyDamage(incomingDamage);
        health -= damage;
        print(health);
        if (health <= 0)
        {
            Death();
        }
    }

    public void PauseMe(bool pausing)
    {
        inputEnabled = !pausing;
    }

    
    //Code for equipping different weapons
    public void EquipWeapon(int weaponToEquip)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);
        this.currentWeapon = Instantiate(weapons[weaponToEquip], weaponPosition).GetComponent<BaseWeaponScript>();
    }

    //What movetype is used for attack?
    public void Attack(int attackMove)
    {
        this.currentMovementType = MovementType.Attacking;
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

    public void PlayerMovement(bool sprinting)
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1).normalized);

        Vector3 move = v * camForward + h * cam.right;

        if (move.magnitude > 0.0000001f)
        {
            currentMovementType = sprinting ? MovementType.Sprinting : MovementType.Idle;

            move.Normalize();
            move *= moveSpeed;

            if (sprinting || jumpMomentum)
            {
                move *= 4;
            }
            transform.rotation = Quaternion.LookRotation(move);
        }

        //If the player character is on the ground you can jump
        if (charController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (sprinting)
                {
                    jumpMomentum = true;
                }
                yVelocity = jumpSpeed;
            }
        }
        else
        {
            yVelocity -= gravity;
        }

        move.y += yVelocity;

        charController.Move(move / 8);
        if (jumpMomentum && charController.isGrounded)
        {
            jumpMomentum = false;
        }
    }
}