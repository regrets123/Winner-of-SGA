using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class BaseAbilityScript : BaseEquippableObject {
    
    public virtual void UseAbility()
    {

    }

    protected virtual void Update()
    {
        if (Input.GetButtonDown("Ability"))
        {
            UseAbility();
        }
    }

    public override void Equip()
    {
        player.CurrentAbility = this;
    }

}
