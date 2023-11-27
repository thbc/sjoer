using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
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
            float rng = _distance / 1852;


            distanceText.text = Math.Round(rng, 3).ToString() + "NM";// ((int)_distance).ToString()+ "m";
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