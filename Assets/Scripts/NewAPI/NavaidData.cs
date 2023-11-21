using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
//using System.Web;
using UnityEngine;

namespace Assets.DataManagement.Navaids
{

    [Serializable]
    public class NavaidData
    {

        [Serializable]
        public class FeatureCollection
        {
            public List<Feature> features;
        }

        [Serializable]
        public class Feature
        {
            [NonSerialized]public GameObject gameObject;
            public string type;
            public Geometry geometry;
            public Properties properties;
        }

        [Serializable]
        public class Geometry
        {
            public string type;
            // Add any other necessary geometry fields here
        }

        [Serializable]
        public class Properties
        {
            public string GmlID;
            public int OBJECTID;
            public double Long;
            public double Lat;
            public string navn;
            public string sted;
            public string beliggenhet;
            public string TYPEID;
            public string GlobalID;
        }

        // Usage example
        public static FeatureCollection GetFeatureCollection(string jsonString)
        {
            FeatureCollection featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(jsonString);
            return featureCollection;
        }

        public static void DebugNavaidsInNavaidCollection(FeatureCollection featureCollection)
        {
            foreach (var feature in featureCollection.features)
            {
                // Use the properties as needed
                Debug.Log("Longitude: " + feature.properties.Long);
                Debug.Log("Latitude: " + feature.properties.Lat);
                Debug.Log("Name: " + feature.properties.navn);
                Debug.Log("Place: " + feature.properties.sted);
                Debug.Log("Location: " + feature.properties.beliggenhet);
                Debug.Log("Type ID: " + feature.properties.TYPEID);
                Debug.Log("Global ID: " + feature.properties.GlobalID);
            }
        }

        private string GetTestJsonString()
        {
            // Your JSON string here
            return "{\"navaids\":[{\"type\":\"Feature\",\"properties\":{\"Long\":5.15259852,\"Lat\":60.42475670,\"navn\":\"\",\"sted\":\"Hetlevik\",\"beliggenhet\":\"Askøy\",\"TYPEID\":\"StyrbordLateralMerke\",\"GlobalID\":\"{CFEAAE53-75B7-4ABD-8116-E6B7D35A5BD6}\"}},{\"type\":\"Feature\",\"properties\":{\"Long\":5.15334454,\"Lat\":60.42426616,\"navn\":\" \",\"sted\":\"Hetlevik\",\"beliggenhet\":\"Askøy\",\"TYPEID\":\"BabordLateralMerke\",\"GlobalID\":\"{EC5F3D5A-B948-4487-A096-6846F3AE77DB}\"}}]}";
        }


    }
}