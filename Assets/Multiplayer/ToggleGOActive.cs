using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGOActive : MonoBehaviour
{
    public GameObject go;
    public void ToggleActive()
    {
        if (go.activeSelf)
        {
            go.SetActive(false);
            Debug.Log("toggle..hiding " + go.name);
        }
        else if (!go.activeSelf)
        {
            go.SetActive(true);
            Debug.Log("toggle..showing " + go.name);

        }


    }
}
