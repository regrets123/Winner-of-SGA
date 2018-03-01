using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson*/

public class CameraFollow : MonoBehaviour, IPausable
{
    [SerializeField]
    float cameraMoveSpeed, clampAngle, inputSensitivity, smoothX, smoothY;

    [SerializeField]
    GameObject cameraFollowObj, cameraObj, playerObj;

    PauseManager pM;

    Vector3 followPOS;

    float camDistanceXToPlayer;
    float camDistanceYToPlayer;
    float camDistanceZToPlayer;
    float mouseX;
    float mouseY;
    float finalInputX;
    float finalInputZ;
    float rotX = 0.0f;
    float rotY = 0.0f;

    bool paused = false;

    // Use this for initialization
    void Start()
    {
        pM = FindObjectOfType<PauseManager>();
        pM.Pausables.Add(this);
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
    }

    public void PauseMe(bool pausing)
    {
        paused = pausing;
    }
    
    void LateUpdate()
    {
        if (!paused)
        {
            //Get buttons for cameramovement from character controller
            float inputX = Input.GetAxis("RightStickHorizontal");
            float inputZ = Input.GetAxis("RightStickVertical");
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            finalInputX = inputX + mouseX;
            finalInputZ = inputZ + mouseY;

            rotY += finalInputX * inputSensitivity * Time.deltaTime;
            rotX += finalInputZ * inputSensitivity * Time.deltaTime;

            //clamps the x rotation so camera isn't able to spin around under the character
            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);

            transform.rotation = localRotation;
        }
        CameraUpdater();
    }

    //Lets the camera follow a specific object, in this case and empty object attached to the character
    void CameraUpdater()
    {
        //Object for camera to follow, in this case the player character
        Transform target = cameraFollowObj.transform;

        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
