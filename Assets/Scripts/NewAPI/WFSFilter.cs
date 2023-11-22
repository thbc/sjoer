using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
//using System.Web;
using UnityEngine;
namespace Assets.DataManagement.Navaids.WFSRequestHelper
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
        public static string Request_SeamarkersBeacon(double minLong, double maxLong, double minLat, double maxLat)
        {
            string[] typeIds = new string[] { "BabordLateralMerke", "StyrbordLateralMerke" };
            string wfsRequestUrl = BuildWfsRequestUrl(new string[]{"Fastsjømerke","Flytendemerke"}, minLong, maxLong, minLat, maxLat,typeIds);
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
        public static string Request_SeamarkersCardinal(double minLong, double maxLong, double minLat, double maxLat)
        {
            string[] typeIds = new string[] { "NordKardinalBøye", "ØstKardinalBøye", "SørKardinalBøye", "VestKardinalBøye" };
            string wfsRequestUrl = BuildWfsRequestUrl(new string[]{"Flytendemerke"}, minLong, maxLong, minLat, maxLat,typeIds);
            return wfsRequestUrl;
        }

           public static string Request_SeamarkersLight(double minLong, double maxLong, double minLat, double maxLat)
        {
            //string[] typeIds = new string[] { "NordKardinalBøye", "ØstKardinalBøye", "SørKardinalBøye", "VestKardinalBøye" };
            string wfsRequestUrl = BuildWfsRequestUrl(new string[]{"Lys"}, minLong, maxLong, minLat, maxLat);
            return wfsRequestUrl;
        }
        public static string BuildWfsRequestUrl(string[] typeNames, double minLong, double maxLong, double minLat, double maxLat,string[] typeIds=null)
        {
            string fullTypeName="";
            
            for (int i = 0; i < typeNames.Length; i++)            
            {
                typeNames[i] = "NFSSistOperativ:" + typeNames[i];
                if(string.IsNullOrEmpty(fullTypeName))
                    fullTypeName = typeNames[i];
                else fullTypeName = fullTypeName+ "," + typeNames[i];

            }

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
            string keepProperties = "propertyName=nfs_NFSSistOperativ:Long,nfs_NFSSistOperativ:Lat,nfs_NFSSistOperativ:navn,nfs_NFSSistOperativ:sted,nfs_NFSSistOperativ:beliggenhet,nfs_NFSSistOperativ:TYPEID,nfs_NFSSistOperativ:GlobalID&";

            string encodedFilter = UnityWebRequest.EscapeURL(filter.ToString(SaveOptions.DisableFormatting));
            return $"{baseWfsUrl}?service=WFS&request=GetFeature&version=2.0.0&typename={fullTypeName}&outputFormat=CSV&{keepProperties}Filter={encodedFilter}";
        }

    }
}