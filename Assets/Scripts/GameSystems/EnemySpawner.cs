using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//By Andreas Nilsson
public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    List<GameObject> enemiesToSpawn;
    [SerializeField]
    List<Transform> spawnPositions;
    [SerializeField]
    float respawnTime;

    //When the player character enters the collider area enemy is spawned in specific location
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Spawner();

            StartCoroutine("Respawn");
        }
    }

    //Spawns enemy
    void Spawner()
    {
        foreach (GameObject enemy in enemiesToSpawn)
        {
            foreach (Transform enemyPos in spawnPositions)
            {
                Instantiate(enemy, enemyPos.position, enemyPos.rotation);
            }
        }
    }

    private IEnumerator Respawn()
    {
        gameObject.GetComponent<Collider>().enabled = !gameObject.GetComponent<Collider>().enabled;

        yield return new WaitForSeconds(respawnTime);

        gameObject.GetComponent<Collider>().enabled = gameObject.GetComponent<Collider>().enabled = true;
    }
}
