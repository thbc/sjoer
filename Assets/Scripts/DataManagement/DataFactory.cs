using Assets.Positional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataManagement
{
    class DataFactory : HelperClasses.CSSingleton<DataFactory>
    {
        public Player aligner = null;

        public DataAdapter getDataAdapter(DataAdapters dataAdapter)
        {
            DataAdapter adapter;

            switch (dataAdapter)
            {
                case DataAdapters.GPSInfo:
                    adapter = new GPSInfoAdapter();
                    break;
                case DataAdapters.BarentswatchAIS:
                    adapter = new BarentswatchAISDataAdapter();
                    break;
                default:
                    throw new ArgumentException("No such data adapter", nameof(dataAdapter));
            }

            return adapter;
        }

        public Connection getConnection(DataConnections dataConnection)
        {
            Connection connection;

            // TODO: Change returned connection to appropriate ones
            switch (dataConnection)
            {
                case DataConnections.BarentswatchAIS:
                    connection = new BarentswatchAISConnection();
                    break;
                case DataConnections.PhoneGPS:
                    connection = new PhoneGPSConnection();
                    break;
                case DataConnections.MockPhoneGPS:
                    connection = new MockNMEAConnection();
                    break;
                case DataConnections.HardcodedGPS:
                    connection = new HardcodedGPSConnection();
                    break;
                case DataConnections.VesselGPS:
                    connection = new HardcodedGPSConnection();
                    break;
                default:
                    throw new ArgumentException("No such data connection", nameof(dataConnection));
            }

            return connection;
        }

        public ParameterExtractor getParameterExtractor(ParameterExtractors parameterExtractor)
        {
            // The base class doesn't have any functionality
            ParameterExtractor extractor;

            switch (parameterExtractor)
            {
                case ParameterExtractors.BarentswatchAIS:
                    extractor = new BarentswatchAISParameterExtractor(aligner);
                    break;
                case ParameterExtractors.None:
                    extractor = new ParameterExtractor();
                    break;
                default:
                    extractor = new ParameterExtractor();
                    break;
            }

            return extractor;
        }

        //// This one might be unneccessary
        //public DTO getDTO(DataSources dataSource)
        //{
        //    // TODO: Assign proper DTO's
        //    DTO dto = new GPSInfoDTO();

        //    switch (dataSource)
        //    {
        //        case DataSources.GPSInfo:
        //            dto = new GPSInfoDTO();
        //            break;
        //        case DataSources.Postgres:
        //            break;
        //        case DataSources.AIS:
        //            dto = new AISDTO();
        //            break;
        //        case DataSources.Mock:
        //            break;
        //        default:
        //            break;
        //    }

        //    return dto;
        //}
    }
}
