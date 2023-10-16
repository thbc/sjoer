using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HelperClasses;
using Assets.Resources;
using Assets.InfoItems;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class EyeTrackingMode : MonoBehaviour
{
    public static EyeTrackingMode Instance { get; private set; }

    [HideInInspector]
    public bool isEyeTracking = true; // on start this is set to false


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
       //     DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Invoke("DisableOnStart_Delayed", 5f);
    }
    private void DisableOnStart_Delayed()
    {
        DisableEyeTracking();
        isEyeTracking = false;
    }

    public bool SetEyeTrackingMode()
    {
        isEyeTracking = !isEyeTracking;
        Debug.Log("gaze tracking: " + isEyeTracking);
        if (isEyeTracking)
            EnableEyeTracking();
        else
            DisableEyeTracking();
        return isEyeTracking;
    }
    // GazeTracking Mode
    void EnableEyeTracking()
    {
        if (CoreServices.InputSystem != null && CoreServices.InputSystem.EyeGazeProvider != null)
        {
            CoreServices.InputSystem.EyeGazeProvider.Enabled = true;
        }
    }

    void DisableEyeTracking()
    {
        if (CoreServices.InputSystem != null && CoreServices.InputSystem.EyeGazeProvider != null)
        {
            CoreServices.InputSystem.EyeGazeProvider.Enabled = false;
        }
    }

}
