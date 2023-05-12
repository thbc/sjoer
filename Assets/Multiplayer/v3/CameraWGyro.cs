using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CameraWGyro : MonoBehaviour //NetworkBehaviour
{
   /*  void Start()
    {
        // Enable the gyroscope
        Input.gyro.enabled = true;
    }

    void Update()
    {
        // Get the gyroscope input
        Quaternion gyro = Input.gyro.attitude;
    if(gyro != null)
    {
        // Apply the rotation to the camera
        if(gyro.y != 0)
        transform.rotation = new Quaternion(transform.rotation.x, -gyro.y, transform.rotation.z, 0);
    }
    } */
   
    public void manualRotateCam()
    {
        /* if(!IsOwner) return;
        transform.Rotate(0, 10, 0); */
    }
}
