using Assets.Positional;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.DataManagement
{
    public class DataRetriever
    {
        private DataAdapter dataAdapter;
        private Connection dataConnection;
        private ParameterExtractor parameterExtractor;

        public DataRetriever(DataConnections connection, DataAdapters adapter, ParameterExtractors extractor, Player aligner)
        {
            // Assign aligner to the DataFactory if DataFactory.Instance.aligner is null
            DataFactory.Instance.aligner ??= aligner;
            dataConnection = DataFactory.Instance.getConnection(connection);
            dataAdapter = DataFactory.Instance.getDataAdapter(adapter);
            parameterExtractor = DataFactory.Instance.getParameterExtractor(extractor);
        }

        public async Task<DTO> fetch()
        {
            //Debug.Log(await connection.get(parameterExtractor.get()));
            return dataAdapter.convert(
                await dataConnection.get(parameterExtractor.get())
                );
        }

        public bool isConnected()
        {
            return dataConnection.connected;
        }

        public void OnDestroy()
        {
            dataConnection.OnDestroy();
        }
    }
}