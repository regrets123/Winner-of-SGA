using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamageTrigger : MonoBehaviour
{
    [SerializeField]
    protected int damage;

    public virtual void DealDamage(IKillable target)
    {
        target.TakeDamage(damage);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            DealDamage(other.gameObject.GetComponent<IKillable>());
        }
    }
}