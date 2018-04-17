using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderAI : BaseEnemyScript
{
    [SerializeField]
    AudioClip raiderHowl;

    public override void HeavyAttack()
    {
        if (!alive)
        {
            return;
        }

        StartCoroutine(FreezeNav(2f));
        previousMovementType = currentMovementType;
        this.currentMovementType = MovementType.Attacking;
        heavyAttack = Random.Range(1, 3);

        attackColliderActivationSpeed = 1.0f;
        attackColliderDeactivationSpeed = 1.5f;

        StartCoroutine(ActivateAttackCollider(heavyAttack));

        if (heavyAttack == 1)
        {
            anim.SetTrigger("HeavyAttack1");
        }
        else if (heavyAttack == 2)
        {
            anim.SetTrigger("HeavyAttack2");
        }

        weapon.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
        StartCoroutine("AttackCooldown");
    }

    protected override void Dodge()
    {
        anim.SetTrigger("Dodge");
        StartCoroutine("Invulnerability");
        nav.Move(target.transform.position + transform.position);
    }

    protected override void Aggro(PlayerControls newTarget)
    {
        base.Aggro(newTarget);
        SoundManager.instance.RandomizeSfx(raiderHowl, raiderHowl);
    }
}
