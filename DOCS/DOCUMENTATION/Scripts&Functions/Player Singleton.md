This class is designed to interact with GPS data and calibrate a virtual environment to align with real-world coordinates. It offers functionalities for debugging, camera management, and data logging.

- GPS Data Retrieval and Management:
    
    - Initializes and utilizes [DataRetriever](/C:/Program%20Files/Joplin/resources/app.asar/%5BDataRetriever%5D%28:/b2567088f37b4418a4ce02573bb2a6ab%29 "%5BDataRetriever%5D(:/b2567088f37b4418a4ce02573bb2a6ab)") to fetch GPS data: gpsRetriever = new DataRetriever(DataConnections.PhoneGPS, DataAdapters.GPSInfo, ParameterExtractors.None, this); 
    - Stores the latest GPS update in \`lastGPSUpdate\`.
    - Implements \`updateGPS()\` method for updating GPS data asynchronously.
- Debugging:
    
    - `public bool debugVesselInfoInEditor` controlls whether API retrieved vessel info should be printed to the console in the Editor.
    - implements [DebugOnHead](/C:/Program%20Files/Joplin/resources/app.asar/%5BDebugOnHead%5D%28:/18bb9d05af4a499bb7934d19477e61fd%29 "%5BDebugOnHead%5D(:/18bb9d05af4a499bb7934d19477e61fd)") script with received GPS data, if enabled from GUI
- Calibration and Orientation:
    
    - Calculates the difference between the vessel's true north bearing and the Unity coordinate system's bearing.
    - Offers calibration functionalities to align the virtual environment with real-world coordinates and in relation to GPS data.
- Positional Transformations:
    
    - Converts GPS coordinates to Unity world coordinates.
    - Includes methods for calculating the player's current position and the surrounding area in latitudinal and longitudinal terms.
- Camera and Lighting Control:
    
    - Manages the main camera and light intensity settings; applies night mode.

&nbsp;

- Data Logging and Representation:
    - Includes GetLogData() to collect and structure player data, including position, rotation, and GPS information. \[See here for more information on Data Logs\]([Log Data](:/1602e63036f84feb86bcae1d46f3c987))

&nbsp;