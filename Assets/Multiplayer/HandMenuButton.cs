using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;
using System;

public class HandMenuButton : MonoBehaviour
{
    public Material inactiveButtonMat;
    public Material activeButtonMat;
    private bool _buttonActive;

    public MeshRenderer buttonPlane;
    public TextMesh buttonLabel;
    public bool buttonActive
    {
        get
        {
            return _buttonActive;
        }
        set
        {
            _buttonActive = value;
            if (value == true)
            {
              // buttonPlane.material = activeButtonMat;
                buttonLabel.text = "Deactivate Marking";
            }
            else
            {
              //  buttonPlane.material   = inactiveButtonMat;
                buttonLabel.text = "Activate Marking";
            }

        }
    }

    public enum ButtonType
    {
        MarkMode
    }
    public ButtonType buttonType = ButtonType.MarkMode;

    public void Clicked()
    {
        Debug.LogWarning("Object was tapped");

        switch (buttonType)
        {

            case ButtonType.MarkMode:
                buttonActive = Marker.Instance.SetMarkMode();
                Debug.LogWarning("MARKMODE: " + buttonActive);
                break;
            default:
                break;
        }

        
    }
    
  

    
}
