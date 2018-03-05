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
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            Instantiate(enemiesToSpawn[i], spawnPositions[i].position, spawnPositions[i].rotation);
        }
    }

    private IEnumerator Respawn()
    {
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(respawnTime);

        GetComponent<Collider>().enabled = true;
    }
}
