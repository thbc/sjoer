using UnityEngine;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Assets.HelperClasses;

namespace Assets.SceneManagement
{
    public class MySceneManager : MonoSingleton<MySceneManager>
    {
        // TODO: Write loop here, that loops when not calibrating.
        private bool looping = true;
        void Start()
        {
            HandleDuplicateSceneManagerInScene();
        }

        private void HandleDuplicateSceneManagerInScene()
        {
            MySceneManager[] foundSceneManagers = FindObjectsOfType<MySceneManager>();

            // If no sceneManager or only one sceneManager is found, no need for further processing.
            if (foundSceneManagers.Length <= 1) return;

            GameObject gameObjToDelete = DetermineDuplicateSceneManager(foundSceneManagers);

            if (gameObjToDelete != null)
            {
                Destroy(gameObjToDelete);
                Debug.Log("Destroyed duplicate scenemanager object in the initial scene.");
            }
        }

        private GameObject DetermineDuplicateSceneManager(MySceneManager[] foundSceneManagers)
        {
            GameObject gameObjToDelete = null;

            foreach (MySceneManager sceneManager in foundSceneManagers)
            {
                Scene playerScene = sceneManager.gameObject.scene;

                Debug.LogFormat("{0} in scene: {1}", sceneManager.gameObject.name, playerScene.name);

                if (playerScene.buildIndex == 0)
                {
                    Debug.LogFormat("Marked for deletion: {0} in scene: {1}", sceneManager.gameObject.name, playerScene.name);
                    gameObjToDelete = sceneManager.gameObject;
                }
            }

            return gameObjToDelete;
        }

        void Update()
        {

        }

        public void setNewScene(Scenes scene)
        {
            Debug.Log($"Setting next scene to {scene}");
            SceneManager.LoadScene((int)scene);
        }

        public void goNextScene(bool fromStart = false)
        {
            Scenes nextScene = fromStart ? (Scenes)0 : getCurrentScene();

            while (nextScene == Scenes.Calibration)
            {
                nextScene = getNextScene(nextScene);
            }

            setNewScene(nextScene);
        }

        private Scenes getCurrentScene()
        {
            return (Scenes)SceneManager.GetActiveScene().buildIndex;
        }

        private Scenes getNextScene(Scenes scene)
        {
            int nextScene = ((int)scene + 1);
            return Enum.GetValues(typeof(Scenes)).Length < nextScene ? (Scenes)0 : (Scenes)nextScene;
        }

        //This is now done from MonoBehaviour "FindMySceneManager"
        //// This is called from the SpeechInputHandler_Global when the command 'Calibrate North' is given 
        public void startCalibration()
        {
            looping = false;
            this.setNewScene(Scenes.Calibration);
        }

        public void exitCalibration()
        {
            looping = true;
            this.goNextScene(true);
        }

        // TODO: Implement Scene transitions here
    }
}
