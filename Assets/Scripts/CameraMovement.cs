using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform cameraPosition;

    //bool variables
    public bool onSand;
    public bool isReachSpeedLimit;

    //speed variables
    public float speed;

    //sand jittering
    public Transform sandPosition;

    //Camera position variables
    public float offSetX;
    public float offSetY;
    public float offSetZ;

    private void Update()
    {
        if (onSand)
        {
            transform.position = Vector3.Lerp(cameraPosition.position, sandPosition.position, Mathf.PingPong(Time.time * speed, 1.0f));
        }
        else
        {
            transform.position = cameraPosition.position;
            //+ new Vector3(offSetX, offSetY, offSetZ)
        }

    }
}
