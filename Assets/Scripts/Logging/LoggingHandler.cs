using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Assets.Positional;

public class LoggingHandler : MonoBehaviour
{
    public float loggingInterval = 1.0f;
    private string initialTimestamp;

    // Parameters you mentioned are public, but depending on how you update them,
    // you might want to make them private and update via methods or property setters.

    private StringBuilder csvContent;
    private bool isLoggingStarted = false;

    public TextMesh lastLogLabel;

    public void StartStopLogging(TextMesh _StartStopBtn)
    {
        if (!isLoggingStarted)
        {
            _StartStopBtn.text = "Stop logging";
            csvContent = new StringBuilder();
            StartCoroutine(LogData());
        }
        else if (isLoggingStarted)
        {
            _StartStopBtn.text = "Start logging";
            StoreLog();
        }
    }


    private IEnumerator LogData()
    {
        while (true)
        {
            if (!isLoggingStarted)
            {
                initialTimestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                isLoggingStarted = true;

                // CSV headers. Add all the parameter names here in the order they will be logged.
                string header = "Time,PlayerPosition,PlayerRotation,Unity2TrueNorth," +
                                "LastGPSUpdate_Latitude,LastGPSUpdate_Longitude,LastGPSUpdate_Heading,LastGPSUpdate_SOG," +
                                "CalibrationHDGVessel,CalibrationHDGHolo,CalibrationDiff," +
                                "CameraPosition,CameraRotation,MRPlayspacePosition,MRPlayspaceRotation";
                csvContent.AppendLine(header);
            }

            // Here's where we use the method from Player.Instance to get current data
            PlayerData data = Player.Instance.GetLogData();

            string currentTime = System.DateTime.Now.ToString("o");
            List<string> currentRow = new List<string>
        {
            currentTime,
            data.playerPos.ToString(), // Assuming you can use ToString(), otherwise format as needed
            data.playerRot.eulerAngles.ToString(), // Converting Quaternion to Euler for readability
            data.unity2TrueNorth.eulerAngles.ToString(),
            data.lastGPSUpdate_Latitude.ToString(),
            data.lastGPSUpdate_Longitude.ToString(),
            data.lastGPSUpdate_Heading.ToString(),
            data.lastGPSUpdate_SOG.ToString(),
            data.CalibrationHDGVessel.ToString(),
            data.CalibrationHDGHolo.ToString(),
            data.CalibrationDiff.ToString(),
            data.cameraPos.ToString(),
            data.cameraRot.eulerAngles.ToString(),
            data.mrPlayspacePos.ToString(),
            data.mrPlayspaceRot.eulerAngles.ToString()
        };

            csvContent.AppendLine(EncodeCSVRow(currentRow));

            // This logs every 'loggingInterval' seconds
            yield return new WaitForSeconds(loggingInterval);
        }
    }


    public void StoreLog()
    {
        string directoryPath = Application.persistentDataPath + "/logs/";

        // Check if the directory exists, if not, create it
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Constructing the file path using the initial timestamp
        string filePath = directoryPath + "Log_" + initialTimestamp + ".csv";

        // Writing the content to the file
        File.WriteAllText(filePath, csvContent.ToString());
        lastLogLabel.text = "Last: "+ "Log_" + initialTimestamp + ".csv";

        // Clear the log buffer if you plan to continue logging new data
        csvContent.Clear();
        isLoggingStarted = false; // if you plan to capture a new timestamp for the next set of logs
    }

    private string EncodeCSVRow(List<string> fields)
    {
        StringBuilder builder = new StringBuilder();
        bool firstColumn = true;
        foreach (string field in fields)
        {
            // Add separator if this isn't the first field
            if (!firstColumn)
                builder.Append(",");
            // Wrap field in quotes if it contains special characters (comma, quote, etc.)
            if (field.IndexOfAny(new char[] { '"', ',' }) != -1)
            {
                // Double up any quotes
                builder.AppendFormat("\"{0}\"", field.Replace("\"", "\"\""));
            }
            else
            {
                builder.Append(field);
            }
            firstColumn = false;
        }
        return builder.ToString();
    }

    private void OnApplicationQuit()
    {
        // Save the data when the application is closed
        StoreLog();
    }
}
public class PlayerData
{
    public Vector3 playerPos;
    public Quaternion playerRot;
    public Quaternion unity2TrueNorth;
    public double lastGPSUpdate_Latitude;
    public double lastGPSUpdate_Longitude;
    public double lastGPSUpdate_Heading;
    public double lastGPSUpdate_SOG;
    public float CalibrationHDGVessel;
    public float CalibrationHDGHolo;
    public float CalibrationDiff;
    public Vector3 cameraPos;
    public Quaternion cameraRot;
    public Vector3 mrPlayspacePos;
    public Quaternion mrPlayspaceRot;

}

