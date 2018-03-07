﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*By Andreas Nilsson*/

public class CameraFollow : MonoBehaviour, IPausable
{
    [SerializeField]
    float cameraMoveSpeed, clampAngle, inputSensitivity, smoothX, smoothY, lockOnSmooth;

    [SerializeField]
    GameObject cameraFollowObj, cameraObj, playerObj;

    PauseManager pM;

    Vector3 followPOS;

    List<BaseEnemyScript> targetsLockOnAble = new List<BaseEnemyScript>();

    List<BaseEnemyScript> visibleEnemies = new List<BaseEnemyScript>();

    BaseEnemyScript targetToLockOn, lookAtMe;

    Quaternion toRotate, localRotation;

    float camDistanceXToPlayer, camDistanceYToPlayer, camDistanceZToPlayer;

    float mouseX, mouseY;

    float finalInputX, finalInputZ;

    float rotX = 0.0f, rotY = 0.0f;

    bool paused = false, lockOn = false;

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
        if (!paused && !lockOn)
        {
            //Get buttons for cameramovement from character controller, both for a console controller and PC controls
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

            localRotation = Quaternion.Euler(rotX, rotY, 0.0f);

            transform.rotation = localRotation;
        }
        else if(lockOn && !paused)
        {
        }

        CameraUpdater();

        CameraLockOn();
    }

    //Lets the camera follow a specific object, in this case and empty object attached to the character
    void CameraUpdater()
    {
        //Object for camera to follow, in this case the player character
        Transform target = cameraFollowObj.transform;

        float step = cameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    void CameraLockOn()
    {
        if (Input.GetButtonDown("LockOn") && targetsLockOnAble != null && !lockOn)
        {
            float distance;
            float closestDistance;

            visibleEnemies = new List<BaseEnemyScript>();

            foreach (BaseEnemyScript enemy in targetsLockOnAble)
            {
                if (!Physics.Linecast(transform.position, enemy.transform.position, 8))
                {
                    visibleEnemies.Add(enemy);
                }
                //else
                //{
                //    visibleEnemies.Remove(enemy);
                //}
            }

            closestDistance = targetsLockOnAble.Min(e => (e.transform.position - cameraFollowObj.transform.position).magnitude);

            foreach (BaseEnemyScript target in visibleEnemies)
            {
                distance = Vector3.Distance(cameraFollowObj.transform.position, target.transform.position);

                if (distance <= closestDistance)
                {
                    lockOn = true;
                    lookAtMe = target;
                }
            }
        }
        else if (Input.GetButtonDown("LockOn") && lockOn)
        {
            lookAtMe = null;
            //transform.rotation = Quaternion.Lerp(transform.rotation, localRotation, lockOnSmooth * Time.deltaTime);
            lockOn = false;
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f && lockOn)
        {
            visibleEnemies = new List<BaseEnemyScript>();


            foreach (BaseEnemyScript enemy in targetsLockOnAble)
            {
                if (!Physics.Linecast(transform.position, enemy.transform.position, 8))
                {
                    visibleEnemies.Add(enemy);
                }
                //else
                //{
                //    visibleEnemies.Remove(enemy);
                //}
            }
            
            for (int i = 0; i < visibleEnemies.Count; i++)
            {

                if (visibleEnemies[i] == lookAtMe)
                {
                    int newIndex;

                    if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    {
                        newIndex = i - 1;
                    }
                    else
                    {
                        newIndex = i + 1;
                    }

                    if (newIndex < 0)
                    {
                        lookAtMe = visibleEnemies[visibleEnemies.Count - 1];
                    }
                    else if (newIndex >= visibleEnemies.Count)
                    {
                        lookAtMe = visibleEnemies[0];
                    }
                    else
                    {
                        lookAtMe = visibleEnemies[newIndex];
                    }

                    break;
                }
            }
        }

        if (lookAtMe != null)
        {
            toRotate = Quaternion.LookRotation(lookAtMe.transform.position - transform.position);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRotate, lockOnSmooth*Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        targetToLockOn = other.gameObject.GetComponent<BaseEnemyScript>();

        if (targetToLockOn != null)
        {
            targetsLockOnAble.Add(targetToLockOn);
        }
    }

    void OnTriggerExit(Collider other)
    {
        targetsLockOnAble.Remove(other.gameObject.GetComponent<BaseEnemyScript>());
    }
}
