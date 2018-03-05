using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson && Björn Andersson*/

public class MagicForceJump : BaseAbilityScript
{
    [SerializeField]
    GameObject effectsPrefab, spawnPos;

    [SerializeField]
    float magicJumpSpeed;
    
    public override void UseAbility()
    {
        if (player.GetComponent<CharacterController>().isGrounded)
        {
                //instantiate a magic jump circle
                GameObject jumpParticles = Instantiate(effectsPrefab, spawnPos.transform.position, spawnPos.transform.rotation);

                //Add a force to the player going up form your current position.
                player.YVelocity = magicJumpSpeed;

                Destroy(jumpParticles, 1.5f);
        }
    }
}
