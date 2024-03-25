using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Physics;

using Assets.Positional;


public class CursorMultiplayer : MonoBehaviour, IMixedRealitySourceStateHandler
{
    public OSCSender sender;
   
    IMixedRealityPointer p;
  

    private void OnEnable()
    {
        // Subscribe to input source events
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        if (eventData.Controller != null && eventData.Controller.ControllerHandedness == Handedness.Left)
        {
            p = eventData.Controller.InputSource.Pointers.FirstOrDefault();
            SetupPointer();
        }
    
        
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        if (p != null && eventData.Controller != null && eventData.Controller.ControllerHandedness == Handedness.Left)
        {
            p = null;
        }
    }
    Vector3 previousPosition;
    Quaternion previousRotation;

   // public Transform playerTransform;

    public Player player;


    public Transform playerCoordinatesTransform;

    private void FixedUpdate()
    {
        if (p != null && !p.IsFocusLocked) // trying out IsFocusLocked for now
        {
               
            if (sender != null)
            {                       
                    RayStep rayStep = p.Rays[0];
                    Vector3 rayOrigin = rayStep.Origin;
                    Vector3 rayDirection = rayStep.Direction;
                    Vector3 positionUnitsAway = rayOrigin + rayDirection * 5.0f;


                    Vector3 obj = positionUnitsAway;
                    float distance = Vector3.Distance(obj, playerCoordinatesTransform.transform.position);

                    // Get a vector from player to object
                    Vector3 dir = (obj - playerCoordinatesTransform.transform.position).normalized;

                    // Calculate azimuth and elevation
                    float azimuth = (Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);
                    if (azimuth < 0) azimuth += 360; // Ensure azimuth is between 0 and 360

                    float elevation = Mathf.Asin(dir.y) * Mathf.Rad2Deg;

                    // Convert angles from degrees to radians
                    float azimuthRad = azimuth * Mathf.Deg2Rad;
                    float elevationRad = elevation * Mathf.Deg2Rad;

                    // Convert spherical coordinates to Cartesian coordinates
                    float x = distance * Mathf.Cos(elevationRad) * Mathf.Sin(azimuthRad);
                    float y = distance * Mathf.Sin(elevationRad);
                    float z = distance * Mathf.Cos(elevationRad) * Mathf.Cos(azimuthRad);

                    sender.SendCursorPos(x,y, z);
                
            }
            else
                {
                    Debug.LogWarning("sender not defined");
                }            
        }
    }
    void SetupPointer()
    {
        Debug.LogWarning("setup pointer");
    }

    private void OnPrimaryPointerChanged(IMixedRealityPointer oldPointer, IMixedRealityPointer newPointer)
    {
        // this is for only Left hand tracking

        if (newPointer != null
            && newPointer.Controller != null
            && newPointer.Controller.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left)
        {
            p = newPointer;
            SetupPointer();
        }
        else
        {
            p = null;
        }

    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.FocusProvider?.UnsubscribeFromPrimaryPointerChanged(OnPrimaryPointerChanged);
        OnPrimaryPointerChanged(null, null);
    }
}



