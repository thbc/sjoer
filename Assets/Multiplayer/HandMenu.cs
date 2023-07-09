using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HelperClasses;
using Assets.Resources;
using Assets.InfoItems;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
public class HandMenu : MonoBehaviour
{
    public Handedness handMode = Handedness.Left;
    public GameObject menuObject; // This object will appear in front of the user's palm.
    public float objectDistance = 0.2f; // This is the distance from the palm to the object.
    public float wristAngleThreshold = -0.7f;
    private bool _IsWristFacingUser;
    public bool IsWristFacingUser
    {
        get
        {
            return _IsWristFacingUser;
        }

        set
        {
            Debug.LogWarning("Activate wrist menu: " + value);
            _IsWristFacingUser = value;
            menuObject.SetActive(value); // Show the object when the palm is facing the user.
        }
    }

    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Check hand pose every 1 second

            var hand = HandJointUtils.FindHand(handMode);
            if (hand != null)
            {
                if (hand.TryGetJoint(TrackedHandJoint.Wrist, out MixedRealityPose wristPose))
                {
                    IsWristFacingUser = !(Vector3.Dot(wristPose.Up, Camera.main.transform.forward) < wristAngleThreshold);
                    // Position the object in front of the palm.
                    menuObject.transform.position = wristPose.Position + wristPose.Forward * objectDistance;
                    // Rotate the object to face the camera.
                    menuObject.transform.rotation = Quaternion.LookRotation(menuObject.transform.position - Camera.main.transform.position);
                }
                else
                {
                    IsWristFacingUser = false;
                }
            }
            else
            {
                IsWristFacingUser = false;
            }
        }
    }

    
}
