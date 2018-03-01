using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectileDestroy : MonoBehaviour
{
    [SerializeField]
    protected IKillable targetToHit;

    [SerializeField]
    protected int damage = 15;

    void Start()
    {
        Destroy(transform.parent.gameObject, 8);
    }

    public virtual void DealDamage(IKillable target)
    {
        target.TakeDamage(damage);
    }

    void OnTriggerEnter(Collider other)
    {
        targetToHit = other.gameObject.GetComponent<IKillable>();

        if (targetToHit != null && other.gameObject.tag != "Player")
        {
            DealDamage(targetToHit);
            Destroy(transform.parent.gameObject);
        }
        else if (other.gameObject.layer == 8)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
