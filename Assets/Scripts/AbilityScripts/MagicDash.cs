using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilssion && Björn Andersson*/

public class MagicDash : BaseAbilityScript
{
    [SerializeField]
    float duration;

    //Activated from the BaseAbility script. If the player have enough stamina the ability will activate and drain the staminaCost
    public override void UseAbility()
    {
            base.UseAbility();
            player.Anim.SetTrigger("Dash");
            StartCoroutine("Dash");
    }

    protected override void Update()
    {
        if ((player.CurrentMovementType == MovementType.Idle
           || player.CurrentMovementType == MovementType.Sprinting //Låter spelaren använda abilities när den inte attackerar, dodgar eller liknande
           || player.CurrentMovementType == MovementType.Walking
           || player.CurrentMovementType == MovementType.Jumping)
           && Input.GetButtonDown("Ability")
           && !coolingDown && !player.Dead)
        {
            if (player.Stamina >= abilityCost)
            {
                player.StaminaBar.value = player.Stamina;
                UseAbility();
            }
        }
    }

    //Enumerator smooths out the dash so it doesn't happen instantaneously
    IEnumerator Dash()
    {
        player.CurrentMovementType = MovementType.Dashing;
        yield return new WaitForSeconds(duration);
        player.CurrentMovementType = MovementType.Running;
    }
}
