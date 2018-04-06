using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAI : BaseEnemyScript
{
    protected override void DashAttack()
    {
        if (!alive)
        {
            return;
        }

        nav.isStopped = true;
        previousMovementType = currentMovementType;
        this.currentMovementType = MovementType.Attacking;
        lightAttack = Random.Range(1, 4);
        attackColliderActivationSpeed = 0.5f;
        attackColliderDeactivationSpeed = 1.0f;
        StartCoroutine("ActivateAttackCollider");
        anim.SetTrigger("DashAttack");
        nav.Move(target.gameObject.transform.position);
        weapon.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
        StartCoroutine("AttackCooldown");
    }
}
