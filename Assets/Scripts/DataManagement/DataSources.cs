using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataManagement
{
    public enum DataAdapters
    {
        GPSInfo,
        BarentswatchAIS/* ,
        KystverketNAVAID */
    }

    public enum DataConnections
    {
        MockPhoneGPS,
        PhoneGPS,
        VesselGPS,
        HardcodedGPS,
        BarentswatchAIS/* ,
        KystverketNAVAID */
    }

    public enum ParameterExtractors
    {
        BarentswatchAIS,/* 
        KystverketNAVAID, */
        None
    }
}
