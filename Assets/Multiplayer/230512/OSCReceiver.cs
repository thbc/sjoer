using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class OSCReceiver : MonoBehaviour
{
    public OSC osc;

   // [Space(20)]
    //public TextMeshProUGUI tempText;

    public GameObject CoPlayerCursorHighlight;


    void Start()
    {

        osc.SetAddressHandler("/pos", OnReceivePos);
        osc.SetAddressHandler("/rot", OnReceiveRot);
        osc.SetAddressHandler("/marked", OnReceiveMark);


    }

    bool isFirstMsg = true;
    void OnReceivePos(OscMessage message)
    {
        if (isFirstMsg)
        {
            CoPlayerCursorHighlight.SetActive(true);
            isFirstMsg = false;
        }
        float x = message.GetFloat(0);
        float y = message.GetFloat(1);
        float z = message.GetFloat(2);

        Debug.Log("received: " + x + y + z);
        //if (tempText != null)
        //{
        //    tempText.text = x.ToString("F1") + y.ToString("F1") + z.ToString("F1");
        //}
        if (CoPlayerCursorHighlight != null)
        {
            CoPlayerCursorHighlight.transform.position = new Vector3(x, y, z);
        }
    }
    void OnReceiveRot(OscMessage message)
    {
        float x = message.GetFloat(0);
        float y = message.GetFloat(1);
        float z = message.GetFloat(2);
        float w = message.GetFloat(3);

        Debug.Log("received: " + x + y + z + w);
        //if (tempText != null)
        //{
        //    tempText.text = x.ToString("F1") + y.ToString("F1") + z.ToString("F1");
        //}
        if (CoPlayerCursorHighlight != null)
        {
            CoPlayerCursorHighlight.transform.rotation = new Quaternion(x, y, z, w);
        }
    }
 

        //TODO: create list of marked strings and check if "s" already exists
        void OnReceiveMark(OscMessage message)
    {
        string sO = message.ToString();
        string s = sO.Replace(@"/marked ", "");
        if(Marker.Instance.IsVesselMarked(s)) return;
        s = s + " (HorizonPlane)";
        GameObject vesselGO;
         Debug.Log("received mark: " + s);       

        if (GameObject.Find(s))
        {
            vesselGO = GameObject.Find(s);
            if (vesselGO.tag == "MARKED") return;

            Marker.Instance.MarkItem(vesselGO);
        }
        else
        {
            s = "[T] " + s;
            if(GameObject.Find(s))
            {
                vesselGO = GameObject.Find(s);
                if (vesselGO.tag == "MARKED") return;

                Marker.Instance.MarkItem(vesselGO);
            }
        }
    }


}
