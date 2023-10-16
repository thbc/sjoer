using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SceneManagement;
public class FindMySceneManager : MonoBehaviour
{
      public void ExecuteStartCalibration()
    {
        MySceneManager.Instance.startCalibration();
    }
}
