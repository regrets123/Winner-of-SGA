using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson*/

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    float sensitivityX, sensitivityY, cameraSmoothing, maximumY, minimumY;

    float rotationY, rotationX, mouseX, camDistance;

    [SerializeField]
    GameObject camBase, playerChar;

    [SerializeField]
    LayerMask camOcclusion;

    Vector3 camOffset;

    private void Start()
    {
        camDistance = Vector3.Distance(transform.position, playerChar.transform.position);
        camOffset = playerChar.transform.position - transform.position;
    }

    //Mouse sensitivity setting
    public void SetCamSensitivityX(char axis, float sensitivity)
    {
        if (axis == 'x')
        {
            this.sensitivityX = sensitivity;
        }
        else
        {
            this.sensitivityY = sensitivity;
        }
    }

    void LateUpdate()
    {
        CameraMovement();
    }

    public void CameraMovement()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        camBase.transform.localEulerAngles = new Vector3(-rotationY, 0f, 0);
        playerChar.transform.localEulerAngles = new Vector3(0f, rotationX, 0f);

        Vector3 targetCamPos = playerChar.transform.localPosition + camOffset;

        RaycastHit wallHit = new RaycastHit();
        /*
                if (Physics.Raycast(playerChar.transform.position, transform.position - playerChar.transform.position, out wallHit, camDistance, camOcclusion))
                {
                    //transform.localPosition = new Vector3(/*wallHit.point.x + wallHit.normal.x * 0.5ftransform.position.x, transform.position.y, wallHit.point.z + wallHit.normal.z * 0.5f);
                    //transform.localPosition = Vector3.Lerp(transform.position, playerChar.transform.position, cameraSmoothing);
                    transform.localPosition = new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, wallHit.point.y + wallHit.normal.y * 0.5f, wallHit.point.z + wallHit.normal.z * 0.5f);

                }
                else if (Vector3.Distance(transform.position, playerChar.transform.position) < camDistance)
                {
                    transform.localPosition = Vector3.Lerp(transform.position, playerChar.transform.position - camOffset, cameraSmoothing);
                    //transform.localPosition = Vector3.Lerp(transform.position, playerChar.transform.position - camOffset, cameraSmoothing);
                }
                /*

                if (Physics.Linecast(playerChar.transform.position, transform.position, out wallHit, camOcclusion))
                {
                    //the x and z coordinates are pushed away from the wall by hit.normal.
                    //the y coordinate stays the same.

                }
                else if (Vector3.Distance(transform.position, playerChar.transform.position) < camDistance)
                {
                    transform.position.
                }
                */



        /*
        if (Physics.Linecast(playerChar.transform.position, transform.position, out wallHit, camOcclusion))
        {
            transform.position = Vector3.Lerp(transform.position, wallHit.point, cameraSmoothing);
        }
        else if (Vector3.Distance(transform.position, playerChar.transform.position) < camDistance)
        {

        }
        */



        if (Physics.Raycast(playerChar.transform.position, transform.position - playerChar.transform.position, out wallHit, camDistance, camOcclusion))
        {
            transform.position = Vector3.Lerp(transform.position, wallHit.point, cameraSmoothing);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - playerChar.transform.position, cameraSmoothing);
        }
    }
}