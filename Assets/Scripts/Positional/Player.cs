using Assets.DataManagement;
using Assets.HelperClasses;
using System;
using UnityEngine;
using Assets.Resources;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.SceneManagement;
namespace Assets.Positional
{
    public class Player : MonoSingleton<Player>
    {
        public bool debugVesselInfoInEditor = true;
        public Camera mainCamera;

        // Difference between vessel bearing (true north) and Hololens bearing (unity coordinates)
        private Quaternion unityToTrueNorthRotation = Quaternion.identity;
        private AISDTO lastGPSUpdate;
        private DataRetriever gpsRetriever;
        Timer GPSTimer;

        private float CalibrationHDGVessel = 0;
        private float CalibrationHDGHolo = 0;
        private float CalibrationDiff = 0;

        private async void updateGPS()
        {
            lastGPSUpdate = (AISDTO)await gpsRetriever.fetch();

      //      Debug.Log("Lat: " + lastGPSUpdate.Latitude + "Lon: " + lastGPSUpdate.Longitude + "Heading: " + lastGPSUpdate.Heading + "SOG: " + lastGPSUpdate.SOG);
            if (lastGPSUpdate != null && lastGPSUpdate.Valid)
            {
                if (DebugOnHead.Instance != null)
                {
                    if (DebugOnHead.Instance.gameObject.activeInHierarchy)
                    {
                        DebugOnHead.Instance.DebugTextOnHead_2("updateGPS: " +
                    "Lat: " + lastGPSUpdate.Latitude +
                    "Lon: " + lastGPSUpdate.Longitude +
                    "Heading: " + lastGPSUpdate.Heading +
                    "SOG: " + lastGPSUpdate.SOG);
                    }
                }
            }
            else
            {
                if (DebugOnHead.Instance != null)
                {
                    if (DebugOnHead.Instance.gameObject.activeInHierarchy)
                    {
                        DebugOnHead.Instance.DebugTextOnHead_2("GPS data invalid or null: " +
                        "Lat: " + lastGPSUpdate.Latitude +
                        "Lon: " + lastGPSUpdate.Longitude +
                        "Heading: " + lastGPSUpdate.Heading +
                        "SOG: " + lastGPSUpdate.SOG);
                    }
                }
            }

            unitytoTrueNorth();
#if UNITY_EDITOR
            //this was added to keep the console clean.
            if (this.debugVesselInfoInEditor)
            {
                Debug.Log("Heading: " + lastGPSUpdate.Heading);
                Debug.Log("SOG: " + lastGPSUpdate.SOG);
                Debug.Log("Lat: " + lastGPSUpdate.Latitude);
                Debug.Log("Lon: " + lastGPSUpdate.Longitude);
            }
#endif

        }

        public Vector2 GetLatLon
        {
            get { return lastGPSUpdate != null ? new Vector2((float)lastGPSUpdate.Latitude, (float)lastGPSUpdate.Longitude) : new Vector2(0, 0); }
        }

        public Double Heading
        {
            get { return lastGPSUpdate != null ? lastGPSUpdate.Heading : 0; }
        }

        public Double SOG
        {
            get { return lastGPSUpdate != null ? lastGPSUpdate.SOG : 0; }
        }

        public Quaternion Unity2TrueNorth
        {
            get { return unityToTrueNorthRotation; }
        }

        // Start is called before the first frame update
        void Start()
        {
            HandleDuplicatePlayersInScene();

            InitializeGPSRetriever();
            /* gpsRetriever = new DataRetriever(DataConnections.PhoneGPS, DataAdapters.GPSInfo, ParameterExtractors.None, this);
            GPSTimer = new Timer(0.5f, updateGPS);
            SetLightIntensity(); */
        }

        private void HandleDuplicatePlayersInScene()
        {
            Player[] foundPlayers = FindObjectsOfType<Player>();

            // If no player or only one player is found, no need for further processing.
            if (foundPlayers.Length <= 1) return;

            GameObject gameObjToDelete = DetermineDuplicatePlayer(foundPlayers);

            if (gameObjToDelete != null)
            {
                Destroy(gameObjToDelete);
                Debug.Log("Destroyed duplicate player object in the initial scene.");
            }
        }

        private GameObject DetermineDuplicatePlayer(Player[] players)
        {
            GameObject gameObjToDelete = null;

            foreach (Player player in players)
            {
                Scene playerScene = player.gameObject.scene;

                Debug.LogFormat("{0} in scene: {1}", player.gameObject.name, playerScene.name);

                // Assuming '0' is the build index of your initial scene. Consider using scene name for clarity.
                if (playerScene.buildIndex == 0)
                {
                    Debug.LogFormat("Marked for deletion: {0} in scene: {1}", player.gameObject.name, playerScene.name);
                    gameObjToDelete = player.gameObject;
                }
            }

            return gameObjToDelete;
        }

        public void InitializeGPSRetriever()
        {
            gpsRetriever = new DataRetriever(DataConnections.PhoneGPS, DataAdapters.GPSInfo, ParameterExtractors.None, this);
            GPSTimer = new Timer(0.5f, updateGPS);
            SetLightIntensity();
        }

        public Camera GetMainCamera()
        {
            EnsureMainCamera();
            return mainCamera;
        }

        // Black becomes transparent on HoloLens (and so do shadows)
        // Therefore, this code ensures that the scene is brightly lit
        public void SetLightIntensity(int value = 30)
        {
            Light playerLight = mainCamera.transform.GetChild(0).gameObject.GetComponent<Light>();
            playerLight.range = (float)Config.Instance.conf.UISettings["HorizonPlaneRadius"] + 1;
            playerLight.intensity = value;
        }

        public void EnsureMainCamera()
        {
            if (!mainCamera)
            {
                mainCamera = (Camera)GameObject.Find("MixedRealityPlayspace").transform.GetChild(0).gameObject.GetComponent<Camera>();
                SetLightIntensity();
            }
        }

        // Update is called once per frame
        void Update()
        {
            EnsureMainCamera();

            //  Temporary not insanely swift GPS update hack
            if (!gpsRetriever.isConnected() || GPSTimer.hasFinished())
            {
                GPSTimer.restart();
            }
            GPSTimer.Update();

            // The regular code
            //if (dataRetriever.isConnected())
            //{
            //    updateGPS();
            //}

        }

        private void OnDestroy()
        {
            gpsRetriever.OnDestroy();
        }

        public Vector3 GetWorldTransform(double lat, double lon)
        {
            double x, y, z;
            if (lastGPSUpdate != null && lastGPSUpdate.Valid)
            {
                GPSUtils.Instance.GeodeticToEnu(
                    lat, lon, 0,
                    lastGPSUpdate.Latitude, lastGPSUpdate.Longitude, 0,
                    out x, out y, out z
                );
            }
            else
            {
                GPSUtils.Instance.GeodeticToEnu(
                    lat, lon, 0,
                    Config.Instance.conf.NonVesselSettings["Latitude"], Config.Instance.conf.NonVesselSettings["Longitude"], 0,
                    out x, out y, out z
                );
            }

            Vector3 newPos = unityToTrueNorthRotation * (new Vector3((float)x, (float)z, (float)y) - mainCamera.transform.position) + mainCamera.transform.position;

            return newPos;

            //return mainCamera.transform.RotateAround(mainCamera.transform.position, mainCamera.transform.position + new Vector3((float)x, (float)z, (float)y), unityToTrueNorthRotation.y);
        }

        public Tuple<Vector2, Vector2> GetCurrentLatLonArea()
        {
            double lat, lon;

            // Use the harcoded values if the GPS reading is invalid
            if (lastGPSUpdate != null && lastGPSUpdate.Valid)
            {
                lat = lastGPSUpdate.Latitude;
                lon = lastGPSUpdate.Longitude;
            }
            else
            {
#if UNITY_EDITOR
                //this was added to keep the console clean.
                if (this.debugVesselInfoInEditor)
                    Debug.Log("Invalid GPS. Using default.");
#endif
                lat = Config.Instance.conf.NonVesselSettings["Latitude"];
                lon = Config.Instance.conf.NonVesselSettings["Longitude"];
            }

            return HelperClasses.GPSUtils.Instance.GetCurrentLatLonArea(
                    lat,
                    lon
                );
        }


        // The rotation that transforms the Unity north axis to True north
        // This should only be executed when Hololens and vessel are aligned
        // (and thus vessel compass information and Hololens direction match)

        #region new added, not part of original script
        Camera calibrationCam;
        /*  public void PrepareCalibration(Camera _calibCam)
         {
             calibrationCam = _calibCam;
             mainCamera = calibrationCam;
             SetLightIntensity();
         }
         public void FinishCalibration()
         {
             mainCamera = null;
             this.EnsureMainCamera();
         } */


        #endregion
        public void calibrate()
        {
            this.unitytoTrueNorth(true);
        }
        private void unitytoTrueNorth(bool calibrate = false)
        {
            if (calibrate)
            {
                CalibrationHDGHolo = mainCamera.transform.rotation.eulerAngles.y;
                CalibrationHDGVessel = lastGPSUpdate != null && lastGPSUpdate.Valid ? (float)lastGPSUpdate.Heading : 0;
                CalibrationDiff = CalibrationHDGVessel - CalibrationHDGHolo;
            }

            float CurrentHDGVessel = lastGPSUpdate != null && lastGPSUpdate.Valid ? (float)lastGPSUpdate.Heading : 0;
            float UpdateDiff = CurrentHDGVessel - CalibrationHDGVessel;

            unityToTrueNorthRotation = Quaternion.Euler(0, -(CalibrationDiff + UpdateDiff), 0);

//            Debug.Log("Unity to true north: " + unityToTrueNorthRotation);

        }
    }

}
