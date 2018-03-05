using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class HealthPotion : BaseAbilityScript
{
    [SerializeField]
    int amount;

    public override void UseAbility()
    {
        player.RestoreHealth(amount);
    }
}
