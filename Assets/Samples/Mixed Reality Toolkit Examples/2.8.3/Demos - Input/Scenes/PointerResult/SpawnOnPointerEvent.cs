using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    // Example script that spawns a prefab at the pointer hit location.
    [AddComponentMenu("Scripts/MRTK/Examples/SpawnOnPointerEvent")]
    public class SpawnOnPointerEvent : MonoBehaviour
    {
        public GameObject PrefabToSpawn;

        public GameObject playerCube;

        public void Spawn(MixedRealityPointerEventData eventData)
        {
            if (PrefabToSpawn != null)
            {
                var result = eventData.Pointer.Result;
                Instantiate(PrefabToSpawn, result.Details.Point, Quaternion.LookRotation(result.Details.Normal));
            }
            if(playerCube == null)
            {
                playerCube = GameObject.FindGameObjectWithTag("playerCube");
            }
            else if(playerCube != null)
            {
                var result = eventData.Pointer.Result;
                playerCube.transform.position = result.Details.Point;
            }
           

        }
    }
}