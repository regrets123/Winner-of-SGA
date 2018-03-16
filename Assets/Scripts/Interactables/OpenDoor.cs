using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject doorToOpen, leverPullPos;

    [SerializeField]
    float movePlayerSmoother;

    Animator anim, animDoor;

    PlayerControls playerToMove;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        animDoor = doorToOpen.gameObject.GetComponent<Animator>();
    }

    public void Interact(PlayerControls player)
    {
        playerToMove = player;
        StartCoroutine("MovePlayerToInteract");
        //player.gameObject.transform.position = Vector3.MoveTowards(player.gameObject.transform.position, leverPullPos.gameObject.transform.position, smooth);
        player.gameObject.transform.rotation = Quaternion.Lerp(player.gameObject.transform.rotation, leverPullPos.gameObject.transform.rotation, movePlayerSmoother);
        player.InteractTime = 6f;
        anim.SetTrigger("LeverPull");
        player.Anim.SetTrigger("PullLever");
        StartCoroutine("OpenSesame");
    }

    IEnumerator OpenSesame()
    {
        yield return new WaitForSeconds(5f);
        animDoor.SetTrigger("OpenDoor");
    }

    IEnumerator MovePlayerToInteract()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / movePlayerSmoother;
            playerToMove.gameObject.transform.position = Vector3.Lerp(playerToMove.gameObject.transform.position, leverPullPos.gameObject.transform.position, t);
            yield return null;
        }
    }
    //IEnumerator LeverPull()
    //{
    //    yield return new WaitForSeconds(0.0f);
    //}
}
