using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.DataManagement
{
    public abstract class DTO
    {
        public string Key { get; set; }
        public bool Target { get; set; }
        public int TargetNum { get; set; }
        public bool Valid { get; set; }


    }

    public class AISDTOs : DTO
    {
        public AISDTO[] vessels;
    }

    public class AISDTO : DTO
    {
        public DateTime TimeStamp { get; set; }
        public int MMSI { get; set; }
        public double SOG { get; set; }
        public double COG { get; set; }
        public double Rot { get; set; }
        public double Heading { get; set; }
        public double Draught { get; set; }
        public double NavStat { get; set; }
        public int ShipType { get; set; }
        public string CallSign { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Destination { get; set; }
        public DateTime ETA { get; set; }

        public string Name { get; set; }
        public int IMO { get; set; }
        public string Country { get; set; }


        public bool IsMarked { get; set; }

    }
    // --new class-------------------------

    public class NAVAIDDTOs : DTO
    {
        public NAVAIDDTO[] navaids;
    }

    public class NAVAIDDTO : DTO
    {
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }
        public string MessageType { get; set; }
        public int Mmsi { get; set; }
        public int DimensionA { get; set; }
        public int DimensionB { get; set; }
        public int DimensionC { get; set; }
        public int DimensionD { get; set; }
        public string TypeOfAidsToNavigation { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TypeOfElectronicFixingDevice { get; set; }

        public string Name { get; set; }

        //   public bool IsMarked { get; set; }

    }




    //public class GPSInfoDTO : DTO
    //{
    //    public DateTime DT { get; set; }
    //    public bool Valid { get; set; }
    //    public double Latitude { get; set; }
    //    public string LatNS { get; set; }
    //    public double Longitude { get; set; }
    //    public string LongEW { get; set; }
    //    public double SOG { get; set; }
    //    public double TrueCourse { get; set; }
    //    public double Variation { get; set; }
    //    public string VarEW { get; set; }
    //    public string Checksum { get; set; }
    //}
}
