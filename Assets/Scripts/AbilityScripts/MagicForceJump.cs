﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson && Björn Andersson*/

public class MagicForceJump : BaseAbilityScript
{
    [SerializeField]
    GameObject effectsPrefab, spawnPos;

    [SerializeField]
    float magicJumpSpeed, delayTime;

    public override void UseAbility()
    {
        if (player.GetComponent<CharacterController>().isGrounded)
        {
            base.UseAbility();
            //instantiate a magic jump circle
            StartCoroutine("SuperJump");
            player.CurrentMovementType = MovementType.SuperJumping;
        }
    }

    IEnumerator SuperJump()
    {   
        GameObject jumpParticles = Instantiate(effectsPrefab, spawnPos.transform.position, spawnPos.transform.rotation);
        //player.Anim.SetTrigger("SuperJump");
        yield return new WaitForSeconds(delayTime);
        //Add a force to the player going up form your current position.
        player.YVelocity = magicJumpSpeed;
        Destroy(jumpParticles, 1.5f);
    }
}
