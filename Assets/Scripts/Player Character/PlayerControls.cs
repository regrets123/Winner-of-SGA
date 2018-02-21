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

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
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
    }

    private void Update()
    {
        bool sprinting = false;
        if (Input.GetAxis("Sprint") > 0f)
        {
            stamina -= 1;
            sprinting = true;
        }
        else if (stamina < maxStamina)
        {
            stamina += 1;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        }
        PlayerMovement(sprinting);
        //MoveMe();
    }

    void MoveMe()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0, v);
        Vector3 velocity = movement * moveSpeed;
        if (velocity != Vector3.zero)
        {
            Quaternion newRot = transform.rotation;
            newRot.y = cam.transform.rotation.y;
            transform.rotation = newRot;
        }

        if (charController.isGrounded)
        {

            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpSpeed;
            }
        }
        else
        {
            yVelocity -= gravity;
        }

        velocity.y = yVelocity;
        //velocity = transform.TransformDirection(velocity);

        charController.Move(movement * moveSpeed * Time.deltaTime);


        //transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
    }

    public void PlayerMovement(bool sprinting)
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1).normalized);

        Vector3 move = v * camForward + h * cam.right;

        if (move.magnitude > 0.0000001f)
        {
            move.Normalize();
            move *= moveSpeed;
            transform.rotation = Quaternion.LookRotation(move);
        }


        charController.Move(move / 8);

        //Vector3 movement = new Vector3(h, 0, v);
        //Vector3 velocity = movement * moveSpeed;

        //movement = transform.TransformDirection(movement);

        //Quaternion newRot = transform.rotation;
        //newRot.y = cam.transform.rotation.y;

        /*
        Vector3 nextDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (nextDir != Vector3.zero)
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, nextDir, Time.deltaTime * rotspeed, 0.0f);
        }

        charController.Move(nextDir/8);
        */
        //transform.Rotate(0f, Input.GetAxis("Horizontal") * rotspeed, 0f);

        //charController.SimpleMove(movement);

        // movement = cam.transform.TransformDirection(movement);

        if (charController.isGrounded)
        {

            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpSpeed;
            }
        }
        else
        {
            yVelocity -= gravity;
        }

        //velocity.y = yVelocity;
        //velocity = transform.TransformDirection(velocity);

        //charController.Move(movement * moveSpeed * Time.deltaTime);

    }
}