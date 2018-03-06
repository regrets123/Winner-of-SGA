using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class LifeForceTransmitterScript : MonoBehaviour
{
    PlayerControls target;

    int lifeForceAmount;

    Vector3 startPos;

    public void StartMe(PlayerControls target, int lifeForceAmount)
    {
        this.startPos = transform.position;
        this.target = target;
        this.lifeForceAmount = lifeForceAmount;
    }

    void Update() //Får fiendens själ att röra sig mot spelaren
    {
        gameObject.transform.position = Vector3.Lerp(startPos, target.transform.position, 1);
        if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 0.1f)
        {
            this.target.ReceiveLifeForce(lifeForceAmount);
            Destroy(this.gameObject);
        }
    }
}
