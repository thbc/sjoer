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

    public void SendCursor(Vector3 pointerLocalPosition, Quaternion pointerLocalRotation)
    {
       
    }

    public void SendCursorPos(Vector3 _pos)
    {
        //float x = _pos.x;
        //float y = _pos.y;
        //float z = _pos.z;

        //OscMessage message = new OscMessage();
        //message.address = "/pos";
        //message.values.Add(x);
        //message.values.Add(y);
        //message.values.Add(z);
        //osc.Send(message);
        OscMessage msg = new OscMessage();
        msg.address = "/pos";
        msg.values.Add(_pos.x);
        msg.values.Add(_pos.y);
        msg.values.Add(_pos.z);
        
        osc.Send(msg);

        Debug.Log("sent " + msg.ToString());
    }
    public void SendCursorRot(Quaternion _rot)
    {
        OscMessage msg = new OscMessage();
        msg.address = "/rot";
        msg.values.Add(_rot.x);
        msg.values.Add(_rot.y);
        msg.values.Add(_rot.z);
        msg.values.Add(_rot.w);
       
        osc.Send(msg);

        Debug.Log("sent " + msg.ToString());
    }
    public void SendCursorAngles(float _az, float _elev)
    {
        //float x = _rot.x;
        //float y = _rot.y;
        //float z = _rot.z;
        //float w = _rot.w;
        //OscMessage message = new OscMessage();
        //message.address = "/rot";
        //message.values.Add(x);
        //message.values.Add(y);
        //message.values.Add(z);
        //message.values.Add(w);
        //osc.Send(message);
        OscMessage msg = new OscMessage();
        msg.address = "/rot";
        msg.values.Add(_az);
        msg.values.Add(_elev);
        

        Debug.Log("sent " + msg.ToString());
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
