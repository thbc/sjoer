using Assets.DataManagement;
using Assets.HelperClasses;
using System;
using UnityEngine;
using Assets.Resources;
using Microsoft.MixedReality.Toolkit;

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
        /// <summary>
        /// This is subscribed to the onComplete Action of Timer and therefore invoked when "GPSTimer.Update();" is done.
        /// </summary>
        private async void updateGPS()
        {
            lastGPSUpdate = (AISDTO)await gpsRetriever.fetch();

            Debug.Log("Lat: " + lastGPSUpdate.Latitude + "Lon: " + lastGPSUpdate.Longitude + "Heading: " + lastGPSUpdate.Heading + "SOG: " + lastGPSUpdate.SOG);
            if (lastGPSUpdate != null && lastGPSUpdate.Valid)
            {
                if (DebugOnHead.Instance.gameObject.activeInHierarchy)
                {
                    DebugOnHead.Instance.DebugTextOnHead_2("updateGPS: " + "Lat: " + lastGPSUpdate.Latitude + "Lon: " + lastGPSUpdate.Longitude + "Heading: " + lastGPSUpdate.Heading + "SOG: " + lastGPSUpdate.SOG);
                }
            }
            else
            {
                if (DebugOnHead.Instance.gameObject.activeInHierarchy)
                {
                    DebugOnHead.Instance.DebugTextOnHead_2("GPS data invalid or null: " + "Lat: " + lastGPSUpdate.Latitude + "Lon: " + lastGPSUpdate.Longitude + "Heading: " + lastGPSUpdate.Heading + "SOG: " + lastGPSUpdate.SOG);
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

            //here we want to check if the position changed between the previous GPSUpdate and the current one:
            if (lastGPSUpdate.Latitude != previousGPSLat || lastGPSUpdate.Longitude != previousGPSLon)
            {
                OnPlayerPositionChanged?.Invoke();
                previousGPSLon = lastGPSUpdate.Longitude;
                previousGPSLat = lastGPSUpdate.Latitude;
            }

        }
        public Action OnPlayerPositionChanged;
        double previousGPSLat;
        double previousGPSLon;

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

        void Start()
        {
            InitializeGPSRetriever();
            /* gpsRetriever = new DataRetriever(DataConnections.PhoneGPS, DataAdapters.GPSInfo, ParameterExtractors.None, this);
            GPSTimer = new Timer(0.5f, updateGPS);
            SetLightIntensity(); */
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
        public int nightMode;
        // Black becomes transparent on HoloLens (and so do shadows)
        // Therefore, this code ensures that the scene is brightly lit
        public void SetLightIntensity(int value = 30)
        {
            Light playerLight = mainCamera.transform.GetChild(0).gameObject.GetComponent<Light>();
            playerLight.range = (float)Config.Instance.conf.UISettings["HorizonPlaneRadius"] + 1;
            if (nightMode == 0)
            {
                playerLight.intensity = value;
                Debug.Log("disable nightmode; light intensity: " + playerLight.intensity);
            }

            else if (nightMode == 1)
            {
                playerLight.intensity = 15;
                Debug.Log("enable semi nightmode; light intensity: " + playerLight.intensity);

            }
            else if (nightMode == 2)
            {
                playerLight.intensity = 1;
                Debug.Log("enable nightmode; light intensity: " + playerLight.intensity);
            }

        }

        public void EnsureMainCamera()
        {
            if (!mainCamera)
            {
                mainCamera = (Camera)GameObject.Find("MixedRealityPlayspace").transform.GetChild(0).gameObject.GetComponent<Camera>();
                SetLightIntensity();
            }
        }

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
           
            if(OnPlayerPositionChanged != null)
                OnPlayerPositionChanged = null;
        
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

        public Tuple<Vector2, Vector2> GetCurrentLatLonArea(double customRange = -1)//previously: () ; added customRange to allow for different ranges (e.g. vessel vs navaids)
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

            return HelperClasses.GPSUtils.Instance.GetCurrentLatLonArea(lat, lon, customRange);
        }



        #region new added, not part of original script
        Tuple<Vector2, Vector2> previousArea;
        /// <summary>
        /// New method for checking whether the curren Lat/Lon Area has changed significantly.
        /// </summary>
        /// <param name="toleranceMeters"></param>
        /// <returns></returns>
        public bool HasAreaChanged(float toleranceMeters = 10f)
        {
            if (previousArea == null)
                return true;

            Tuple<Vector2, Vector2> currentArea = GetCurrentLatLonArea();
            Vector2 currentCenter = CalculateCenter(currentArea);
            Vector2 previousCenter = CalculateCenter(previousArea);

            if (Vector2.Distance(currentCenter, previousCenter) > toleranceMeters)
            {
                // The area has moved beyond the tolerance threshold
                Debug.Log("Significant movement detected.");

                // Update the previous area
                previousArea = currentArea;
                return true;

            }
            else return false;
        }
        private Vector2 CalculateCenter(Tuple<Vector2, Vector2> area)
        {
            // Calculate the center of the area
            Vector2 center = new Vector2(
                (area.Item1.x + area.Item2.x) / 2,
                (area.Item1.y + area.Item2.y) / 2
            );
            return center;
        }
        // The rotation that transforms the Unity north axis to True north
        // This should only be executed when Hololens and vessel are aligned
        // (and thus vessel compass information and Hololens direction match)

        Camera calibrationCam;
        public void PrepareCalibration(Camera _calibCam)
        {
            calibrationCam = _calibCam;
            mainCamera = calibrationCam;
            SetLightIntensity();
        }
        public void FinishCalibration()
        {
            mainCamera = null;
            this.EnsureMainCamera();
        }

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

            Debug.Log("Unity to true north: " + unityToTrueNorthRotation);

        }
        public Transform mixedrealityPlayspace;
        public PlayerData GetLogData()
        {
            PlayerData data = new PlayerData();

            data.playerPos = transform.position;
            data.playerRot = transform.rotation;
            data.unity2TrueNorth = Unity2TrueNorth;
            data.lastGPSUpdate_Latitude = lastGPSUpdate.Latitude;
            data.lastGPSUpdate_Longitude = lastGPSUpdate.Longitude;
            data.lastGPSUpdate_Heading = lastGPSUpdate.Heading;
            data.lastGPSUpdate_SOG = lastGPSUpdate.SOG;
            data.CalibrationHDGVessel = CalibrationHDGVessel;
            data.CalibrationHDGHolo = CalibrationHDGHolo;
            data.CalibrationDiff = CalibrationDiff;
            data.cameraPos = mainCamera.transform.position;
            data.cameraRot = mainCamera.transform.rotation;
            if (mixedrealityPlayspace == null)
                mixedrealityPlayspace = GameObject.Find("MixedRealityPlayspace").transform;
            data.mrPlayspacePos = mixedrealityPlayspace.position;
            data.mrPlayspaceRot = mixedrealityPlayspace.rotation;

            return data;
        }
    }

}
