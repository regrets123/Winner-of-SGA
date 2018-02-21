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
    int maxHealth;

    [SerializeField]
    float maxStamina;

    [SerializeField]
    float moveSpeed = 5.0f;

    public float h;
    public float v;

    float yVelocity = 0.0f;

    float stamina;

    int health;

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
    }

    public void PlayerMovement(bool sprinting)
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v);
        Vector3 velocity = direction * moveSpeed;

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
        velocity = transform.TransformDirection(velocity);

        charController.Move(velocity * Time.deltaTime);
    }
}