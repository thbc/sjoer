using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
//using System.Web;
using UnityEngine;
using Assets.Positional;
namespace Assets.DataManagement.Navaids.WFSRequestHelper
{

    public class WFSRequestBuilder
    {
        private static readonly string ogcNamespace = "http://www.opengis.net/ogc";
        private static readonly string typeName = "nfs_NFSSistOperativ:Fastsjømerke";
        private static readonly string baseWfsUrl = "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer";

        /*
        These are the queries available on the WFS server:
     !   Lys    
        Sektorlinje
        Lyssektor
        Racon
     !   Fastsjømerke
     !   Flytendemerke
        Overettlinje__lys
        Overettlinje__merke
        */

        /// <summary>
        /// The Seamarkers, in our case PortHand and StarboardHand.
        /// </summary>
        /// <param name="typeIds"></param>
        /// <param name="minLong"></param>
        /// <param name="maxLong"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <returns></returns>
       /*  public static string Request_SeamarkersBeacon(double minLong, double maxLong, double minLat, double maxLat)
        {
            string[] typeIds = new string[] { "BabordLateralMerke", "StyrbordLateralMerke" };//,"NordKardinalBøye", "ØstKardinalBøye", "SørKardinalBøye", "VestKardinalBøye" };
            string wfsRequestUrl = BuildWfsRequestUrl(new string[] { "Fastsjømerke", "Flytendemerke" }, minLong, maxLong, minLat, maxLat, typeIds);
            return wfsRequestUrl;
        } */
        /// <summary>
        /// Flytendemerke:- Cardinal Mark N (Flytendemerke, NordKardinalBøye), - Cardinal Mark E (Flytendemerke, ØstKardinalBøye), - Cardinal Mark S (Flytendemerke, SørKardinalBøye), - Cardinal Mark W (Flytendemerke,VestKardinalBøye)
        /// </summary>
        /// <param name="minLong"></param>
        /// <param name="maxLong"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <returns></returns>
       /*  public static string Request_SeamarkersCardinal(double minLong, double maxLong, double minLat, double maxLat)
        {
            string[] typeIds = new string[] { "NordKardinalBøye", "ØstKardinalBøye", "SørKardinalBøye", "VestKardinalBøye" };
            string wfsRequestUrl = BuildWfsRequestUrl(new string[] { "Flytendemerke" }, minLong, maxLong, minLat, maxLat, typeIds);
            return wfsRequestUrl;
        }

        public static string Request_SeamarkersLight(double minLong, double maxLong, double minLat, double maxLat)
        {
            //string[] typeIds = new string[] { "NordKardinalBøye", "ØstKardinalBøye", "SørKardinalBøye", "VestKardinalBøye" };
            string wfsRequestUrl = BuildWfsRequestUrl(new string[] { "Lys" }, minLong, maxLong, minLat, maxLat);
            return wfsRequestUrl;
        } */

        public static string Request_SeamarkersAll(double minLong, double maxLong, double minLat, double maxLat, string _typename)
        {
            //  string[] typeIds = new string[] { "BabordLateralMerke", "StyrbordLateralMerke"};//,"NordKardinalBøye", "ØstKardinalBøye", "SørKardinalBøye", "VestKardinalBøye" };

           // string wfsRequestUrl = BuildWfsRequestUrl(new string[] { "IB", "Lys" }, minLong, maxLong, minLat, maxLat); //"Fastsjømerke","IB","Lys","Flytendemerke" 
                       string wfsRequestUrl = BuildWfsRequestUrl(minLong, maxLong, minLat, maxLat,_typename); //"Fastsjømerke","IB","Lys","Flytendemerke" 

            return wfsRequestUrl;
        }


        public static string BuildWfsRequestUrl(double minLong, double maxLong, double minLat, double maxLat, string _typename)// = NavaidsGroup.Lyktefundament)
        {
            string fullTypeName="nfs_NFSSistOperativ:"+_typename;
            string desiredTypeID="Lyktefundament";
            /* if(group2Retrieve== NavaidsGroup.Lyktefundament)
            {
                fullTypeName = "nfs_NFSSistOperativ:Lys,nfs_NFSSistOperativ:IB";//string.Join(":","nfs_NFSSistOperativ","IB");//string.Join(",",string.Join(":","nfs_NFSSistOperativ","Lys"),string.Join(":","nfs_NFSSistOperativ","IB"));//"Lys");
                desiredTypeID="Lyktefundament";
            } */
            

            return $"{baseWfsUrl}?service=WFS&request=GetFeature&version=2.0.0&typename="+fullTypeName+"&propertyName=nfs_NFSSistOperativ:Long,nfs_NFSSistOperativ:Lat,nfs_NFSSistOperativ:TYPEID,nfs_NFSSistOperativ:GlobalID,nfs_NFSSistOperativ:navn&Filter=%3Cogc:Filter%3E%3Cogc:And%3E%3Cogc:PropertyIsEqualTo%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:TYPEID%3C/ogc:PropertyName%3E%3Cogc:Literal%3E"+desiredTypeID+"%3C/ogc:Literal%3E%3C/ogc:PropertyIsEqualTo%3E%3Cogc:PropertyIsGreaterThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Long%3C/ogc:PropertyName%3E%3Cogc:Literal%3E"+minLong.ToString()+"%3C/ogc:Literal%3E%3C/ogc:PropertyIsGreaterThan%3E%3Cogc:PropertyIsLessThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Long%3C/ogc:PropertyName%3E%3Cogc:Literal%3E"+maxLong.ToString()+"%3C/ogc:Literal%3E%3C/ogc:PropertyIsLessThan%3E%3Cogc:PropertyIsGreaterThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Lat%3C/ogc:PropertyName%3E%3Cogc:Literal%3E"+minLat.ToString()+"%3C/ogc:Literal%3E%3C/ogc:PropertyIsGreaterThan%3E%3Cogc:PropertyIsLessThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Lat%3C/ogc:PropertyName%3E%3Cogc:Literal%3E"+maxLat.ToString()+"%3C/ogc:Literal%3E%3C/ogc:PropertyIsLessThan%3E%3C/ogc:And%3E%3C/ogc:Filter%3E";
            // test string for returning all entries of feature "Lys" that have TYPEID="Lyktefundament", and that are within Lon/Lat area (==boundingBox)
            //return "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer?service=WFS&request=GetFeature&version=2.0.0&typename=nfs_NFSSistOperativ:Lys&propertyName=nfs_NFSSistOperativ:Long,nfs_NFSSistOperativ:Lat,nfs_NFSSistOperativ:TYPEID,nfs_NFSSistOperativ:GlobalID&Filter=%3Cogc:Filter%3E%3Cogc:And%3E%3Cogc:PropertyIsEqualTo%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:TYPEID%3C/ogc:PropertyName%3E%3Cogc:Literal%3ELyktefundament%3C/ogc:Literal%3E%3C/ogc:PropertyIsEqualTo%3E%3Cogc:PropertyIsGreaterThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Long%3C/ogc:PropertyName%3E%3Cogc:Literal%3E5.135830%3C/ogc:Literal%3E%3C/ogc:PropertyIsGreaterThan%3E%3Cogc:PropertyIsLessThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Long%3C/ogc:PropertyName%3E%3Cogc:Literal%3E5.167121%3C/ogc:Literal%3E%3C/ogc:PropertyIsLessThan%3E%3Cogc:PropertyIsGreaterThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Lat%3C/ogc:PropertyName%3E%3Cogc:Literal%3E60.417901%3C/ogc:Literal%3E%3C/ogc:PropertyIsGreaterThan%3E%3Cogc:PropertyIsLessThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Lat%3C/ogc:PropertyName%3E%3Cogc:Literal%3E60.430572%3C/ogc:Literal%3E%3C/ogc:PropertyIsLessThan%3E%3C/ogc:And%3E%3C/ogc:Filter%3E";
        }

    }
}


// Other request experiments:
/* string fullTypeName = string.Join(",", typeNames.Select(tn => $"NFSSistOperativ:{tn}"));
           var v1 = Player.Instance.GetWorldTransform(minLat, minLong);
           var v2 = Player.Instance.GetWorldTransform(maxLat, maxLong);
           float dist = (v1 - v2).magnitude;


           Debug.LogWarning("BBOX: " + dist);

           XNamespace ogc_ = "http://www.opengis.net/ogc";
           var filter = new XElement(ogc_ + "Filter",
               new XElement(ogc_ + "And",
                   new XElement(ogc_ + "PropertyIsGreaterThanOrEqualTo",
                       new XElement(ogc_ + "PropertyName", "Long"),
                       new XElement(ogc_ + "Literal", "5.1")
                   ),
                   new XElement(ogc_ + "PropertyIsLessThanOrEqualTo",
                       new XElement(ogc_ + "PropertyName", "Long"),
                       new XElement(ogc_ + "Literal", "5.1")
                   ),
                   new XElement(ogc_ + "PropertyIsGreaterThanOrEqualTo",
                       new XElement(ogc_ + "PropertyName", "Lat"),
                       new XElement(ogc_ + "Literal", "60.3")
                   ),
                   new XElement(ogc_ + "PropertyIsLessThanOrEqualTo",
                       new XElement(ogc_ + "PropertyName", "Lat"),
                       new XElement(ogc_ + "Literal", "60.3")
                   )
               )
           );
           string keepProperties = string.Join(",", new[]
           {
       "nfs_NFSSistOperativ:Long",
       "nfs_NFSSistOperativ:Lat",

       "nfs_NFSSistOperativ:TYPEID",
       "nfs_NFSSistOperativ:GlobalID"

   });

           string encodedFilter = UnityWebRequest.EscapeURL(filter.ToString(SaveOptions.DisableFormatting));
           string wfsRequestUrl = $"{baseWfsUrl}?service=WFS&request=GetFeature&version=2.0.0&typename={fullTypeName}&outputFormat=CSV&propertyName={keepProperties}&Filter={encodedFilter}"; 

        string _typename="";
        if(group2Retrieve == NavaidsGroup.Lyktefundament)
        {_typename= "nfs_NFSSistOperativ:Lys,nfs_NFSSistOperativ:IB";}
       wfsRequestUrl= $"{baseWfsUrl}?service=WFS&request=GetFeature&version=2.0.0&typename={_typename}&outputFormat=CSV&Filter=%3Cogc:Filter%3E%3Cogc:And%3E%3Cogc:PropertyIsEqualTo%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:TYPEID%3C/ogc:PropertyName%3E%3Cogc:Literal%3ELyktefundament%3C/ogc:Literal%3E%3C/ogc:PropertyIsEqualTo%3E%3Cogc:PropertyIsGreaterThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Long%3C/ogc:PropertyName%3E%3Cogc:Literal%3E5.135830%3C/ogc:Literal%3E%3C/ogc:PropertyIsGreaterThan%3E%3Cogc:PropertyIsLessThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Long%3C/ogc:PropertyName%3E%3Cogc:Literal%3E5.167121%3C/ogc:Literal%3E%3C/ogc:PropertyIsLessThan%3E%3Cogc:PropertyIsGreaterThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Lat%3C/ogc:PropertyName%3E%3Cogc:Literal%3E60.417901%3C/ogc:Literal%3E%3C/ogc:PropertyIsGreaterThan%3E%3Cogc:PropertyIsLessThan%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:Lat%3C/ogc:PropertyName%3E%3Cogc:Literal%3E60.430572%3C/ogc:Literal%3E%3C/ogc:PropertyIsLessThan%3E%3C/ogc:And%3E%3C/ogc:Filter%3E";
           return wfsRequestUrl; */
/*   XNamespace ogc = ogcNamespace;

  //weirdly this only seems to work when we look for "Fastsjømerke":
  XElement typeIdFilter = null;
  if (typeIds != null && typeIds.Length > 0)
  {
      typeIdFilter = new XElement(ogc + "Or",
          typeIds.Select(typeId =>
              new XElement(ogc + "PropertyIsEqualTo",
                  new XElement(ogc + "PropertyName", "nfs_NFSSistOperativ:TYPEID"),
                  new XElement(ogc + "Literal", typeId))));
  }



  var filterElements = new List<XElement>
{
typeIdFilter, // This will be null if typeIds is null or empty, which is fine for XML construction. 
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
}.Where(x => x != null); // Removes the null typeIdFilter if it's not needed.

  XElement filter = new XElement(ogc + "Filter",
      new XElement(ogc + "And", filterElements));

  string encodedFilter = UnityWebRequest.EscapeURL(filter.ToString(SaveOptions.DisableFormatting));

  // It's better to use string.Join to avoid manually concatenating with commas.
  string keepProperties = string.Join(",", new[]
  {
"nfs_NFSSistOperativ:Long",
"nfs_NFSSistOperativ:Lat",

"nfs_NFSSistOperativ:TYPEID",
"nfs_NFSSistOperativ:GlobalID"

});
  return "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer?service=WFS&request=GetFeature&version=2.0.0&typename=NFSSistOperativ:NFSSistOperativ:Lys&outputFormat=CSV&propertyName=nfs_NFSSistOperativ:Long,nfs_NFSSistOperativ:Lat,nfs_NFSSistOperativ:TYPEID,nfs_NFSSistOperativ:GlobalID&Filter=%3CFilter%20xmlns=%22http://www.opengis.net/ogc%22%3E%3CAnd%3E%3CPropertyIsGreaterThanOrEqualTo%3E%3CPropertyName%3ELong%3C/PropertyName%3E%3CLiteral%3E5.3%3C/Literal%3E%3C/PropertyIsGreaterThanOrEqualTo%3E%3CPropertyIsLessThanOrEqualTo%3E%3CPropertyName%3ELong%3C/PropertyName%3E%3CLiteral%3E5.4%3C/Literal%3E%3C/PropertyIsLessThanOrEqualTo%3E%3CPropertyIsEqualTo%3E%3CPropertyName%3ELat%3C/PropertyName%3E%3CLiteral%3E60.4%3C/Literal%3E%3C/PropertyIsEqualTo%3E%3C/And%3E%3C/Filter%3E";
  //return $"{baseWfsUrl}?service=WFS&request=GetFeature&version=2.0.0&typename={fullTypeName}&outputFormat=CSV&propertyName={keepProperties}&Filter=%3Cogc:Filter%3E%3Cogc:And%3E%3Cogc:PropertyIsEqualTo%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:TYPEID%3C/ogc:PropertyName%3E%3Cogc:Literal%3EBabordLateralMerke%3C/ogc:Literal%3E%3C/ogc:PropertyIsEqualTo%3E%3Cogc:BBOX%3E%3Cogc:PropertyName%3Enfs_NFSSistOperativ:SHAPE%3C/ogc:PropertyName%3E%3Cgml:Envelope%20srsName=%22urn:ogc:def:crs:EPSG::25833%22%3E%3Cgml:lowerCorner%3E5.135830%2060.417901%3C/gml:lowerCorner%3E%3Cgml:upperCorner%3E5.167121%2060.430572%3C/gml:upperCorner%3E%3C/gml:Envelope%3E%3C/ogc:BBOX%3E%3C/ogc:And%3E%3C/ogc:Filter%3E";
  //   return $"{baseWfsUrl}?service=WFS&request=GetFeature&version=2.0.0&typename={fullTypeName}&outputFormat=CSV&propertyName={keepProperties}&bbox={minLong.ToString()},{minLat.ToString()},{maxLong.ToString()},{maxLat.ToString()}";//&Filter={encodedFilter}";
*/