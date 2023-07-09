using Microsoft.MixedReality.Toolkit.Utilities.GameObjectManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class te : MonoBehaviour
{
    GameObject cursor;
    static int counter = 0;

     GameObject origin;

    public OSCSender sender;
    void OnEnable()
    {
        if(origin == null)
        {
            origin = GameObject.FindGameObjectWithTag("MainCamera");
        }
       
        if (cursor == null)
        {
            cursor = this.gameObject;
        }
        counter++;
    }
    private void OnDisable()
    {
        counter--;
    }

    // Update is called once per frame
    void Update()
    {
       // var cursors = GameObject.FindGameObjectsWithTag("cursorHighlight");
       // foreach (var cursor in cursors)
       // {
       if(origin != null) { 
            Debug.Log(cursor.name + counter + "\n"
                + cursor.transform.position.x + "" + cursor.transform.position.y + " " +cursor.transform.position.z + "\n"
                +(origin.transform.position.x - cursor.transform.position.x) + "" + (origin.transform.position.y - cursor.transform.position.y) + " " + (origin.transform.position.z - cursor.transform.position.z));
            
            if(sender != null)
            {
                sender.SendR(origin.transform.position.x - cursor.transform.position.x);
            }
            else
            {
                Debug.LogWarning("sender not defined");
            }
        }
    }
}
