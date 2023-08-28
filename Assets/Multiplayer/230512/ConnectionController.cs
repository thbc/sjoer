using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    public OSC osc;

    public OSCReceiver oscReceiver;
    public OSCSender oscSender;
    public TextMesh statusLabel;
    public TextMesh statusLabel_2;

    public CursorMultiplayer cursorMP;

    public static ConnectionController Instance { get; private set; }

    private void Awake()
    {
        // 2. Logic in Awake() to handle multiple instances
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }

  
    public static bool IsValidIP(string ipString)
    {
        if (String.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }

    public static string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
    [System.Serializable]
    public class PlayerConfig
    {
        public string gpsIP;
        public int gpsPort = 6000;
        public string partnerDeviceIP;
        public int inPort;
        public int outPort;

        private int port_1 = 6969;
        private int port_2 = 7070;

        private PlayerSelection _playerSelection;
        public PlayerSelection playerSelection
        {
            get
            {
                return _playerSelection;
            }
            set { 
                _playerSelection = value;
                
                switch (value)
                {
                    case PlayerSelection.Player_1:
                        inPort = port_1;
                        outPort = port_2;
                        break;
                    case PlayerSelection.Player_2:
                        inPort = port_2;
                        outPort = port_1;
                        break;
                    case PlayerSelection.TestMode:
                        partnerDeviceIP = "127.0.0.1";
                        inPort = port_1;
                        outPort = port_1;
                        break;
                }
            }
        }
        public enum PlayerSelection
        {
            Player_1, Player_2, TestMode
        }

       
  

    }
    public PlayerConfig playerConfig = new PlayerConfig();


    public PlayerConfig LoadPlayerConfig()
    {
        PlayerConfig loadedPlayerConfig = new PlayerConfig();
        loadedPlayerConfig.partnerDeviceIP = PlayerPrefs.GetString("PartnerDeviceIP", "");
        loadedPlayerConfig.gpsIP = PlayerPrefs.GetString("GPSIP", "");

        //string _storedPlayerSel = PlayerPrefs.GetString("PlayerSelection", "");
        //if (_storedPlayerSel == "Player_1")
        //    loadedPlayerConfig.playerSelection = PlayerSelection.Player_1;
        //else if (_storedPlayerSel == "Player_2")
        //    loadedPlayerConfig.playerSelection = PlayerSelection.Player_2;

        if (!string.IsNullOrWhiteSpace(loadedPlayerConfig.partnerDeviceIP))// && !string.IsNullOrWhiteSpace(_storedPlayerSel))
            return loadedPlayerConfig;
        else
            return null;
    }


    public void StorePlayerConfig(PlayerConfig _playerConfig2Store)
    {
        //if (_playerConfig2Store.playerSelection != PlayerSelection.TestMode)
        //{
        //    if (_playerConfig2Store.playerSelection == PlayerSelection.Player_1)
        //        PlayerPrefs.SetString("PlayerSelection", "Player_1");
        //    if (_playerConfig2Store.playerSelection == PlayerSelection.Player_2)
        //        PlayerPrefs.SetString("PlayerSelection", "Player_2");

        if (_playerConfig2Store.partnerDeviceIP != "127.0.0.1")
        {
            PlayerPrefs.SetString("PartnerDeviceIP", _playerConfig2Store.partnerDeviceIP);
              PlayerPrefs.Save();
  }
        else
        { Debug.LogWarning("Did not store Partner device ip configuration, since partnerIP was localhost..."); }
        

        // }
    }


    private void Start()
    {
        cursorMP.enabled = false;
        //DisplayConnectionSettings(false);

        playerConfig.gpsIP = PlayerPrefs.GetString("GPSIP", "");
        Debug.LogWarning(playerConfig.gpsIP);




    }

    public void SelectPlayer(int _playerIndex)
    {
        if (_playerIndex == 1)
            playerConfig.playerSelection = PlayerConfig.PlayerSelection.Player_1;
        else if (_playerIndex == 2)
            playerConfig.playerSelection = PlayerConfig.PlayerSelection.Player_2;
        else if (_playerIndex == 0)
            playerConfig.playerSelection = PlayerConfig.PlayerSelection.TestMode;

        statusLabel.text = playerConfig.playerSelection.ToString();

       
        if (playerConfig.playerSelection == PlayerConfig.PlayerSelection.TestMode)
        {
            TryConnect();
        }
    }

  
    public void TryConnect()
    {

        if (osc.isInitialized)
            Reconnect();

        if (playerConfig != null)
        {
            osc.SetupOSC(playerConfig.partnerDeviceIP, playerConfig.outPort, playerConfig.inPort);// osc.enabled = true;
            statusLabel.text = "Trying to connect " + playerConfig.playerSelection.ToString() + " with " +
            "\n" + "Partner Device: " + playerConfig.partnerDeviceIP
           + "\n" + "OutPort: " + playerConfig.outPort.ToString() + "|InPort: " + playerConfig.inPort.ToString();


            oscReceiver.SetupListener();
            if(ConnectingCoroutine != null)
                StopCoroutine(ConnectingCoroutine);
            ConnectingCoroutine = StartCoroutine(WaitForConnection());

            //playerConfig.StorePlayerConfig();

        }
        else
        {
            statusLabel.text = "Failed to connect to" + playerConfig.partnerDeviceIP+ "\n"
                + "Please change the connection settings.";
        }
    }
    public Coroutine ConnectingCoroutine;
    public IEnumerator WaitForConnection()
    {
        while (!oscReceiver.receivedPong)
        {
            // Send a ping.
            oscSender.SendPing();
            statusLabel.text = "Trying to connect " + playerConfig.playerSelection.ToString() + " with " +
                "\n" + "Partner Device: " + playerConfig.partnerDeviceIP
                + "\n" + "OutPort: " + playerConfig.outPort.ToString() + "|InPort: " + playerConfig.inPort.ToString()
                + "\n" + "Ping: " + DateTime.Now.ToString();

            // Wait for another 5 seconds before the next ping.
            yield return new WaitForSeconds(5f);
        }

        // .... if receivedPong = true this will be called once:
        statusLabel.text = "Received pong "+ DateTime.Now.ToString()+"\n"+ playerConfig.playerSelection.ToString() + " with " +
                "\n" + "Partner Device: " + playerConfig.partnerDeviceIP
                + "\n" + "OutPort: " + playerConfig.outPort.ToString() + "|InPort: " + playerConfig.inPort.ToString() ;
        // OnSuccessfullyConnected();

    }




    void Reconnect()
    {
        if (ConnectingCoroutine != null)
        {
            StopCoroutine(ConnectingCoroutine);
            ConnectingCoroutine = null;
        }

        cursorMP.enabled = false;


        osc.isInitialized = false;
        oscReceiver.receivedPong = false;
        oscSender.sentPong = false;
        oscReceiver.receivedPing = false;
        oscSender.sentPing = false;


        osc.Close();
        osc.enabled = false;
        osc.enabled = true;
        isConnected = false;

        oscReceiver.CoPlayerCursorHighlight.SetActive(false);
    }
    bool isConnected = false;
    public void OnSuccessfullyConnected()
    {
        cursorMP.enabled = true;
        statusLabel.text = "Successfully connected " + playerConfig.playerSelection.ToString() + " to " + playerConfig.partnerDeviceIP;
        isConnected = true;
        
        StorePlayerConfig(playerConfig);

    }



}
