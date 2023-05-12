using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using Microsoft.MixedReality.Toolkit.Input;

namespace MultiPlayer.Core.v1
{

public class ServerBasic:MonoBehaviour
{
    public NetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;
    private GazeProvider gazeProvider;
    public void OnEnable()
    {
        gazeProvider = FindObjectOfType<GazeProvider>();

        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4; // The local address to which the client will connect to is 127.0.0.1
        endpoint.Port = 9000;
        if (m_Driver.Bind(endpoint) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);


    }

    public void Dispose()
    {
        if (m_Driver.IsCreated)
        {
            m_Driver.Dispose();
        }
        if (m_Connections.IsCreated)
        {
            m_Connections.Dispose();
        }
    }

    public void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        // CleanUpConnections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }
        // AcceptNewConnections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                 

                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadUInt();

                    Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;



        // Do something with gazePosition and gazeDirection

                   /*  m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    writer.WriteUInt(number);
                    m_Driver.EndSend(writer); */

                 /*    m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    writer.WriteFloat(gazeDirection.y);
                    m_Driver.EndSend(writer); */

                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
            Vector3 gazePosition = gazeProvider.GazePointer.Position;
        //Vector3 gazeDirection = gazeProvider.GazeDirection;
        // Do something with gazePosition and gazeDirection

                   /*  m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    writer.WriteUInt(number);
                    m_Driver.EndSend(writer); */

                    m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                    writer.WriteFloat(gazePosition.x);
                    writer.WriteFloat(gazePosition.y);
                    writer.WriteFloat(gazePosition.z);
                    m_Driver.EndSend(writer);
        }
    }
}

}