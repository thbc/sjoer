using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Assets.DataManagement.Navaids
{
    public class NavaidMonoBehaviour : MonoBehaviour
    {
        public GameObject CanvasTextGO;
        public TextMeshProUGUI typeText;
        public TextMeshProUGUI distanceText;
        public void setTypeText(string _type)
        {
            typeText.text = _type;
        }
        public void setDistanceText(float _distance)
        {
            distanceText.text = ((int)_distance).ToString()+ "m (distance)";
        }
        public void DisplayContent(bool _state)
        {
            CanvasTextGO.SetActive(_state);
        }

    }
}