using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Positional;
/* 
    This script is a helper for Multiplayer functionality.
    It serves as reference to the Transformation between initial coordinate system and Unity2TrueNorth...
 */
public class PlayerCoordinates : MonoBehaviour
{
    public Player player;
    float initialY;
    [Tooltip("If true, the player's main camera replaces the assigned Transform playerCam.")]
    public bool usePlayerMainCam;

    [Tooltip("The transform position of the coordinate system. This can either be static or moving along with the player, like its main Camera. Will only be used if usePlayerMainCam is false..")]
    public Transform playerCam;


    void OnEnable()
    {
        if (usePlayerMainCam)
            playerCam = player.GetMainCamera().transform; //mainCamera


        initialY = this.transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(playerCam.position.x, initialY, playerCam.position.z);
        transform.rotation = player.Unity2TrueNorth;
    }

}
