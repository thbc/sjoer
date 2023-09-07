using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Calibration
{
    class BowAligner : MonoBehaviour
    {
        [SerializeField]
        private GameObject dash;

        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private int dashCount = 10;

        private GameObject[] dashArray;

        void Start()
        {
            dashArray = new GameObject[dashCount];
            dashArray[0] = dash;

            float dashLength = Vector3.Scale(dash.transform.localScale, transform.localScale).z;
            float dashDistance = 0.3f;
            for(int i = 1; i < dashCount; i++)
            {
                dashArray[i] = Instantiate(
                    dash, 
                    dash.transform.position - Vector3.forward * (i * (dashLength + dashDistance)),
                    Quaternion.identity,
                    this.transform);
            }
        }

        void Update()
        {
            this.transform.position = mainCamera.transform.position + (mainCamera.transform.forward * 3);
            this.transform.rotation = mainCamera.transform.rotation;
        }
    }
}
