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
        BarentswatchAIS
    }

    public enum DataConnections
    {
        MockPhoneGPS,
        PhoneGPS,
        VesselGPS,
        HardcodedGPS,
        BarentswatchAIS
    }

    public enum ParameterExtractors
    {
        BarentswatchAIS,
        None
    }
}
