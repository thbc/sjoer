In the latest version of Sjør, the initial extra calibration scene was merged into the main scene. Multiple calibrations during one session (starting the app until closing it) can be a bit buggy. Therefore, in the current state we recommend to avoid calibration if possible, but instead making sure that phone and headset are aligned and facing north when starting the app.

To re-implement 2-scene calibration you have to activate the GameObject "*SpeechInputHandler-Original*" in the Default-scene and deactivate the *SpeechInputHandler-New*. You will also need to make sure that all references of the main-scene are kept when changing between main- and calibration-scene. Also, make sure that the calibration scen has been added and enabled in the Build Settings "Scenes in Build".

&nbsp;