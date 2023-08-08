using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Positional;
public class PlayerCoordinates : MonoBehaviour
{
    public Player player;
    float initialY;
    [Tooltip("If true, the player's main camera replaces the assigned Transform playerCam.")]
    public bool usePlayerMainCam;

    [Tooltip("The transform position of the coordinate system. This can either be static or moving along with the player, like its main Camera. Will only be used if usePlayerMainCam is false..")]
    public Transform playerCam; 


    void Start()
    {            
                if (usePlayerMainCam)
                    playerCam = player.mainCamera.transform;

               
                initialY = this.transform.position.y;                       
    }

    void Update()
    {
        transform.position = new Vector3(playerCam.position.x, initialY, playerCam.position.z);
        transform.rotation = player.Unity2TrueNorth;                
    }

}
