using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Positional;
using Multiplayer.Marking;
public class OSCReceiver : MonoBehaviour
{
    public OSC osc;
    public ConnectionController connectionController;
    public OSCSender oscSender;

    // [Space(20)]
    //public TextMeshProUGUI tempText;

    public GameObject CoPlayerCursorHighlight;
    public Transform playerTransform;

    public Player aligner;

    public Transform playerCoordinateTransform;

    public bool _receivedPing;
    public bool receivedPing
    {
        get { return _receivedPing; }
        set
        {
            if (value == true)
                osc.isInitialized = true;
            _receivedPing = value;
        }
    }
    public bool receivedPong;
    public void SetupListener()
    {
        osc.SetAddressHandler("/ping", OnReceivePing);
        osc.SetAddressHandler("/pong", OnReceivePong);

        osc.SetAddressHandler("/cursorPos", OnReceiveCursorPos);
        osc.SetAddressHandler("/marked", OnReceiveMark);

        osc.SetAddressHandler("/unmarked/sent", OnReceiveUnmarkSent);
        osc.SetAddressHandler("/unmarked/received", OnReceiveUnmarkReceived);


    }



    void OnReceivePing(OscMessage message)
    {
        Debug.Log("received ping");
        connectionController.statusLabel_2.text = "received ping" + connectionController.statusLabel_2.text;
        receivedPing = true;

        // Do other operations.
        CoPlayerCursorHighlight.SetActive(true);
        oscSender.SendPong();
    }
    void OnReceivePong(OscMessage message)
    {
        Debug.Log("received pong");
        connectionController.statusLabel_2.text = "received pong" + connectionController.statusLabel_2.text;

        receivedPong = true;

        connectionController.OnSuccessfullyConnected();
    }



    void OnReceiveCursorPos(OscMessage msg)
    {
        if (CoPlayerCursorHighlight != null)
        {
            float x = msg.GetFloat(0);
            float y = msg.GetFloat(1);
            float z = msg.GetFloat(2);
            // Return the position relative to the player's position
            CoPlayerCursorHighlight.transform.position = playerCoordinateTransform.transform.position + new Vector3(x, y, z);


            // Ensure the cursor faces the camera
            CoPlayerCursorHighlight.transform.LookAt(Player.Instance.mainCamera.transform);
        }
    }


    ///////////////
    /// 
    void OnReceiveMark(OscMessage message)
    {
        string sO = message.ToString();
        string s = sO.Replace(@"/marked ", "");
        Debug.Log("received 'marked sent' for: " + s + ", triggering 'markReceived'");

        MarkerMode.Instance.OnReceivedMark_receivedMarkers(s);
    }
    void OnReceiveUnmarkSent(OscMessage message)
    {
        string sO = message.ToString();
        string s = sO.Replace(@"/unmarked/sent ", "");
        Debug.Log("received 'unmarked sent' for: " + s + " , triggering 'unmarkReceived'");
        MarkerMode.Instance.OnReceivedUnmark_receivedMarkers(s);
    }
    void OnReceiveUnmarkReceived(OscMessage message)
    {
        string sO = message.ToString();
        string s = sO.Replace(@"/unmarked/received ", "");
        Debug.Log("received 'unmarked received' for: " + s + " , triggering 'unmarkSent'");
        MarkerMode.Instance.OnReceivedUnmark_sentMarkers(s);
    }

}
