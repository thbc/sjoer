using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HelperClasses;
using Assets.Resources;
using Assets.InfoItems;

public class Marker : MonoBehaviour
{
    public static Marker Instance { get; private set; }

    public Material assignedMaterial;
    public Material markedMaterial;

    public bool allowMarking=false;

    public OSCSender sender;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    // Method to get the assigned material
    public Material GetAssignedMaterial()
    {
        return assignedMaterial;
    }
    public Material GetMarkedMaterial()
    {
        return markedMaterial;
    }

    public bool SetMarkMode()
    {
        allowMarking = !allowMarking;
        Debug.Log("allowMarking: " + allowMarking);
        return allowMarking;
    }
    public void SendMarker(string _vesselName)
    {
        Debug.LogWarning("Send MARKER: " + _vesselName);
        sender.SendMarked(_vesselName);

    }

    public void MarkItem(GameObject vesselGO)
    {
        /*         if (vesselGO.tag != "MARKED")
                { */
        Debug.LogWarning("found: " + vesselGO.name);
        vesselGO.gameObject.tag = "MARKED";
        vesselGO.GetComponent<TargettableInfoItem>().OnClick(); // trying to open item through OnClick function

        vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetMarkedMaterial();
        //   GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = Marker.Instance.GetMarkedMaterial();;
        //}
        AddVessel(vesselGO.name);
    }
    public void UnmarkItem(GameObject vesselGO)
    {
        Debug.LogWarning("UNMARK: " + vesselGO.name);
        vesselGO.gameObject.tag = "Untagged";
        vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetAssignedMaterial();
          RemoveVessel(vesselGO.name);
    }
    public List<string> markedVessels = new List<string>();
    void AddVessel(string vesselName)
    {
        if (!markedVessels.Contains(vesselName))
        {
            markedVessels.Add(vesselName);
        }
    }

    void RemoveVessel(string vesselName)
    {
        markedVessels.Remove(vesselName);
    }

    public bool IsVesselMarked(string vesselName)
    {
        return markedVessels.Contains(vesselName);
    }

}
