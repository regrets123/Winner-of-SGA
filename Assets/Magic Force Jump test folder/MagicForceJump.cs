using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicForceJump : MonoBehaviour
{
    [SerializeField]
    public GameObject magicJumpPrefab;
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public GameObject spawnPos;

    //A currently public float, 
    //so you can adjust the speed as necessary.
    public float speed = 20.0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            playerPrefab.GetComponent<Rigidbody>().AddForce(playerPrefab.transform.up * speed, ForceMode.Impulse);

            //instantiate a magic jump circle
            GameObject magicJump = Instantiate(magicJumpPrefab, spawnPos.transform.position, spawnPos.transform.rotation);

            //Add a force to the player going up form your current position.

            Destroy(magicJump, 1.5f);
        }
    }
}
