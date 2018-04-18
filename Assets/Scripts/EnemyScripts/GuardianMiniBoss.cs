using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianMiniBoss : GuardianAI {

    [SerializeField]
    GameObject myDrop;

    protected override void Death()
    {
        base.Death();
        Instantiate(myDrop);
    }
}
