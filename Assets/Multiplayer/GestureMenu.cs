using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GestureMenu : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter Detected: " + other.gameObject.name);
        
        if(other.gameObject.tag == "YourTargetTag")
        {
            Debug.Log("Target Tag Detected: " + other.gameObject.name);
            YourFunction();
        }
    }

    void YourFunction()
    {
        Debug.Log("Your function is triggered");
    }
}