using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class BaseAbilityScript : BaseEquippableObject {


    [SerializeField]
    protected float staminaCost;

    public virtual void UseAbility()
    {

    }

    protected virtual void Update()
    {
        if ((player.CurrentMovementType == MovementType.Idle
            || player.CurrentMovementType == MovementType.Sprinting //Låter spelaren använda abilities när den inte attackerar, dodgar eller liknande
            || player.CurrentMovementType == MovementType.Walking)
            && Input.GetButtonDown("Ability"))
        {
            UseAbility();
        }
    }

    public override void Equip()
    {
        base.Equip();
        player.CurrentAbility = this;
    }

}
