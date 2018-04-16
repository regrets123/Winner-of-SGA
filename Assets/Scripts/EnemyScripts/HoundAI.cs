using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class HoundAI : BaseEnemyScript
{
    [SerializeField]
    float howlTime, minJumpAttackDistance;

    [SerializeField]
    GameObject weapon2;

    [SerializeField]
    Transform weapon2Pos;

    protected override void Start()
    {
        base.Start();
        this.weapon2 = Instantiate(weapon2, weapon2Pos);
        weapon2.GetComponent<BaseWeaponScript>().GetComponent<Collider>().enabled = false;
    }

    protected override void Aggro(PlayerControls newTarget)
    {
        StartCoroutine(AggroHowl(newTarget));
    }

    public override void LightAttack()
    {
        previousMovementType = currentMovementType;
        this.currentMovementType = MovementType.Attacking;
        anim.SetTrigger("SwipeAttack");
        weapon.GetComponent<BaseWeaponScript>().Attack(0.5f, false);
        weapon.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
        StartCoroutine("AttackCooldown");
    }

    public override void HeavyAttack()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < minJumpAttackDistance)
        {
            LightAttack();
            return;
        }
        previousMovementType = currentMovementType;
        this.currentMovementType = MovementType.Attacking;
        weapon2.GetComponent<BaseWeaponScript>().Attack(1f, true);
        weapon2.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
        StartCoroutine("AttackCooldown");
    }

    IEnumerator AggroHowl(PlayerControls newTarget)
    {
        transform.LookAt(newTarget.transform);
        transform.rotation = new Quaternion(0f, transform.rotation.y, 0f, transform.rotation.w);
        anim.SetTrigger("Aggro");
        yield return new WaitForSeconds(howlTime);
        base.Aggro(newTarget);
    }
}
