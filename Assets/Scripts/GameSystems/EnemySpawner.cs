using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//By Andreas Nilsson
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public Transform enemyPos;

    private float repeatRate = 5.0f;

    //When the player character enters the collider area enemy is spawned in specific location
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;

            Spawner();
            Destroy(gameObject);
        }
    }

    //Spawns enemy
    void Spawner()
    {
        Instantiate(enemy, enemyPos.position, enemyPos.rotation);
    }
}
