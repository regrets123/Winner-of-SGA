using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class BaseAbilityScript : BaseEquippableObject {


    [SerializeField]
    protected float staminaCost, cooldownTime;

    bool coolingDown = false;

    public virtual void UseAbility()
    {
        StartCoroutine("Cooldown");
    }

    protected virtual void Update()
    {
        if ((player.CurrentMovementType == MovementType.Idle
            || player.CurrentMovementType == MovementType.Sprinting //Låter spelaren använda abilities när den inte attackerar, dodgar eller liknande
            || player.CurrentMovementType == MovementType.Walking
            || player.CurrentMovementType == MovementType.Jumping)
            && Input.GetButtonDown("Ability")
            && !coolingDown)
        {
            UseAbility();
        }
    }

    public override void Equip()
    {
        base.Equip();
        player.CurrentAbility = this;
    }

    protected IEnumerator Cooldown()
    {
        coolingDown = true;
        yield return new WaitForSeconds(cooldownTime);
        coolingDown = false;
    }

}
