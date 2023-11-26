using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Xml.Linq;


namespace Assets.DataManagement.Navaids
{
    [Serializable]
    public enum NavaidFeatureType
    {
        Undefined, BabordLateral, StyrbordLateral, Lyktefundament, Kardinal
    }
    public enum NavaidsGroup
    {
        All, StyrbordBabordLateral, Lyktefundament, Kardinal
    }
    [Serializable]
    public class NavaidData
    {
        public static int kardinalCounter;
        public static int StyrbordLateralCounter;
        public static int BabordLateralCounter;
        public static int LyktefundamentCounter;

        [Serializable]
        public class NavaidDict
        {
            public Dictionary<string, NavaidData.Navaid> navaids = new Dictionary<string, NavaidData.Navaid>();
        }

        [Serializable]
        public class Navaid
        {
            public Navaid() { }
            public double Longitude;
            public double Latitude;
            public string _Name;
          public string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            _Name = HelperClasses.StringSplitter.SplitString(value);
        }
    }
        public string _Place;
        public string Place
        {
            get
            {
                return _Place;
            }
            set
            {
                _Name = HelperClasses.StringSplitter.SplitString(value);//value.Length > 6 ? value.Substring(0, 6) : value;
            }
        }
        /*
                    public string Location; */
        public string _TypeID;
        public string TypeID
        {
            get
            {
                return _TypeID;
            }
            set
            {
                // NavaidType = ParseTypeID(value);
                _TypeID = value;
            }
        }
        public string GlobalID;
        // Add other properties as needed
        // [NonSerialized]
        public NavaidFeatureType NavaidType;
        [NonSerialized] public GameObject Shape;
        [NonSerialized] public NavaidMonoBehaviour navaidInstance;

        public static Navaid CreateIfDefinedType(string typeId, string globalID)
        {
            NavaidFeatureType navaidType = ParseTypeID(typeId);
            if (navaidType != NavaidFeatureType.Undefined)
            {
                return new Navaid
                {
                    GlobalID = globalID,
                    TypeID = typeId,
                    NavaidType = navaidType // Set directly since we already have the parsed type
                };
            }
            else
            {
                Debug.Log("Skipped Navaid: " + globalID + " (was Undefined: " + typeId + ").");
                return null; // Or handle undefined types as needed
            }
        }


        private static NavaidFeatureType ParseTypeID(string _id)
        {
            if (_id.Contains("Kardinal"))
            {
                kardinalCounter++;
                return NavaidFeatureType.Kardinal;
            }
            else if (_id.Contains("StyrbordLateral"))
            {
                StyrbordLateralCounter++;
                return NavaidFeatureType.StyrbordLateral;
            }
            else if (_id.Contains("Lyktefundament"))
            {
                LyktefundamentCounter++;
                return NavaidFeatureType.Lyktefundament;
            }
            else if (_id.Contains("BabordLateral"))
            {
                BabordLateralCounter++;
                return NavaidFeatureType.BabordLateral;
            }
            else
                return NavaidFeatureType.Undefined;

        }

        public bool IsPartOfGroup(NavaidsGroup group)
        {
            switch (group)
            {
                case NavaidsGroup.All:
                    return true;
                case NavaidsGroup.StyrbordBabordLateral:
                    if (NavaidType == NavaidFeatureType.BabordLateral || NavaidType == NavaidFeatureType.StyrbordLateral)
                        return true;
                    else return false;
                case NavaidsGroup.Lyktefundament:
                    if (NavaidType == NavaidFeatureType.Lyktefundament)
                        return true;
                    else return false;
                case NavaidsGroup.Kardinal:
                    if (NavaidType == NavaidFeatureType.Kardinal)
                        return true;
                    else return false;
                default: return false;
            }
        }
    }

    public class NavaidDataXMLParser
    {
        public List<Navaid> ParseXml(string xmlContent)
        {
            List<Navaid> navaids = new List<Navaid>();
            XNamespace nfs = "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer";

            try
            {
                XDocument xdoc = XDocument.Parse(xmlContent);

                // Process 'Lys' elements
                foreach (var element in xdoc.Descendants(nfs + "Lys"))
                {
                    navaids.Add(ProcessNavaidElement(element, nfs));
                }

                // Process 'IB' elements
                foreach (var element in xdoc.Descendants(nfs + "IB"))
                {
                    navaids.Add(ProcessNavaidElement(element, nfs));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing XML: " + ex.Message);
            }

            return navaids.Where(n => n != null).ToList(); // Filter out any null entries
        }

        private Navaid ProcessNavaidElement(XElement element, XNamespace nfs)
        {
            var globalIDElement = element.Element(nfs + "GlobalID");
            var typeIDElement = element.Element(nfs + "TYPEID");
            var longElement = element.Element(nfs + "Long");
            var latElement = element.Element(nfs + "Lat");
            var nameElement = element.Element(nfs + "navn");

            if (globalIDElement == null || typeIDElement == null || longElement == null || latElement == null)
            {
                // One of the critical elements is missing, so we cannot process this entry.
                Debug.LogWarning("A critical element is missing in the XML data.");
                return null;
            }

            string globalID = globalIDElement.Value;
            string typeId = typeIDElement.Value;
            Navaid newNavaid = Navaid.CreateIfDefinedType(typeId, globalID);

            if (newNavaid == null)
            {
                // The navaid is of an undefined type, so we cannot process it.
                return null;
            }

            double longitude = Convert.ToDouble(longElement.Value);
            double latitude = Convert.ToDouble(latElement.Value);


            newNavaid.Longitude = longitude;
            newNavaid.Latitude = latitude;
            if (!string.IsNullOrEmpty(nameElement.Value.ToString()))
            {
                newNavaid.Name = nameElement.Value.ToString();
            }
            else
            {
                // name is null, therefore we take location
                var placeElement = element.Element(nfs + "sted");

                if (!string.IsNullOrEmpty(placeElement.Value.ToString()))
                {
                    newNavaid.Name = placeElement.Value.ToString();
                }
            }

            return newNavaid;
        }
        /*  public List<Navaid> ParseXml(string xmlContent)
         {
             List<Navaid> navaids = new List<Navaid>();
             XNamespace nfs = "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer";

             try
             {
                 XDocument xdoc = XDocument.Parse(xmlContent);

                 foreach (var element in xdoc.Descendants(nfs + "Lys"))
                 {
                     // Extract the values you need from each element
                     string globalID = element.Element(nfs + "GlobalID").Value;
                     string typeId = element.Element(nfs + "TYPEID").Value;
                     Navaid newNavaid = Navaid.CreateIfDefinedType(typeId, globalID);

                     double longitude = Convert.ToDouble(element.Element(nfs + "Long").Value);
                     double latitude = Convert.ToDouble(element.Element(nfs + "Lat").Value);

                     newNavaid.Longitude = longitude;
                     newNavaid.Latitude = latitude;

                     //navaids.Add(navaid);

                     if (newNavaid != null)
                     {
                         if (newNavaid.NavaidType == NavaidFeatureType.Undefined)
                             Debug.LogWarning(newNavaid.GlobalID + " undefined: " + newNavaid.TypeID);

                         navaids.Add(newNavaid);
                     }
                 }
                 foreach (var element in xdoc.Descendants(nfs + "IB"))
                 {
                     // Extract the values you need from each element
                     string globalID = element.Element(nfs + "GlobalID").Value;
                     string typeId = element.Element(nfs + "TYPEID").Value;
                     Navaid newNavaid = Navaid.CreateIfDefinedType(typeId, globalID);

                     double longitude = Convert.ToDouble(element.Element(nfs + "Long").Value);
                     double latitude = Convert.ToDouble(element.Element(nfs + "Lat").Value);

                     newNavaid.Longitude = longitude;
                     newNavaid.Latitude = latitude;

                     //navaids.Add(navaid);

                     if (newNavaid != null)
                     {
                         if (newNavaid.NavaidType == NavaidFeatureType.Undefined)
                             Debug.LogWarning(newNavaid.GlobalID + " undefined: " + newNavaid.TypeID);

                         navaids.Add(newNavaid);
                     }
                 }
             }
             catch (Exception ex)
             {
                 Debug.LogError("Error parsing XML: " + ex.Message);
             }
             return navaids;
         } */
    }

    public class NavaidDataCSVParser
    {
        private CsvParser csvParser = new CsvParser();

        public List<Navaid> GetNavaidFeaturesFromString(string csvData)
        {
            //                Debug.LogWarning("Getnavaiddatafromstring: "+ csvData);
            List<Navaid> features = new List<Navaid>();
            var parsedLines = csvParser.ParseCsv(csvData);
            //  Debug.LogWarning("header: " + parsedLines[0]);


            // Skip the first line as it's the header
            foreach (var fields in parsedLines.Skip(1))
            {
                // Parse fields into a Navaid object
                var feature = ParseFieldsIntoFeature(fields);
                if (feature != null)
                {
                    if (feature.NavaidType == NavaidFeatureType.Undefined)
                        Debug.LogWarning(feature.GlobalID + " undefined: " + feature.TypeID);

                    features.Add(feature);
                }
            }

            return features;
        }



        private static Navaid ParseFieldsIntoFeature(string[] fields)
        {
            try
            {
                var typeID = fields[4].Trim();
                var globalID = fields[5].Trim();
                Navaid newNavaid = Navaid.CreateIfDefinedType(typeID, globalID);
                if (newNavaid == null)
                {
                    return null;
                }

                // Assuming the fields[0] is 'GmlId' and fields[1] is 'OBJECTID', and you want to start from fields[2]
                var longitude = double.Parse(fields[2].Trim());
                var latitude = double.Parse(fields[3].Trim());
                newNavaid.Latitude = latitude;
                newNavaid.Longitude = longitude;
                return newNavaid;
                //   var name = fields[4].Trim();
                //   var place = fields[5].Trim();
                //  var location = fields[6].Trim(); 


                /* var longitude = double.Parse(fields[2].Trim());
                var latitude = double.Parse(fields[3].Trim());
                //   var name = fields[4].Trim();
               //   var place = fields[5].Trim();
                //  var location = fields[6].Trim(); 
                var typeID = fields[4].Trim();
                var globalID = fields[5].Trim();

                return new Navaid
                {
                    Longitude = longitude,
                    Latitude = latitude,
                    //  Name = name,
                    // Place = place,
                   //  Location = location, 
                    TypeID = typeID,
                    GlobalID = globalID
                }; */
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing feature: " + fields[5] + ex.Message);
                return null;
            }
        }

    }
}



public class CsvParser
{
    public IEnumerable<string[]> ParseCsv(string csvContent)
    {
        string[] lines = csvContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // var lines = csvContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            //  Debug.LogWarning(line);
            bool insideQuote = false;
            var fields = new List<string>();
            var currentField = new StringBuilder();

            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    insideQuote = !insideQuote;
                }
                else if (ch == ',' && !insideQuote)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(ch);
                }
            }

            if (currentField.Length > 0)
            {
                fields.Add(currentField.ToString());
            }

            yield return fields.ToArray();
        }
    }
}



}