The Unity project implements a Mixed Reality visualization of realtime AIS ship vessel data in the Barents Sea.

It was produced during two main programming cycles.

The first from [Imable](https://github.com/Imable/sjoer) created the foundation of the HoloLens implementation, for retrieving API data and displaying data in real-time. It uses the GPS position data streamed from a connected phone via a locally hosted server (on the phone) to update the Unity positions of the vessel object correctly matched to the player position. A substantial part of development has established geographically correct conversions and performance optimized C# class structures.

An additional calibration scene calibrates the main scene based on the offset between player orientation and real world north orientation.

The user can either select vessel icons (pins) with their hand or by gaze in order to show an extended info field with additional information about the vessel.

<img src="../_resources/001.gif" alt="001.gif" width="749" height="313" class="jop-noMdConv">

In this version, graphics are loaded from the *Resources* folder and instantiated in a complex [factory based system (see more)](../DOCUMENTATION/Scripts&Functions/factory%20based%20system.md).

The other cycle by ==thbc== focused on implementing multiplayer functionality and interacting with settings on a user interface (UI). Although some parts of the original project had to be adapted, this can mainly be seen as another layer on top of the base project. Some approaches are done differently to the first part and the scripts are rather attached to GameObjects within the scene than instantiated from others on the fly.

When two HoloLens HMDs (Head Mounted Displays) are connected to the same WIFI network the devices send out data to each other via OSC, after entering each IP addresses. This will show a cursor from the other players hand pointer. If MarkMode is enabled on both devices, one can mark vessel pins for the player, and there will be different colours for sent markers, received markers as well as sent-and-received markers.

Debug information can be enabled from the UI and print e.g. received GPS data string or visualize coordinate systems for predefined Transforms (helpful for debugging position and orientation). An additional setting allows also to create log files during runtime and to store them in the path *"Application.persistentDataPath + "/logs/"*.

A recent addition to this project brought data about static navigation aids (fixed marks such as poles, lights, and more) into the project.

A config file allows parameters to be changed during development process for testing out the different data, vessel, calibration and connection settings. To allow editing of certain parameters during runtime, without requiring re-exporting, some can be overwritten in settings on the hand based user interface. These will be stored to the Unity PlayerPrefs and persist only on this device, until reinstalling the app. Parameters that can be edited from the UI are e.g. the vessel's bridge height, multiplayer and GPS connection settings, the API's retrieved area size, the vessel name of the current ship and more.

&nbsp;

* * *

[Getting started](DOCS/DOCUMENTATION/001_GettingStarted.md.md)
