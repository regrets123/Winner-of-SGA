using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    CharacterController charController;

    [SerializeField]
    int health, stamina;

    [SerializeField]
    float jumpSpeed = 20.0f;

    [SerializeField]
    float gravity = 1.0f;

    [SerializeField]
    float moveSpeed = 5.0f;

    public float h;
    public float v;

    float yVelocity = 0.0f;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerMovement();
    }

    public void PlayerMovement()
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
