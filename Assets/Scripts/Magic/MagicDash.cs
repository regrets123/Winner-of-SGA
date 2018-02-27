using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilssion && Björn Andersson*/

public class MagicDash : BaseAbilityScript
{
    MovementType previousMovementType;

    public override void UseAbility()
    {
        if (player.Stamina >= staminaCost)
        {
            StartCoroutine("Dash");
            player.Stamina -= staminaCost;
        }
    }

    //Enumerator smooths out the dash so it doesn't happen instantaneously
    IEnumerator Dash()
    {
        previousMovementType = player.CurrentMovementType;
        player.CurrentMovementType = MovementType.Dashing;
        yield return new WaitForSeconds(0.3f);
        player.CurrentMovementType = previousMovementType;
    }
}
