using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Positional;

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

        osc.SetAddressHandler("/cursorPos", OnReceiveCursorPos); 
        osc.SetAddressHandler("/marked", OnReceiveMark);
        osc.SetAddressHandler("/unmarked", OnReceiveUnmark);

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


    public Transform playerTransform;

    public Player aligner;
   
    public Transform playerCoordinateTransform;
    void OnReceiveCursorPos(OscMessage msg)
    {
        if (CoPlayerCursorHighlight != null)
        {
            float x = msg.GetFloat(0);
            float y = msg.GetFloat(1);
            float z = msg.GetFloat(2);
            // Return the position relative to the player's position
            CoPlayerCursorHighlight.transform.position = playerCoordinateTransform.transform.position + new Vector3(x, y, z);
        }
    }
    
    void OnReceiveMark(OscMessage message)
    {
        string sO = message.ToString();
        string s = sO.Replace(@"/marked ", "");
        MarkerMode.Instance.OnMarkItemReceived(s);
    }

void OnReceiveUnmark(OscMessage message)
    {
        string sO = message.ToString();
        string s = sO.Replace(@"/unmarked ", "");
        MarkerMode.Instance.OnUnmarkItemReceived(s);
    }

}
