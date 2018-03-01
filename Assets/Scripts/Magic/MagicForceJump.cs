using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson && Björn Andersson*/

public class MagicForceJump : BaseAbilityScript
{
    [SerializeField]
    public GameObject effectsPrefab;
    [SerializeField]
    public GameObject spawnPos;
    [SerializeField]
    float magicJumpSpeed;
    
    public override void UseAbility()
    {
        if (player.GetComponent<CharacterController>().isGrounded)
        {
                //instantiate a magic jump circle
                GameObject jumpParticles = Instantiate(effectsPrefab, spawnPos.transform.position, spawnPos.transform.rotation);

                //Add a force to the player going up form your current position.
                player.GetComponent<PlayerControls>().yVelocity = magicJumpSpeed;

                Destroy(jumpParticles, 1.5f);
        }
    }
}
