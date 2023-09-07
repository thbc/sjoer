using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Assets.Resources;
using Assets.HelperClasses;

namespace Assets.DataManagement
{
    abstract class DataAdapter
    {
        // Takes in a string (JSON?) and spits out DTO object
        public abstract DTO convert(string input);
    }

    class BarentswatchAISDataAdapter : DataAdapter
    {
        private double getDouble(JObject vessel, string s)
        {
            JToken val = vessel.GetValue(s);
            return val.Type != JTokenType.Null ? val.ToObject<double>() : double.NaN;
        }

        private string getString(JObject vessel, string s)
        {
            JToken val = vessel.GetValue(s);
            return val.Type != JTokenType.Null ? val.ToObject<string>() : string.Empty;
        }

        private int getInt(JObject vessel, string s)
        {
            JToken val = vessel.GetValue(s);
            return val.Type != JTokenType.Null ? val.ToObject<int>() : 0;
        }

        private DateTime getDateTime(JObject vessel, string s)
        {
            JToken val = vessel.GetValue(s);
            return val.Type != JTokenType.Null ? val.ToObject<DateTime>() : DateTime.MinValue;
        }

        private Tuple<double, double> getLatLon(JObject vessel, string s)
        {
            JToken val = vessel.SelectToken(s);
            return val.Type != JTokenType.Null 
                ? new Tuple<double, double>((double)val[0], (double)val[1]) 
                : new Tuple<double, double>(double.NaN, double.NaN);
        }

        public override DTO convert(string input)
        {
            AISDTOs dto = new AISDTOs();

            if (input == "err")
            {
                dto.Valid = false;
                return dto;
            }

            JArray vessels = JsonConvert.DeserializeObject<JArray>(input);
            dto.vessels = new AISDTO[vessels.Count];
            dto.Valid = true;

            int i = 0;
            foreach (JObject vessel in vessels)
            {

                AISDTO vesselDTO = new AISDTO();

                vesselDTO.Name        = getString(vessel, "name");
                vesselDTO.Key         = vesselDTO.Name;

                // Skip our own vessel when we are in vessel mode
                if (Config.Instance.conf.VesselMode &&
                    vesselDTO.Name == Config.Instance.conf.VesselSettingsS["VesselName"])
                    continue;

                // By default, no target. When connecting to ECDIS this could become useful
                vesselDTO.Target      = false;

                vesselDTO.Valid       = true;
                vesselDTO.TimeStamp   = getDateTime(vessel, "timeStamp");
                vesselDTO.SOG         = getDouble(vessel, "sog");
                vesselDTO.Rot         = getDouble(vessel, "rot");
                vesselDTO.NavStat     = getDouble(vessel, "navstat");
                vesselDTO.MMSI        = getInt(vessel, "mmsi");
                vesselDTO.COG         = getDouble(vessel, "cog");
                vesselDTO.ShipType    = getInt(vessel, "shipType");
                vesselDTO.IMO         = getInt(vessel, "imo");
                vesselDTO.CallSign    = getString(vessel, "callsign");
                vesselDTO.Draught     = getDouble(vessel, "draught");
                vesselDTO.Heading     = getDouble(vessel, "heading");
                vesselDTO.Destination = getString(vessel, "destination");
                vesselDTO.ETA         = getDateTime(vessel, "eta");
                vesselDTO.Country     = getString(vessel, "country");

                Tuple<double, double> LatLon = getLatLon(vessel, "geometry.coordinates");
                vesselDTO.Longitude   = LatLon.Item1;
                vesselDTO.Latitude    = LatLon.Item2;

                dto.vessels[i] = vesselDTO;
                i++;
            }

            return dto;
        }
    }

    class GPSInfoAdapter : DataAdapter
    {
        // Code courtesy of Carson63000 at https://stackoverflow.com/questions/5089791/nmea-checksum-in-c-sharp-net-cf
        private bool checksum(string input)
        {
            string cs = input.Split('*')[1];
            int dollarIndex = input.IndexOf('$');

            //Start with first Item
            int checksum = Convert.ToByte(input[dollarIndex + 1]);
            // Loop through all chars to get a checksum
            int starIndex = input.IndexOf('*');
            for (int i = dollarIndex + 2; i < starIndex; i++)
            {
                // No. XOR the checksum with this character's value
                checksum ^= Convert.ToByte(input[i]);
            }
            // Return the checksum formatted as a two-character hexadecimal
            return cs == checksum.ToString("X2");
        }

        // Input: $GPRMC,071228.00,A,5402.6015,N,00025.9797,E,0.2,332.1,180921,0.2,W,A,S*50
        // Input: $GPRMC,153415.692,A,6023.762,N,00519.311,E,082.0,289.8,151121,000.0,W* 7C --> phone mock

        public override DTO convert(string input)
        {
            // We'll ignore index 0, because that contains '$GPRMC'
            string[] splitInput = input.Split(',');

            AISDTO dto = new AISDTO();

            // Catch invalid DTO
            if (splitInput.Length < 12) // || !checksum(input) || splitInput[2] == "V")
            {
                dto.Valid = false;
            } else
            {
                dto.Valid = true;

                string t = splitInput[1];
                dto.Latitude = GPSUtils.Instance.DMSToDecimal(splitInput[3]);
                //dto.LatNS = splitInput[4];
                dto.Longitude = GPSUtils.Instance.DMSToDecimal(splitInput[5].Remove(0, 1));
                //dto.LongEW = splitInput[6];
                dto.SOG = double.Parse(splitInput[7]);
                dto.Heading = splitInput[8] == "" ? 0.0 : double.Parse(splitInput[8]);
                string d = splitInput[9];
                dto.TimeStamp = this.generateDT(t, d);
                //dto.Variation = double.Parse(splitInput[10]);
                //dto.VarEW = splitInput[11];
            }

            return dto;
        }

        private DateTime generateDT(string time, string date)
        {
            try
            {
                int ms = time.IndexOf(".");
                // Some NMEA times have a random number of fractions of seconds attached, therefore, the format might vary
                string format = ms > 0 ? "ddMMyyHHmmss." + new string('f', time.Length - 1 - ms) : "ddMMyyHHmmss";
                return DateTime.ParseExact(date+time, format,
                    System.Globalization.CultureInfo.InvariantCulture);
            } catch (Exception e)
            {
                Debug.Log($"Error parrsing \"{date+time}\" DateTime: " + e);
                return DateTime.Now;
            }
        }
    }
}
