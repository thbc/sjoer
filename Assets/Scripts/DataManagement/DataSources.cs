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
        BarentswatchAIS,
        BarentswatchNAVAID
    }

    public enum DataConnections
    {
        MockPhoneGPS,
        PhoneGPS,
        VesselGPS,
        HardcodedGPS,
        BarentswatchAIS,
        BarentswatchNAVAID
    }

    public enum ParameterExtractors
    {
        BarentswatchAIS,
        BarentswatchNAVAID,
        None
    }
}
