using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;

public class poio : MonoBehaviour, IMixedRealitySourceStateHandler
{
    public OSCSender sender;
   // public GameObject CursorHighlight;
    // Vector3 cursePos = new Vector3();
    IMixedRealityPointer p;
    //private void OnEnable()
    //{
    //    CoreServices.InputSystem?.FocusProvider?.SubscribeToPrimaryPointerChanged(OnPrimaryPointerChanged, true);

    //}

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
    //private void FixedUpdate()
    //{
    //    if (p != null && !p.IsFocusLocked) // trying out IsFocusLocked for now
    //    {

    //        if (mainCursorTransf != null)
    //        {
    //            CursorHighlight.transform.position = p.Position;// + Vector3.forward; // mainCursorTransf.position;
    //            CursorHighlight.transform.rotation = p.Rotation;// mainCursorTransf.rotation;

    //            if (sender != null)
    //            {
    //                if (mainCursorTransf.position != previousPosition && mainCursorTransf.position != Vector3.zero) // Compare with the previous position -- but only send if values are not 0
    //                {
    //                    sender.SendCursor(CursorHighlight.transform.position);

    //                    previousPosition = mainCursorTransf.position; // Update the previous position
    //                }
    //                if (mainCursorTransf.rotation != previousRotation) // Compare with the previous rot
    //                {
    //                    sender.SendCursor(CursorHighlight.transform.rotation);

    //                    previousRotation = mainCursorTransf.rotation; // Update the previous rot
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogWarning("sender not defined");
    //            }
    //            Debug.Log(CursorHighlight.name + "\n" +
    //                      CursorHighlight.transform.position.x + " " + CursorHighlight.transform.position.y + " " + CursorHighlight.transform.position.z);

    //        }
    //    }
    //}
    private void FixedUpdate()
    {
        if (p != null && !p.IsFocusLocked) // trying out IsFocusLocked for now
        {
                if (sender != null)
                {
                    if (p.Position != previousPosition && p.Position != Vector3.zero) // Compare with the previous position -- but only send if values are not 0
                    {
                        sender.SendCursor(p.Position);

                        previousPosition = p.Position; // Update the previous position
                    }
                    if (p.Rotation != previousRotation) // Compare with the previous rot
                    {
                        sender.SendCursor(p.Rotation);

                        previousRotation = p.Rotation; // Update the previous rot
                    }
                }
                else
                {
                    Debug.LogWarning("sender not defined");
                }            
        }
    }
    //Transform mainCursorTransf;
    void SetupPointer()
    {
        //mainCursorTransf = p.BaseCursor.TryGetMonoBehaviour(out MonoBehaviour baseCursor) ? baseCursor.transform : null;
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

        // this is for both hands tracking 
        //if (newPointer != null && newPointer is InputSystemGlobalHandlerListener)//newPointer.InputSourceParent.SourceType == InputSourceType.Hand)  // is InputSystemGlobalHandlerListener)
        //{
       //     p = newPointer;
       //     SetupPointer();
       // }

    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.FocusProvider?.UnsubscribeFromPrimaryPointerChanged(OnPrimaryPointerChanged);
        OnPrimaryPointerChanged(null, null);
    }
}



