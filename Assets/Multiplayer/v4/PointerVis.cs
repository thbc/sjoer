using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Unity.Netcode;

public class PointerVis : MonoBehaviour
{
    // public GameObject PrefabToSpawn;
    public GameObject SpawnedPointer;

    public NetworkManager m_networkManager;

    public bool pointlocalspace;
    public void Spawn(MixedRealityPointerEventData eventData)
    {
        var result = eventData.Pointer.Result;

        /*   if (PrefabToSpawn != null && SpawnedPointer == null)
          {
              SpawnedPointer = Instantiate(PrefabToSpawn, result.Details.Point, Quaternion.LookRotation(result.Details.Normal));
          }

          else if (SpawnedPointer != null && !continuousVis)
          { */
        Debug.Log(result.Details.RayDistance);
        if (result.Details.RayDistance < 100)
        {
            if (!SpawnedPointer.activeSelf)
                SpawnedPointer.SetActive(true);

            SpawnedPointer.transform.position = result.Details.Point;
        }
        else
        {
            SpawnedPointer.SetActive(false);
        }
        /*  } */

    }

     [ServerRpc]
    public void PinchServerRpc(MixedRealityPointerEventData eventData)
    {
        // Execute the original Pinch logic on the server
        Pinch(eventData);
    }

    // Call the Pinch method on the server when it's called on the client
    public void CallPinch(MixedRealityPointerEventData eventData)
    {
        if (m_networkManager.IsClient) // Make sure the function is called only on the client
        {
            PinchServerRpc(eventData);
        }
    }

    
        public void Pinch(MixedRealityPointerEventData eventData)
    {
    
        var result = eventData.Pointer.Result;

        /*   if (PrefabToSpawn != null && SpawnedPointer == null)
          {
              SpawnedPointer = Instantiate(PrefabToSpawn, result.Details.Point, Quaternion.LookRotation(result.Details.Normal));
          }

          else if (SpawnedPointer != null && !continuousVis)
          { */
        Debug.Log(result.Details.RayDistance);
        if (result.Details.RayDistance < 100 )
        {
           /*  if (!SpawnedPointer.activeSelf)
                SpawnedPointer.SetActive(true); */
               

            SpawnedPointer.transform.position = result.Details.Point;
           if(result.Details.RayDistance < 5)
                SpawnedPointer.transform.position = Vector3.ClampMagnitude(SpawnedPointer.transform.position, 5f);

        }
        else
        {   
            float radius =20.0f; // Replace with your desired radius

SpawnedPointer.transform.position = result.Details.Point;
SpawnedPointer.transform.position = Vector3.ClampMagnitude(SpawnedPointer.transform.position, radius);
            /* SpawnedPointer.SetActive(false); */
        
        }
            Transform target = Camera.main.transform; // Replace with the transform of your player if applicable

    // Set the rotation of SpawnedPointer to face towards the target
    SpawnedPointer.transform.LookAt(target);
        /*  } */

    }

}
