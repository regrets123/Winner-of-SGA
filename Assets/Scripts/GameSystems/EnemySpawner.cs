using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Arbetat på av Andreas Nilsson
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public Transform enemyPos1;
    public Transform enemyPos2;
    public Transform enemyPos3;
    public Transform enemyPos4;
    public Transform enemyPos5;


    private float repeatRate = 5.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            InvokeRepeating("EnemySpawner", 0.5f, repeatRate);
            Destroy(gameObject, 11);
            gameObject.GetComponent<BoxCollider>().enabled = false;

            Spawner1();
            Spawner2();
            Spawner3();
            Spawner4();
            Spawner5();
        }
    }

    void Spawner1()
    {
        Instantiate(enemy1, enemyPos1.position, enemyPos1.rotation);
    }

    void Spawner2()
    {
        Instantiate(enemy1, enemyPos2.position, enemyPos2.rotation);
    }

    void Spawner3()
    {
        Instantiate(enemy2, enemyPos3.position, enemyPos3.rotation);
    }

    void Spawner4()
    {
        Instantiate(enemy2, enemyPos4.position, enemyPos4.rotation);
    }

    void Spawner5()
    {
        Instantiate(enemy3, enemyPos5.position, enemyPos5.rotation);
    }
}
