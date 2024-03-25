using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Positional;
public class ControllerCoordRendering : MonoBehaviour
{
    public List<CoordinatesRenderer> renderedCoordinates = new List<CoordinatesRenderer>();

    public void ResetOnCalibrate()
    {
        DisplayRendering(false);
        DisplayRendering(true);
    }

    public void DisplayRendering(bool state)
    {

        if (state)
        {
            for (int i = 0; i < renderedCoordinates.Count; i++)
            {
                var renderedCoordinate = renderedCoordinates[i];

                renderedCoordinate.enabled = true;
            }
        } else if(!state)
        {
            for (int i = 0; i < renderedCoordinates.Count; i++)
            {
                var renderedCoordinate = renderedCoordinates[i];

                renderedCoordinate.enabled = false;
            }
        }

    }
   

}
