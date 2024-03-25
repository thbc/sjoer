`VesselMode`: are we running the application from a real world vessel?

`LatitudeArea` & `LongitudeArea`: dimensions for bounding box that determines area size (in meters) for retrieving ship vessel information from API; default value 3000 m

`UIElementHeight`, `SkyAreaHeightTarget`, `SkyAreaHeightHover`, `OnLookAwayDisappearDelay`, `MaxRulerDistance`, `NumItemsOnHover`: Hardcoded values for visualization and interaction of vessel pins

`VesselName`: Name of the vessel you are currently on, if we are running the app on a real vessel; determines which vessel pin to skip in visualization

`BridgeHeight`: What's the height of your bridge? Can be used for offsetting the y-position of pins; default is 0

`Latitude`, `Longitude`, `Latitude_home`, `Longitude_home`: test values for development when not connected to GPS

`SceneInterval`: does not seem to be used  

`CalibrationSettings`: calibration settings..

`HorizonPlaneRadius`: radius around player for placing pins along horizon

`PhoneGPS IP/port`: connection settings for receiving GPS position from Android App; will most likely change during development; can be set from handbased UI

`Nightmode`: when enabled, adjusts the brightness of the Mixed Reality scene