using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.Multiplayer.Tools;
using Unity.Netcode.Transports.UTP;
using System.Net;
using UnityEngine.UI;
namespace MultiPlayer.Core.v3
{
    public class ConnectionManagerUI : MonoBehaviour
    {
        [SerializeField] GameObject ipPanelUI;
        [SerializeField] GameObject startHostBtn;
        [SerializeField] GameObject startClientBtn;
        [SerializeField] TextMesh ipValue;
        [SerializeField] TextMesh debugTextMesh;

        public Text debugText;

        private IEnumerator coroutine;

        public bool testMode;

        public string defaultIP = "192.168.43.173";
        void Start()
        {
           /*  GetDeviceName();
            if (testMode)
            {
                defaultIP = "127.0.0.1";
            } */
            //  else if(!testMode)
            // {
            //Connect2Host(defaultIP);

            /*   print("Starting " + Time.time + " seconds");


              coroutine = WaitAndPrint(60.0f);
              StartCoroutine(coroutine);

              print("Coroutine started"); */

            //  }
        #if !UNITY_EDITOR
        NetworkManager.Singleton.StartClient();
        #endif
        #if UNITY_EDITOR
        NetworkManager.Singleton.StartHost();
        #endif
        }

        private IEnumerator WaitAndPrint(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            print("Coroutine ended: " + Time.time + " seconds");
            if (!NetworkManager.Singleton.IsConnectedClient)
                NetworkManager.Singleton.StartHost();

        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            startHostBtn.SetActive(false);
            startClientBtn.SetActive(false);

            debugText.text = debugText.text + "\n" + "is host";

        }
        public void StartClient()
        {
            startHostBtn.SetActive(false);
                startClientBtn.SetActive(false);
                Connect2Host();

            /* if (!testMode)
            {
                startHostBtn.SetActive(false);
                startClientBtn.SetActive(false);
                ipPanelUI.SetActive(true);
            }
            else if (testMode)
            {
                startHostBtn.SetActive(false);
                startClientBtn.SetActive(false);
                Connect2Host(defaultIP);

            } */
        }
        // private string _HostIP;
        public void AddChar2HostIP(string _input)
        {
            if (!string.IsNullOrEmpty(ipValue.text))
            {
                ipValue.text = ipValue.text + _input;
            }
            else if (string.IsNullOrEmpty(ipValue.text))
            {
                ipValue.text = _input;
            }
        }
        public void ResetHostIP()
        {
            ipValue.text = "";
        }
        public void Connect2Host()
        {
             NetworkManager.Singleton.StartClient();
            /* if (!string.IsNullOrEmpty(_customIP))
            {
                startHostBtn.SetActive(false);
                startClientBtn.SetActive(false);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                        _customIP,  // The IP address is a string
                        (ushort)7777 //12345 // The port number is an unsigned short
                    );
                NetworkManager.Singleton.StartClient();

                debugText.text = debugText.text + "\n" + "is client; connected to: " + _customIP;

            }
            else
            {
                if (!string.IsNullOrEmpty(ipValue.text))
                {
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                        ipValue.text,  // The IP address is a string
                        (ushort)7777 //12345 // The port number is an unsigned short
                    );
                    NetworkManager.Singleton.StartClient();
                }
            } */
            if (ipPanelUI != null)
                ipPanelUI.SetActive(false);
        }

        void GetDeviceName()
        {
            // Get the device name
            string deviceName = System.Environment.MachineName;

            // Print the device name to the console
            // Debug.Log("Device name is " + deviceName);
            debugText.text = debugText.text + "\n" + deviceName;
            // Resolve the device name to an IP address
            try
            {
                IPHostEntry host = Dns.GetHostEntry(deviceName);
                IPAddress ipAddress = host.AddressList[0];
                Debug.Log("IP address of " + deviceName + " is " + ipAddress.ToString());
                debugText.text = debugText.text + "\n" + "IP address of " + deviceName + " is " + ipAddress.ToString();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        /* private void Update()
    {
        // Get all connected clients
        List<NetworkClient> clients = NetworkManager.Singleton.ConnectedClientsList;

        // Loop through all clients and do something with them
        foreach (NetworkClient client in clients)
        {
            // Do something with the client, e.g. get its network ID
            Debug.Log("Client network ID: " + client.Id);
        }
    } */
    }

}