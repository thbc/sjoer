using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SceneManagement;
using Assets.Positional;
using Assets.Resources;
/// <summary>
/// This reference enforcer ensures that the correct Singleton.Instance is assigned OnEnable when switching from calibration scene back to Default Scene.
/// This method is only required when using two split scenes for calibration.
/// </summary>
public class SingletonReferenceEnforcer : MonoBehaviour
{


    public static SingletonReferenceEnforcer Instance { get; private set; }

   /*  public MySceneManager original_MySceneManager;
    public Player original_Player;
    public AssetManager original_AssetManager; */

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //  DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }

    void OnEnable()
    {
        ConnectionController.Instance.statusLabel = statusLabel;
        ConnectionController.Instance.statusLabel_2 = statusLabel_2;
        ConnectionController.Instance.cursorMP = cursorMP;
    }

#region For Speech Input Handler, which looses references on scene switch
    public void Execute_SceneManager_StartCalibration()
    {
        MySceneManager.Instance.startCalibration();
    }
#endregion

#region Connection controller status labels..
public CursorMultiplayer cursorMP;
 public TextMesh statusLabel;
public TextMesh statusLabel_2;
#endregion

/* #region OSC receiver..

public GameObject cursorRecv;
public Transform playerCoordinateTransform;
#endregion */
   

  
    
/* 
#region Player Instance stuff...

 public void HandleDuplicatePlayerInScene()
    {
        Player[] foundPlayers = FindObjectsOfType<Player>();

        // If no player or only one player is found, no need for further processing.
        if (foundPlayers.Length <= 1)
        {
            original_Player = foundPlayers[0];
            return;
        }
        // ...else..
      original_Player =  GetCorrectPlayer(foundPlayers);

        
    }
    private Player GetCorrectPlayer(Player[] foundPlayers)
    {
        GameObject gameObjToDelete = null;
        Player _toKeep = null;
        foreach (Player sceneManager in foundPlayers)
        {
            UnityEngine.SceneManagement.Scene playerScene = sceneManager.gameObject.scene;

            Debug.LogFormat("{0} in scene: {1}", sceneManager.gameObject.name, playerScene.name);

            if(playerScene.name != "DontDestroyOnLoad") //(playerScene.buildIndex == 0)
            {
                Debug.LogFormat("Marked for deletion: {0} in scene: {1}", sceneManager.gameObject.name, playerScene.name);
                gameObjToDelete = sceneManager.gameObject;
            }
            else 
            {
                //.. is DontDestroyOnLoad and therefore the original one
                _toKeep = sceneManager;
            }
        }
        if (gameObjToDelete != null)
        {
            Destroy(gameObjToDelete);
            Debug.Log("Destroyed duplicate player object in the initial scene.");
        }

        return _toKeep;
    }

#endregion
    #region MySceneManager Instance stuff...
    public void HandleDuplicateSceneManagerInScene()
    {
        MySceneManager[] foundSceneManagers = FindObjectsOfType<MySceneManager>();

        // If no sceneManager or only one sceneManager is found, no need for further processing.
        if (foundSceneManagers.Length <= 1)
        {
            original_MySceneManager = foundSceneManagers[0];
            return;
        }
        // ...else..
      original_MySceneManager =  GetCorrectSceneManager(foundSceneManagers);

        
    }
    private MySceneManager GetCorrectSceneManager(MySceneManager[] foundSceneManagers)
    {
        GameObject gameObjToDelete = null;
        MySceneManager _toKeep = null;
        foreach (MySceneManager playerManager in foundSceneManagers)
        {
            UnityEngine.SceneManagement.Scene playerScene = playerManager.gameObject.scene;

            Debug.LogFormat("{0} in scene: {1}", playerManager.gameObject.name, playerScene.name);

            if(playerScene.name != "DontDestroyOnLoad") //(playerScene.buildIndex == 0)
            {
                Debug.LogFormat("Marked for deletion: {0} in scene: {1}", playerManager.gameObject.name, playerScene.name);
                gameObjToDelete = playerManager.gameObject;
            }
            else 
            {
                //.. is DontDestroyOnLoad and therefore the original one
                _toKeep = playerManager;
            }
        }
        if (gameObjToDelete != null)
        {
            Destroy(gameObjToDelete);
            Debug.Log("Destroyed duplicate scenemanager object in the initial scene.");
        }

        return _toKeep;
    }

    public void Execute_SceneManager_StartCalibration()
    {
        original_MySceneManager.startCalibration();
    }
#endregion


#region AssetManager Instance stuff...

 public void HandleDuplicateAssetManagerInScene()
    {
        AssetManager[] foundAssetManagers = FindObjectsOfType<AssetManager>();

        // If no AssetManager or only one AssetManager is found, no need for further processing.
        if (foundAssetManagers.Length <= 1)
        {
            original_AssetManager = foundAssetManagers[0];
            return;
        }
        // ...else..
      original_AssetManager =  GetCorrectAssetManager(foundAssetManagers);

        
    }
    private AssetManager GetCorrectAssetManager(AssetManager[] foundAssetManagers)
    {
        GameObject gameObjToDelete = null;
        AssetManager _toKeep = null;
        foreach (AssetManager assetManager in foundAssetManagers)
        {
            UnityEngine.SceneManagement.Scene AssetManagerScene = assetManager.gameObject.scene;

            Debug.LogFormat("{0} in scene: {1}", assetManager.gameObject.name, AssetManagerScene.name);

            if(AssetManagerScene.name != "DontDestroyOnLoad") //(AssetManagerScene.buildIndex == 0)
            {
                Debug.LogFormat("Marked for deletion: {0} in scene: {1}", assetManager.gameObject.name, AssetManagerScene.name);
                gameObjToDelete = assetManager.gameObject;
            }
            else 
            {
                //.. is DontDestroyOnLoad and therefore the original one
                _toKeep = assetManager;
            }
        }
        if (gameObjToDelete != null)
        {
            Destroy(gameObjToDelete);
            Debug.Log("Destroyed duplicate AssetManager object in the initial scene.");
        }

        return _toKeep;
    }

#endregion
 */


}
