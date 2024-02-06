using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnHead : MonoBehaviour
{
    public static DebugOnHead Instance { get; private set; }

    public bool activeOnStart = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        this.gameObject.transform.GetChild(0).gameObject.SetActive(activeOnStart);

    }

    [HideInInspector]
    public string text_1;
    [HideInInspector]
    public string text_2;

    public void DebugTextOnHead_1(string _txt)
    {
      text_1 =_txt;
    }

    public void DebugTextOnHead_2(string _txt)
    {
        text_2 = _txt;
    }

}
