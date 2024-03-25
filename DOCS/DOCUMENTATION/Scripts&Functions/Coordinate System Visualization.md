# Coordinate System Visualization

This is just a little visualization helper for Transform positions and rotations.

- *ControllerCoordRendering* controls the visualization by holding a reference list with *CoordinatesRenderer.*
- *CoordinatesRenderer* draws lines from its referenced Transform position. If the same GameObject has also a *PlayerCoordinates* component, this *PlayerCoordinates' playerCam* will be used for setting the rendering position, and the *Unity2TrueNorth* Quaternion will set the rotation of the coordinates rendering.  
    You can change the ray length, the line's material and colour and its name label. Make sure to assign the Transform when not visualizing the player's coordinates.

&nbsp;

The visualization can be turned on from HandMenu-UI