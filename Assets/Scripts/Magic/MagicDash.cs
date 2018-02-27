using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilssion && Björn Andersson*/

public class MagicDash : BaseAbilityScript
{
    //Vector3 dashVelocity;
    //Vector3 move;

    //bool dashing = false;

    MovementType previousMovementType;

    public override void UseAbility()
    {
        StartCoroutine("Dash");
    }

    IEnumerator Dash()
    {
        previousMovementType = player.CurrentMovementType;
        player.CurrentMovementType = MovementType.Dashing;
        yield return new WaitForSeconds(0.6f);
        player.CurrentMovementType = previousMovementType;
    }
}
