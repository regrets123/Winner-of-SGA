using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Björn Andersson*/

public class PlayerResources : MonoBehaviour {

    [SerializeField]
    int maxHealth;

    [SerializeField]
    float maxStamina;

    int health;

    float stamina;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        //death animation och reload last saved state
    }

    private void Update()
    {
        if(Input.GetAxis("Sprint") > 0f)
        {
            stamina -= 1;
        }
        else if (stamina < maxStamina)
        {
            stamina += 1;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        }
    }
}