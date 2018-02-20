using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    [SerializeField]
    float sensitivityX = 5.0f;

    [SerializeField]
    float sensitivityY = 5.0f;

    public float minimumY = -30.0f;
    public float maximumY = 30.0f;

    float rotationY = 0.0f;
    float rotationX = 0.0f;

    float mouseX = 0.0f;

    [SerializeField]
    GameObject camBase, playerChar;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        CameraMovement();
    }

    public void CameraMovement()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        camBase.transform.localEulerAngles = new Vector3(-rotationY, 0f, 0);
        playerChar.transform.localEulerAngles = new Vector3(0f, -rotationX, 0f);
    }
}
