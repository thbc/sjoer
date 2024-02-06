using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TextMesh))]
public class DebugTextMesh : MonoBehaviour
{
    TextMesh debugText;

    public int debugTextIndex = 1;
    
    private void Start()
    {
      debugText = GetComponent<TextMesh>();
    }
    private void Update()
    {
        if(debugTextIndex == 1)
            debugText.text = DebugOnHead.Instance.text_1;
        else if(debugTextIndex == 2)
            debugText.text = DebugOnHead.Instance.text_2;
    }

}
