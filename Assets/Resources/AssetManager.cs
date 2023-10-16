using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Assets.HelperClasses;
using UnityEngine.SceneManagement;
namespace Assets.Resources
{
    public  class AssetManager : MonoSingleton<AssetManager>
    {
        public StringTextDictionary config;
        public StringGameObjectDictionary objects;
        
        void Start()
        {
            HandleDuplicateAssetManagerInScene();
        }

        private void HandleDuplicateAssetManagerInScene()
        {
            AssetManager[] foundAssetManagers = FindObjectsOfType<AssetManager>();

            // If no AssetManager or only one AssetManager is found, no need for further processing.
            if (foundAssetManagers.Length <= 1) return;

            GameObject gameObjToDelete = DetermineDuplicateAssetManager(foundAssetManagers);

            if (gameObjToDelete != null)
            {
                Destroy(gameObjToDelete);
                Debug.Log("Destroyed duplicate AssetManager object in the initial scene.");
            }
        }

        private GameObject DetermineDuplicateAssetManager(AssetManager[] foundAssetManagers)
        {
            GameObject gameObjToDelete = null;

            foreach (AssetManager AssetManager in foundAssetManagers)
            {
                Scene playerScene = AssetManager.gameObject.scene;

                Debug.LogFormat("{0} in scene: {1}", AssetManager.gameObject.name, playerScene.name);

                if (playerScene.buildIndex == 0)
                {
                    Debug.LogFormat("Marked for deletion: {0} in scene: {1}", AssetManager.gameObject.name, playerScene.name);
                    gameObjToDelete = AssetManager.gameObject;
                }
            }

            return gameObjToDelete;
        }
    }
}
