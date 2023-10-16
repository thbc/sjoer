using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Resources;
using Assets.SceneManagement;
using Assets.Positional;
using Assets.HelperClasses;
using TMPro;

namespace Assets.Calibration
{
    public class Calibrator : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI countDown;

        private Timer steadyTimer;
        private Quaternion rot = Quaternion.identity;
        private Vector3 pos = Vector3.zero;
        private DateTime startTime;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Calibrating. Hold head steady for 3 seconds.");
            Player.Instance.EnsureMainCamera();
            Player.Instance.SetLightIntensity(5);

            this.steadyTimer = new Timer(
                (float)Config.Instance.conf.CalibrationSettings["SteadyTime"],
                this.calibrate
                );
            this.startTime = DateTime.Now;
        }

        // Update is called once per frame
        void Update()
        {
            // We need to manually call the update of the Timer instance
            this.steadyTimer.Update();

            countDown.text = $"Face bow in {this.steadyTimer.GetSecondsRemaining()}s";

            // Check for steadyness every second
            //DateTime now = DateTime.Now;
            //if ((now - this.startTime).TotalSeconds > 1)
            //{
            //    this.startTime = now;

            //    // If the rotation and position are shaky, restart the timer
            //    if (isShakyRot(mainCamera.transform.rotation) || isShakyPos(mainCamera.transform.position))
            //    {
            //        this.steadyTimer.restart();
            //    }
            //}
        }

        private bool isShakyRot(Quaternion incoming)
        {
            // Substract the new movement from the last movement so that we get a vector between the rotation points
            Quaternion delta = incoming * Quaternion.Inverse(this.rot);

            // Update the latest rotation
           this.rot = incoming;

            // Check if the length of the movement vector exceeds the threshold
            Debug.Log($"Rotation diff length: {delta.eulerAngles.magnitude}");
            return delta.eulerAngles.magnitude > Config.Instance.conf.CalibrationSettings["RotationThreshold"];
        }

        private bool isShakyPos(Vector3 incoming)
        {
            // Substract the new movement from the last movement so that we get a vector between the rotation points
            Vector3 delta = incoming - this.pos;

            // Update the latest position
            this.pos = incoming;

            // Check if the length of the movement vector exceeds the threshold
            Debug.Log($"Position diff length: {delta.magnitude}");
            return delta.magnitude > Config.Instance.conf.CalibrationSettings["PositionThreshold"];
        }

        // Is executed when the timer completes
        private void calibrate()
        {
            Player.Instance.calibrate();
            MySceneManager.Instance.exitCalibration();
        }
    }
}
