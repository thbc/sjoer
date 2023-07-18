using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class OSCReceiver : MonoBehaviour
{
    public OSC osc;
    public ConnectionController connectionController;
    public OSCSender oscSender;

   // [Space(20)]
    //public TextMeshProUGUI tempText;

    public GameObject CoPlayerCursorHighlight;


    public void SetupListener()
    {
        osc.SetAddressHandler("/ping", OnReceivePing);
        osc.SetAddressHandler("/pong", OnReceivePong);

        osc.SetAddressHandler("/pos", OnReceivePos);
        osc.SetAddressHandler("/rot", OnReceiveRot);
        osc.SetAddressHandler("/marked", OnReceiveMark);
    }

    public bool _receivedPing;
    public bool receivedPing
    {
        get { return _receivedPing; }
        set
        {
            if(value == true)
                osc.isInitialized = true;
            _receivedPing = value;
        }
    }
    public bool receivedPong;

    void OnReceivePing(OscMessage message)
    {
        Debug.Log("received ping");
        connectionController.statusLabel_2.text = "received ping"+ connectionController.statusLabel_2.text;
        receivedPing = true;

        // Do other operations.
        CoPlayerCursorHighlight.SetActive(true);
        oscSender.SendPong();
    }
    void OnReceivePong(OscMessage message)
    {
        Debug.Log("received ping");
        connectionController.statusLabel_2.text = "received pong"+ connectionController.statusLabel_2.text;

        receivedPong = true;

        connectionController.OnSuccessfullyConnected();
    }

    public Transform originTransform;
    void OnReceivePos(OscMessage msg)
    {
        
        //float x = message.GetFloat(0);
        //float y = message.GetFloat(1);
        //float z = message.GetFloat(2);

        //Debug.Log("received: " + x + y + z);
    
        if (CoPlayerCursorHighlight != null)
        {
            Vector3 receivedPosition = new Vector3(msg.GetFloat(0), msg.GetFloat(1), msg.GetFloat(2));


            // Transform the received position and rotation from local coordinates to world coordinates
            Vector3 worldPosition = originTransform.TransformPoint(receivedPosition);

            // CoPlayerCursorHighlight.transform.position = new Vector3(x, y, z);
            CoPlayerCursorHighlight.transform.position = worldPosition;
        }
    }
    void OnReceiveRot(OscMessage msg)
    {
        //float x = message.GetFloat(0);
        //float y = message.GetFloat(1);
        //float z = message.GetFloat(2);
        //float w = message.GetFloat(3);

        //Debug.Log("received: " + x + y + z + w);

        if (CoPlayerCursorHighlight != null)
        {
            //    //CoPlayerCursorHighlight.transform.rotation = new Quaternion(x, y, z, w);

            //  Quaternion receivedRotation = new Quaternion(msg.GetFloat(0), msg.GetFloat(1), msg.GetFloat(2), msg.GetFloat(3));
            //   Quaternion worldRotation = Quaternion.LookRotation(originTransform.TransformDirection(receivedRotation * Vector3.forward), originTransform.TransformDirection(receivedRotation * Vector3.up)); 
        
            
            //Quaternion receivedRotation = new Quaternion(msg.GetFloat(0), msg.GetFloat(1), msg.GetFloat(2), msg.GetFloat(3));
            //Quaternion worldRotation = Quaternion.LookRotation(originTransform.TransformDirection(receivedRotation * Vector3.forward), originTransform.TransformDirection(receivedRotation * Vector3.up)); 
            //CoPlayerCursorHighlight.transform.rotation = worldRotation;

            float azimuth = msg.GetFloat(0);
            float elevation = msg.GetFloat(1);

            Vector3 originToPoint;
            originToPoint.x = Mathf.Sin(elevation) * Mathf.Sin(azimuth);
            originToPoint.y = Mathf.Cos(elevation);
            originToPoint.z = Mathf.Sin(elevation) * Mathf.Cos(azimuth);

            CoPlayerCursorHighlight.transform.position = Vector3.zero + originToPoint * 5.0f; // (5.0f is distance) is how far you want the pointer to be from the origin


        }
    }

  

    //TODO: create list of marked strings and check if "s" already exists
    void OnReceiveMark(OscMessage message)
    {
        string sO = message.ToString();
        string s = sO.Replace(@"/marked ", "");
        if(MarkerMode.Instance.IsVesselMarked(s)) return;
        s = s + " (HorizonPlane)";
        GameObject vesselGO;
         Debug.Log("received mark: " + s);       

        if (GameObject.Find(s))
        {
            vesselGO = GameObject.Find(s);
            if (vesselGO.tag == "MARKED") return;

            MarkerMode.Instance.MarkItem(vesselGO);
        }
        else
        {
            s = "[T] " + s;
            if(GameObject.Find(s))
            {
                vesselGO = GameObject.Find(s);
                if (vesselGO.tag == "MARKED") return;

                MarkerMode.Instance.MarkItem(vesselGO);
            }
        }
    }


}
