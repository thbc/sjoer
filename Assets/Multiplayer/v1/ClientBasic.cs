using UnityEngine;
using Unity.Networking.Transport;

namespace MultiPlayer.Core.v1
{

    public class ClientBasic : MonoBehaviour
    {
        private NetworkDriver m_Driver;
        private NetworkConnection m_Connection;
        private bool m_Done;

        public GameObject receivedGazeObj;

        public GameObject ipInputField;

        public void OnEnable()
        {
            #if UNITY_EDITOR
             m_Driver = NetworkDriver.Create();
            m_Connection = default(NetworkConnection);
            var endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = 9000;
            m_Connection = m_Driver.Connect(endpoint); 
            #endif

            #if !UNITY_EDITOR
            ipInputField.SetActive(true);
            #endif
        }

        public void SetIP(string _ip = "")
        {
            m_Driver = NetworkDriver.Create();
            m_Connection = default(NetworkConnection);
            string ipAddress = _ip;
            if(!string.IsNullOrEmpty(ipAddress))
            {     
                ushort port = 9000;
                if (NetworkEndPoint.TryParse(ipAddress, port, out NetworkEndPoint endPoint))
                {
                    m_Connection = m_Driver.Connect(endPoint);
                }
            }
            else 
            {
                var endpoint = NetworkEndPoint.LoopbackIpv4;
                endpoint.Port = 9000;
                m_Connection = m_Driver.Connect(endpoint);
            }
        }

        public void OnDestroy()
        {
            m_Driver.Dispose();
        }
        public void Dispose()
        {
            m_Driver.Dispose();
        }
        public void Update()
        {
            m_Driver.ScheduleUpdate().Complete();

            if (!m_Connection.IsCreated)
            {
                if (!m_Done)
                    Debug.Log("Something went wrong during connect");
                return;
            }

            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("We are now connected to the server");

                    uint value = 1;
                    m_Driver.BeginSend(m_Connection, out var writer);
                    writer.WriteUInt(value);
                    m_Driver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    /* uint value = stream.ReadUInt();
                    Debug.Log("Got the value = " + value + " back from the server");
                    m_Done = true;
                    m_Connection.Disconnect(m_Driver);
                    m_Connection = default(NetworkConnection); */

                    //  var value = stream.ReadFloat();
                    float x = stream.ReadFloat();
                    float y = stream.ReadFloat();
                    float z = stream.ReadFloat();
                    Vector3 gazeDirection = new Vector3(x, y, z);
                    //Quaternion lookRotation = Quaternion.LookRotation(gazeDirection);
                    receivedGazeObj.transform.position = gazeDirection;

                    //Debug.Log("Got the rotation y = " + value + " back from the server");

                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                    m_Connection = default(NetworkConnection);
                }
            }
        }
    }



}