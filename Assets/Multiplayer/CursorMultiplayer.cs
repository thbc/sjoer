using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Physics;
using Unity.Mathematics;

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
        if (eventData.Controller != null && eventData.Controller.ControllerHandedness == Handedness.Right)
        {
            p = eventData.Controller.InputSource.Pointers.FirstOrDefault();
            SetupPointer();
        }
    
        
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        if (p != null && eventData.Controller != null && eventData.Controller.ControllerHandedness == Handedness.Right)
        {
            p = null;
        }
    }
    Vector3 previousPosition;
    Quaternion previousRotation;

    public Transform originTransform;


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

                //if (p.Position != previousPosition && p.Position != Vector3.zero) // Compare with the previous position -- but only send if values are not 0
                //    {
                    //RayStep rayStep = p.Rays[0];
                    //Vector3 rayOrigin = rayStep.Origin;
                    //Vector3 rayDirection = rayStep.Direction;
                    //Vector3 positionUnitsAway = rayOrigin + rayDirection * 5.0f;
                    //sender.SendCursor(positionUnitsAway);

                    ////sender.SendCursor(p.Position);
                    //previousPosition = p.Position; // Update the previous position
                                   
                        Vector3 pointerPositionRelativeToOrigin = originTransform.InverseTransformPoint(positionUnitsAway);
                        //  Debug.LogWarning("POS " + pointerPositionRelativeToOrigin); 
                        sender.SendCursorPos(pointerPositionRelativeToOrigin);
                        previousPosition = pointerPositionRelativeToOrigin;


                Vector3 originToPoint = positionUnitsAway - Vector3.zero; // this is the position of the pointer in world space
                originToPoint.Normalize(); // make it a unit vector

                float azimuth = Mathf.Atan2(originToPoint.x, originToPoint.z);
                float elevation = Mathf.Asin(originToPoint.y);

                sender.SendCursorAngles(azimuth, elevation);

                //}
                //if (p.Rotation != previousRotation)
                //{
                //sender.SendCursor(p.Rotation);
                //previousRotation = p.Rotation;


                //Quaternion pointerRotationRelativeToOrigin = Quaternion.LookRotation(originTransform.InverseTransformDirection(p.Rotation * Vector3.forward), originTransform.InverseTransformDirection(p.Rotation * Vector3.up));

                //   // Quaternion pointerRotation = Quaternion.LookRotation(rayDirection);

                //    //Quaternion pointerRotationRelativeToOrigin = Quaternion.LookRotation(originTransform.InverseTransformDirection(pointerRotation * Vector3.forward), originTransform.InverseTransformDirection(pointerRotation * Vector3.up));
                //        // Debug.LogWarning("ROT " + pointerRotationRelativeToOrigin);
                //        sender.SendCursorRot(pointerRotationRelativeToOrigin); 
                //        previousRotation =pointerRotationRelativeToOrigin;
                    //}
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
        // this is for only right hand tracking

        if (newPointer != null
            && newPointer.Controller != null
            && newPointer.Controller.ControllerHandedness == Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right)
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



