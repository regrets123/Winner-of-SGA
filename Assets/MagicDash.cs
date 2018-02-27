using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDash : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;

    Vector3 dashVelocity;
    Vector3 move;

    bool dashing = false;

    void Start()
    {
        move = playerPrefab.GetComponent<PlayerControls>().move;
    }

    void Update()
    {
        DashMove();
    }

    void DashMove()
    {
        if (Input.GetButtonDown("Dash"))
        {
            StartCoroutine("Dash");
        }

        if (dashing)
        {
            dashVelocity = move * 2;

            move += dashVelocity;
        }
    }

    IEnumerator Dash()
    {
        dashing = true;
        yield return new WaitForSeconds(0.2f);
        dashing = false;
    }
}
