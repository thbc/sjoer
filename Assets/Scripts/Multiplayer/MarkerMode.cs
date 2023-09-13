using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HelperClasses;
using Assets.Resources;
using Assets.InfoItems;

public class MarkerMode : MonoBehaviour
{
    /* MarkerMode explained:

    client 1:
    If markmode is enabled and a user interacts with an InfoItem, the InfoItem.cs class calls "SendMarker()",
    which send the name of the vessel to be marked to client 2.

    client 2:
    The OSC Receiver calls on MarkerMode "MarkItem(GameObject)", which:
        *invokes a click on the responding InfoItem,
        *marks it with the yellow material,
        *adds its name to a string List "recvMarkedVessels",
        *changes the gameObject's tag to "Marked"

    In InfoItem, if allowMarking is enabled and an gameObject is tagged as "marked", we call the MarkerMode "UnmarkItem(GameObject)" which:
        *resets the gameObject tag to "Untagged"
        *resets its material to the default one
        *removes its name from the recvMarkedVessels List

    The recvMarkedVessels List serves primarily for the OSCReceiver which checks OnMarkReceived whether an object is already marked..
     */
    public static MarkerMode Instance { get; private set; }

    public Material assignedMaterial;
    public Material markedMaterial;
    public Material markedSentMaterial;

    [HideInInspector]
    public bool allowMarking = false;

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
    public Material GetMarkedSentMaterial()
    {
        return markedSentMaterial;
    }
    public bool SetMarkMode()
    {
        allowMarking = !allowMarking;
        Debug.Log("allowMarking: " + allowMarking);
        StoreMarkMode();

        return allowMarking;
    }
    public void SetMarkMode(bool state)
    {
        allowMarking = state;
        Debug.Log("allowMarking: " + allowMarking);
        StoreMarkMode();
    }
    void StoreMarkMode()
    {
        ConnectionController.Instance.playerConfig.sendMarkerMultiplayer = allowMarking;
        if (allowMarking)
            PlayerPrefs.SetInt("MultiplayerMarkMode", 1);
        else if (!allowMarking)
            PlayerPrefs.SetInt("MultiplayerMarkMode", 0);

        PlayerPrefs.Save();

    }


    #region This handling the incoming marker, and detecting when the recipient closes it again
    public void OnMarkItemReceived(string s)//(GameObject vesselGO)
    {
        // return if vessel is already marked
        if (IsVesselMarkedReceived(s)) return;
        s = s + " (HorizonPlane)";
        Debug.Log("received mark: " + s);
        GameObject vesselGO;
        if (GameObject.Find(s))
        {
            vesselGO = GameObject.Find(s);
            if (vesselGO.tag == "MARKED") return;

            MarkItemReceived(vesselGO);
        }
        else
        {   // this logic makes sure both expanded and collapses markers are found
            s = "[T] " + s;
            if (GameObject.Find(s))
            {
                vesselGO = GameObject.Find(s);
                if (vesselGO.tag == "MARKED") return;

                MarkItemReceived(vesselGO);
            }
        }
    }
    void MarkItemReceived(GameObject vesselGO)
    {
        Debug.LogWarning("found: " + vesselGO.name);
        vesselGO.gameObject.tag = "MARKED";
        vesselGO.GetComponent<TargettableInfoItem>().OnClick(); // trying to open item through OnClick function
        vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetMarkedMaterial();
        AddVesselReceived(vesselGO.name);
    }
    public void UnmarkItem(GameObject vesselGO)
    {
        Debug.LogWarning("UNMARK: " + vesselGO.name);
        vesselGO.gameObject.tag = "Untagged";
        vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetAssignedMaterial();
        RemoveVesselReceived(vesselGO.name);

        //TODO: send and notify Unmark to other player
    }
    #endregion
    #region This handling the sending of marker to other player and also marking it for the sender-client and adds to list for keeping track
    // caled from InfoItem if allowMarking is true
    public void SendMarker(string _vesselName)
    {
        if (!sender.osc.isInitialized)
            return;
        //Should send Marker but OSC is not initialized yet..

        //Debug.LogWarning("Send MARKER: " + _vesselName);
        sender.SendMarked(_vesselName);

        OnMarkItemSent(_vesselName);
    }
    public void OnMarkItemSent(string s)
    {
        // return if vessel is already marked
        if (IsVesselMarkedSent(s)) return;
        s = s + " (HorizonPlane)";
        //Debug.Log("sending and marking mark: " + s);
        GameObject vesselGO;
        if (GameObject.Find(s))
        {
            vesselGO = GameObject.Find(s);
            if (vesselGO.tag == "MARKEDout") return;

            MarkItemSent(vesselGO);
        }
        else
        {   // this logic makes sure both expanded and collapses markers are found
            s = "[T] " + s;
            if (GameObject.Find(s))
            {
                vesselGO = GameObject.Find(s);
                if (vesselGO.tag == "MARKEDout") return;

                MarkItemSent(vesselGO);
            }
        }
    }

    public void MarkItemSent(GameObject vesselGO)
    {
        Debug.LogWarning("found: " + vesselGO.name);
        vesselGO.gameObject.tag = "MARKEDout";
        vesselGO.GetComponent<TargettableInfoItem>().OnClick(); // trying to open item through OnClick function
        vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetMarkedSentMaterial();
        AddVesselSent(vesselGO.name);
    }

     public void OnUnmarkItemReceived(string s)
    {
        if (!IsVesselMarkedSent(s)) return;
        s = s + " (HorizonPlane)";
        //Debug.Log("sending and marking mark: " + s);
        GameObject vesselGO;
        if (GameObject.Find(s))
        {
            vesselGO = GameObject.Find(s);
            if (vesselGO.tag == "MARKEDout")
            UnmarkItemReceived(vesselGO);
        }
        else
        {   // this logic makes sure both expanded and collapses markers are found
            s = "[T] " + s;
            if (GameObject.Find(s))
            {
                vesselGO = GameObject.Find(s);
                if (vesselGO.tag == "MARKEDout")
                UnmarkItemReceived(vesselGO);
            }
        }        
    }
    void UnmarkItemReceived(GameObject vesselGO)
    {
        Debug.LogWarning("UNMARK: " + vesselGO.name);
        vesselGO.gameObject.tag = "Untagged";
        vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetAssignedMaterial();
        RemoveVesselSent(vesselGO.name);
    }
    #endregion
    // called from OSCReceiver
/*     void MarkItem(GameObject vesselGO)
    {
        Debug.LogWarning("found: " + vesselGO.name);
        vesselGO.gameObject.tag = "MARKED";
        vesselGO.GetComponent<TargettableInfoItem>().OnClick(); // trying to open item through OnClick function

        vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetMarkedMaterial();
      
        AddVesselReceived(vesselGO.name);
    } */

    public List<string> recvMarkedVessels = new List<string>();
    void AddVesselReceived(string vesselName)
    {
        if (!recvMarkedVessels.Contains(vesselName))
        {
            recvMarkedVessels.Add(vesselName);
        }
    }

    void RemoveVesselReceived(string vesselName)
    {
        recvMarkedVessels.Remove(vesselName);
    }

    public bool IsVesselMarkedReceived(string vesselName)
    {
        return recvMarkedVessels.Contains(vesselName);
    }

    // the sent marker section
    public List<string> sentMarkedVessels = new List<string>();
    void AddVesselSent(string vesselName)
    {
        if (!sentMarkedVessels.Contains(vesselName))
        {
            sentMarkedVessels.Add(vesselName);
        }
    }

    void RemoveVesselSent(string vesselName)
    {
        sentMarkedVessels.Remove(vesselName);
    }

    public bool IsVesselMarkedSent(string vesselName)
    {
        return sentMarkedVessels.Contains(vesselName);
    }
}
