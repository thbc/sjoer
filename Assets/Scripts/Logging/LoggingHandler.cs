using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Assets.Positional;
using Assets.Resources;

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

            // Detailed CSV headers to match each specific data point being logged.
            string header = "Date,Time," +
                            "VesselName, VesselMode, BridgeHeight,"+


                            "PlayerPosX,PlayerPosY,PlayerPosZ,PlayerPosition," +
                            "PlayerRotX,PlayerRotY,PlayerRotZ,PlayerEulerRotation,PlayerRotation," +
                            "Unity2TrueNorthX,Unity2TrueNorthY,Unity2TrueNorthZ,Unity2TrueNorthEuler,Unity2TrueNorth," +
                            "LastGPSLat,LastGPSLong,LastGPSHeading,LastGPSSOG," +
                            "CalibHDGVessel,CalibHDGHolo,CalibDiff," +
                            "CameraPosX,CameraPosY,CameraPosZ,CameraPosition," +
                            "CameraRotX,CameraRotY,CameraRotZ,CameraEulerRotation,CameraRotation," +
                            "MRPlayspacePosX,MRPlayspacePosY,MRPlayspacePosZ,MRPlayspacePosition," +
                            "MRPlayspaceRotX,MRPlayspaceRotY,MRPlayspaceRotZ,MRPlayspaceEulerRotation,MRPlayspaceRotation";
            csvContent.AppendLine(header);
        }

        // Here's where we use the method from Player.Instance to get current data
        PlayerData data = Player.Instance.GetLogData();

        // Separate the current time into date and time parts
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd"); // Only the date part
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss"); // Only the time part

        // Here we prepare the current row to be written in the CSV file, starting with date and time
        List<string> currentRow = new List<string>
        {
            currentDate,
            currentTime,
            Config.Instance.conf.VesselSettingsS["VesselName"],
            Config.Instance.conf.VesselMode.ToString(),
            Config.Instance.conf.VesselSettingsD["BridgeHeight"].ToString(),
            data.playerPos.x.ToString(),
            data.playerPos.y.ToString(),
            data.playerPos.z.ToString(),
            data.playerPos.ToString(), // Assuming you can use ToString(), otherwise format as needed

            data.playerRot.eulerAngles.x.ToString(),
            data.playerRot.eulerAngles.y.ToString(),
            data.playerRot.eulerAngles.z.ToString(),
            data.playerRot.eulerAngles.ToString(), // Converting Quaternion to Euler for readability
            data.playerRot.ToString(),

            data.unity2TrueNorth.eulerAngles.x.ToString(),
            data.unity2TrueNorth.eulerAngles.y.ToString(),
            data.unity2TrueNorth.eulerAngles.z.ToString(),
            data.unity2TrueNorth.eulerAngles.ToString(),
            data.unity2TrueNorth.ToString(),

            data.lastGPSUpdate_Latitude.ToString(),
            data.lastGPSUpdate_Longitude.ToString(),
            data.lastGPSUpdate_Heading.ToString(),
            data.lastGPSUpdate_SOG.ToString(),
            data.CalibrationHDGVessel.ToString(),
            data.CalibrationHDGHolo.ToString(),
            data.CalibrationDiff.ToString(),
                  
            data.cameraPos.x.ToString(),
            data.cameraPos.y.ToString(),
            data.cameraPos.z.ToString(),
            data.cameraPos.ToString(),

            data.cameraRot.eulerAngles.x.ToString(),
            data.cameraRot.eulerAngles.y.ToString(),
            data.cameraRot.eulerAngles.z.ToString(),
            data.cameraRot.eulerAngles.ToString(),
            data.cameraRot.ToString(),

            data.mrPlayspacePos.x.ToString(),
            data.mrPlayspacePos.y.ToString(),
            data.mrPlayspacePos.z.ToString(),
            data.mrPlayspacePos.ToString(),
            
            data.mrPlayspaceRot.eulerAngles.x.ToString(),
            data.mrPlayspaceRot.eulerAngles.y.ToString(),
            data.mrPlayspaceRot.eulerAngles.z.ToString(),
            data.mrPlayspaceRot.eulerAngles.ToString(),
            data.mrPlayspaceRot.ToString()

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

