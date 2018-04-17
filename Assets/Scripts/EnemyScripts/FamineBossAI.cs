using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FamineBossAI : BaseEnemyScript
{

    [SerializeField]
    Slider healthSlider;

    [SerializeField]
    Text bossNameText;

    [SerializeField]
    GameObject blackHole;

    [SerializeField]
    Transform vortexPos;

    GameObject consumeObj;

    float gapCloser, consumeTime = 5f;

    bool enraged = false, enraging = false, consuming = false, teleporting = false;

    public bool Enraged
    {
        get { return enraged; }
    }

    public bool Consuming
    {
        get { return consuming; }
    }

    protected override void Start()
    {
        base.Start();
        bossNameText.text = this.name;
    }

    protected override void Update()
    {
        if (!enraging && !teleporting)
        {
            base.Update();

            if (alive && target != null && canAttack && weapon.GetComponent<BaseWeaponScript>().CanAttack && !target.Dead && Vector3.Distance(transform.position, target.transform.position) > 8 &&
                Vector3.Distance(transform.position, target.transform.position) < aggroRange && this.currentMovementType != MovementType.Attacking && !consuming)
            {
                gapCloser = Random.Range(1, 3);

                if (gapCloser == 1)
                {
                    StartCoroutine("Teleport");
                }
                else if (gapCloser == 2)
                {
                    StartCoroutine("JumpSlash");
                }
            }
        }
    }

    public override void TakeDamage(int incomingDamage, DamageType dmgType)
    {
        base.TakeDamage(incomingDamage, dmgType);

        if (health < maxHealth / 3 && !enraged)
        {
            Enrage();
        }

        //healthSlider.value = health;
    }

    void Enrage()
    {
        nav.isStopped = true;
        anim.SetTrigger("Enrage");
        enraged = true;
        StartCoroutine("WaveArms");
    }

    protected override void Aggro(PlayerControls newTarget)
    {
        base.Aggro(newTarget);
    }

    void MagicAttack()
    {
        nav.isStopped = true;
        consuming = true;
        anim.SetTrigger("MagicAttack");
        anim.SetBool("Consume", true);
        StartCoroutine("Consume");
    }

    IEnumerator JumpSlash()
    {
        if (alive && target != null)
        {
            previousMovementType = currentMovementType;
            this.currentMovementType = MovementType.Attacking;
            attackColliderActivationSpeed = 0.5f;
            attackColliderDeactivationSpeed = 1.0f;
            nav.isStopped = true;
            yield return new WaitForSeconds(0.5f);
            nav.isStopped = false;
            attackColliderActivationSpeed = 0.2f;
            attackColliderDeactivationSpeed = 4f;
            StartCoroutine(ActivateAttackCollider(1));
            anim.SetTrigger("JumpSlash");
            Vector3 targetPosition = target.gameObject.transform.position;
            float originalSpeed = nav.speed;
            nav.speed = nav.speed * 4;
            nav.destination = targetPosition;
            weapon.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
            StartCoroutine("AttackCooldown");
            yield return new WaitForSeconds(1.5f);
            nav.speed = originalSpeed;
            nav.destination = target.gameObject.transform.position;
        }
    }

    IEnumerator Teleport()
    {
        if (alive && target != null)
        {
            nav.isStopped = true;
            teleporting = true;
            anim.SetBool("Teleport", true);
            yield return new WaitForSeconds(2f);
            teleporting = false;
            nav.isStopped = false;
            Vector3 targetPosition = target.gameObject.transform.position + Vector3.back * 4;
            float originalSpeed = nav.speed;
            nav.speed = nav.speed * 2;
            nav.destination = targetPosition;
            weapon.GetComponent<BaseWeaponScript>().StartCoroutine("AttackCooldown");
            StartCoroutine("AttackCooldown");
            yield return new WaitForSeconds(2.5f);
            anim.SetBool("Teleport", false);
            nav.speed = originalSpeed;
            nav.destination = target.gameObject.transform.position;
        }
    }

    IEnumerator WaveArms()
    {
        this.enraging = true;
        yield return new WaitForSeconds(2.7f);
        this.enraging = false;
        MagicAttack();
    }

    IEnumerator Consume()
    {
        transform.LookAt(target.transform);
        transform.rotation = new Quaternion(0f, transform.rotation.y, 0f, transform.rotation.w);
        yield return new WaitForSeconds(1);
        consumeObj = Instantiate(blackHole, vortexPos);
        yield return new WaitForSeconds(consumeTime);
        Destroy(consumeObj);
        anim.SetBool("Consume", false);
        nav.isStopped = false;
        consuming = false;
    }

}
