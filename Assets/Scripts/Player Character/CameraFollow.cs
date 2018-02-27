using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*By Andreas Nilsson*/

public class CameraFollow : MonoBehaviour, IPausable
{

    public float cameraMoveSpeed = 120.0f;
    public GameObject cameraFollowObj;
    Vector3 followPOS;
    public float clampAngle = 60.0f;
    public float inputSensitivity = 150.0f;
    public GameObject cameraObj;
    public GameObject playerObj;
    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistanceZToPlayer;
    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputZ;
    public float smoothX;
    public float smoothY;
    public float rotX = 0.0f;
    public float rotY = 0.0f;
    PauseManager pM;
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

    // Update is called once per frame
    void Update()
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

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);

            transform.rotation = localRotation;
        }
    }

    void LateUpdate()
    {
        CameraUpdater();
    }

    //Lets the camera follow a specific object, in this case and empty object attached to the character
    void CameraUpdater()
    {
        Transform target = cameraFollowObj.transform;

        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
