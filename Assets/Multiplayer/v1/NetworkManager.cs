using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
namespace MultiPlayer.Core.v1
{
    /* public class ConnectionType { }
    public class ServerInstance:ConnectionType{ }
    public class ClientInstance:ConnectionType{ } */

   public class NetworkManager : MonoBehaviour
{
    [SerializeField] ConnectionType _connectionType;
    public ServerBasic serverInstance;
   public ClientBasic clientInstance;
 

    public void InitServer()
    {
        _connectionType = ConnectionType.ServerInstance;

        
            serverInstance.enabled = true;// = new ServerBasic();
     
        Debug.Log("initialising server");


    }

    public void InitClient()
    {
        _connectionType = ConnectionType.ClientInstance;

        
            clientInstance.enabled =true;// = new ClientBasic();
       
    }
    void OnApplicationQuit()
    {
        if(clientInstance.enabled)
        clientInstance.Dispose();
        if(serverInstance.enabled)
        serverInstance.Dispose();
    }

    private void ShowDeviceIP()
    {
        // Implementation for ShowDeviceIP
    }

    private void Check4Server()
    {
        // Implementation for Check4Server
    }

    public enum ConnectionType
    {
        Undefined,
        ServerInstance,
        ClientInstance
    }


        public void Connect2Server(){}
        public void HostAsServer(){}

    }
    

   

   
}