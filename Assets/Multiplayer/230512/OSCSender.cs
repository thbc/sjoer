using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCSender : MonoBehaviour
{
    public OSC osc;

  

 
    public void SendCursor(Vector3 _pos)
    {
        float x = _pos.x;
        float y = _pos.y;
        float z = _pos.z;
    
        OscMessage message = new OscMessage();
        message.address = "/pos";
        message.values.Add(x);
        message.values.Add(y);
        message.values.Add(z);
        osc.Send(message);

        Debug.Log("sent " + message.ToString());
    }
    public void SendCursor(Quaternion _rot)
    {
        float x = _rot.x;
        float y = _rot.y;
        float z = _rot.z;
        float w = _rot.w;
        OscMessage message = new OscMessage();
        message.address = "/rot";
        message.values.Add(x);
        message.values.Add(y);
        message.values.Add(z);
        message.values.Add(w);
        osc.Send(message);

        Debug.Log("sent " + message.ToString());
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
