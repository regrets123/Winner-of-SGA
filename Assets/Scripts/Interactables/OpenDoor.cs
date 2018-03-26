using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject doorToOpen, leverPullPos;

    [SerializeField]
    float movePlayerSmoother;

    float leverPullPosX;

    Animator anim, animDoor;

    PlayerControls playerToMove;

    MovementType previousMovement;


    void Start()
    {
        anim = this.GetComponent<Animator>();
        animDoor = doorToOpen.gameObject.GetComponent<Animator>();
    }

    public void Interact(PlayerControls player)
    {
        playerToMove = player;
        StartCoroutine("MovePlayerToInteract");
        player.InteractTime = 5.13f;
        anim.SetTrigger("LeverPull");
        playerToMove.Anim.SetTrigger("OpenDoor");
        StartCoroutine("OpenSesame");
    }

    IEnumerator OpenSesame()
    {
        yield return new WaitForSeconds(5.0f);
        animDoor.SetTrigger("OpenDoor");
    }

    IEnumerator MovePlayerToInteract()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / movePlayerSmoother;
        //playerToMove.Anim.SetFloat("Speed", 0.5f);

            playerToMove.gameObject.transform.position = Vector3.Lerp(playerToMove.gameObject.transform.position, leverPullPos.gameObject.transform.position, t);
            playerToMove.gameObject.transform.rotation = Quaternion.Lerp(playerToMove.gameObject.transform.rotation, leverPullPos.gameObject.transform.rotation, t);

            yield return null;
        }

    }
}
