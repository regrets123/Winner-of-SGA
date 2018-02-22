using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson*/


//Interface som används av spelaren och alla fiender samt eventuella förstörbara objekt
public interface IKillable
{
    void TakeDamage(int damage);
    void Kill();
}

public enum MovementType
{
    Idle, Walking, Sprinting, Attacking, Dodging, Dashing
}

public class PlayerControls : MonoBehaviour, IKillable
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

    float yVelocity = 0.0f;

    float stamina, h, v;

    int health;

    private Transform cam;

    private Vector3 camForward;

    MovementType currentMovementType;

    //Modifierar skadan efter armor, resistance etc
    public void TakeDamage(int incomingDamage)
    {
        int damage = ModifyDamage(incomingDamage);
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

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

    void Start()
    {
        charController = GetComponent<CharacterController>();
        cam = FindObjectOfType<Camera>().transform;
        this.health = maxHealth;
        currentMovementType = MovementType.Idle;
    }

    private void Update()
    {
        bool sprinting = false;
        if (Input.GetAxis("Sprint") > 0f && stamina > 0.2f)
        {
            stamina -= 0.2f;
            sprinting = true;
        }
        else if (stamina < maxStamina)
        {
            stamina += 0.2f;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        }
        PlayerMovement(sprinting);
        if (Input.GetAxis("Interact") > 0f)
        {
            //interagera med vad det nu kan vara
        }
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
            transform.rotation = Quaternion.LookRotation(move);
        }

        if (charController.isGrounded)
        {

            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpSpeed;
                print("jumping");
            }
        }
        else
        {
            yVelocity -= gravity;
        }

        move.y += yVelocity;

        charController.Move(move / 8);


    }
}