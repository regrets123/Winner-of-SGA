using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicForceJump : BaseAbilityScript
{
    [SerializeField]
    public GameObject magicJumpPrefab;
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public GameObject spawnPos;
    [SerializeField]
    float magicJumpSpeed;

    //A currently public float, 
    //so you can adjust the speed as necessary.
    public float speed = 20.0f;

    void Update()
    {
        if (playerPrefab.GetComponent<CharacterController>().isGrounded)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                //instantiate a magic jump circle
                GameObject magicJump = Instantiate(magicJumpPrefab, spawnPos.transform.position, spawnPos.transform.rotation);

                //Add a force to the player going up form your current position.
                playerPrefab.GetComponent<PlayerControls>().yVelocity = magicJumpSpeed;

                Destroy(magicJump, 1.5f);
            }
        }
    }
}
