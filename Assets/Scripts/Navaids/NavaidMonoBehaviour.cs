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
        public MeshRenderer backgroundMesh;
        private Material defaultMat;
        public Material inactiveMat;

        void Start()
        {
           defaultMat = backgroundMesh.material;
        }
        public void setTypeText(string _type)
        {
            typeText.text = _type;
        }
        public void setDistanceText(float _distance)
        {
            distanceText.text = ((int)_distance).ToString()+ "m";
        }
        public void DisplayContent(bool _state)
        {
            if(_state)
            backgroundMesh.material = defaultMat;
            else
            backgroundMesh.material = inactiveMat;
            
            CanvasTextGO.SetActive(_state);
            
        }

    }
}