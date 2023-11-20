using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
//using System.Web;
using UnityEngine;

namespace WFSRequestHelper
{

    public class WFSRequestBuilder
    {
        private static readonly string ogcNamespace = "http://www.opengis.net/ogc";
        private static readonly string typeName = "nfs_NFSSistOperativ:Fastsjømerke";
        private static readonly string baseWfsUrl = "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer";

        /// <summary>
        /// The Seamarkers, in our case PortHand and StarboardHand.
        /// </summary>
        /// <param name="typeIds"></param>
        /// <param name="minLong"></param>
        /// <param name="maxLong"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <returns></returns>
        public static string Request_Seamarkers(double minLong, double maxLong, double minLat, double maxLat)
        {
            string[] typeIds = new string[] { "BabordLateralMerke", "StyrbordLateralMerke" };
            string wfsRequestUrl = BuildWfsRequestUrl("Fastsjømerke", typeIds, minLong, maxLong, minLat, maxLat);
            return wfsRequestUrl;
        }
        /// <summary>
        /// Flytendemerke:- Cardinal Mark N (Flytendemerke, NordKardinalBøye), - Cardinal Mark E (Flytendemerke, ØstKardinalBøye), - Cardinal Mark S (Flytendemerke, SørKardinalBøye), - Cardinal Mark W (Flytendemerke,VestKardinalBøye)
        /// </summary>
        /// <param name="minLong"></param>
        /// <param name="maxLong"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <returns></returns>
        public static string Request_KardinalMark(double minLong, double maxLong, double minLat, double maxLat)
        {
            string[] typeIds = new string[] { "NordKardinalBøye", "ØstKardinalBøye", "SørKardinalBøye", "VestKardinalBøye" };
            string wfsRequestUrl = BuildWfsRequestUrl("Flytendemerke", typeIds, minLong, maxLong, minLat, maxLat);
            return wfsRequestUrl;
        }
        public static string BuildWfsRequestUrl(string typeName, string[] typeIds, double minLong, double maxLong, double minLat, double maxLat)
        {
            string fullTypeName = "NFSSistOperativ:" + typeName;
            XNamespace ogc = ogcNamespace;

            var typeIdFilters = typeIds.Select(typeId =>
                new XElement(ogc + "PropertyIsEqualTo",
                    new XElement(ogc + "PropertyName", "nfs_NFSSistOperativ:TYPEID"),
                    new XElement(ogc + "Literal", typeId)));

            XElement filter = new XElement(ogc + "Filter",
                new XElement(ogc + "And",
                    new XElement(ogc + "Or", typeIdFilters),
                    new XElement(ogc + "PropertyIsGreaterThan",
                        new XElement(ogc + "PropertyName", "nfs_NFSSistOperativ:Long"),
                        new XElement(ogc + "Literal", minLong.ToString())),
                    new XElement(ogc + "PropertyIsLessThan",
                        new XElement(ogc + "PropertyName", "nfs_NFSSistOperativ:Long"),
                        new XElement(ogc + "Literal", maxLong.ToString())),
                    new XElement(ogc + "PropertyIsGreaterThan",
                        new XElement(ogc + "PropertyName", "nfs_NFSSistOperativ:Lat"),
                        new XElement(ogc + "Literal", minLat.ToString())),
                    new XElement(ogc + "PropertyIsLessThan",
                        new XElement(ogc + "PropertyName", "nfs_NFSSistOperativ:Lat"),
                        new XElement(ogc + "Literal", maxLat.ToString()))
                )
            );

            string encodedFilter = UnityWebRequest.EscapeURL(filter.ToString(SaveOptions.DisableFormatting));
            string keepProperties = "propertyName=nfs_NFSSistOperativ:Long,nfs_NFSSistOperativ:Lat,nfs_NFSSistOperativ:navn,nfs_NFSSistOperativ:sted,nfs_NFSSistOperativ:beliggenhet,nfs_NFSSistOperativ:TYPEID,nfs_NFSSistOperativ:GlobalID&";

            return $"{baseWfsUrl}?service=WFS&request=GetFeature&version=2.0.0&typename={fullTypeName}&outputFormat=GEOJSON&{keepProperties}Filter={encodedFilter}";
        }

    }


    public class KystverketData
    {

        public class FeatureCollection
        {
            public List<Feature> features;
        }
        public class Feature
        {
            public FeatureGeometry geometry;
            public FeatureProperties properties;
        }
        public class FeatureGeometry
        {
            public string type;
        }
        public class FeatureProperties
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


        public static FeatureCollection GetFeatureCollection(string jsonString)
        {
            FeatureCollection featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(jsonString);

            return featureCollection;
        }

        public static void DebugFeaturesInFeatureCollection(FeatureCollection featureCollection)
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
            return "{\"features\":[{\"type\":\"Feature\",\"properties\":{\"Long\":5.15259852,\"Lat\":60.42475670,\"navn\":\"\",\"sted\":\"Hetlevik\",\"beliggenhet\":\"Askøy\",\"TYPEID\":\"StyrbordLateralMerke\",\"GlobalID\":\"{CFEAAE53-75B7-4ABD-8116-E6B7D35A5BD6}\"}},{\"type\":\"Feature\",\"properties\":{\"Long\":5.15334454,\"Lat\":60.42426616,\"navn\":\" \",\"sted\":\"Hetlevik\",\"beliggenhet\":\"Askøy\",\"TYPEID\":\"BabordLateralMerke\",\"GlobalID\":\"{EC5F3D5A-B948-4487-A096-6846F3AE77DB}\"}}]}";
        }


    }
}