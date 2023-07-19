using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCSender : MonoBehaviour
{
    public OSC osc;

    public bool sentPing;
    public bool sentPong;
    public void SendPing()
    {
    

        OscMessage message = new OscMessage();
        message.address = "/ping";
        message.values.Add(0);
        osc.SendPing(message);
        Debug.Log("pinged " + message.ToString());

        sentPing = true;
    }
    public void SendPong()
    {
        OscMessage message = new OscMessage();
        message.address = "/pong";
        message.values.Add(0);
        osc.Send(message);
        Debug.Log("ponged " + message.ToString());
        sentPong = true;
    }

    public void SendCursorPos(float x,float y, float z)
    {
        OscMessage msg = new OscMessage();
        msg.address = "/cursorPos";
        msg.values.Add(x);
        msg.values.Add(y);
        msg.values.Add(z);
        osc.Send(msg);
        Debug.Log("sent: " + msg.ToString());
    }
  
  
    public void SendMarked(string _vesselName)
    {
        OscMessage message = new OscMessage();
        message.address = "/marked";
        message.values.Add(_vesselName);
       
        osc.Send(message);

        Debug.Log("sent " + message.ToString());
    }

   
}
