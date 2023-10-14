using System.Collections;
using System.Collections.Generic;
using Assets.Positional;
using UnityEngine;
using Assets.Resources;
using Multiplayer.Marking;
using static ConnectionController;

public class MenuUIControl : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject generalMenu;
    public GameObject gpsSettings;
    public GameObject gpsLoadedSettings;
    public GameObject gpsManualSettings;

    public GameObject multiPlayerMenu;
    public GameObject connectionSettings;

    public GameObject quickConnectMenu;
    public GameObject playerSelectionMenu;
    public GameObject ipSettingsMenu;
    public GameObject debugMenu;


    public TextMesh markModeBtnLabel;
    public GameObject eyeTrackingModeBtn;





    public TextMesh ipLabel;

    //public ConnectionController connectionController; --> is Singleton

    public GameObject manualIPConfig;
    public GameObject loadedIPConfig;
    public TextMesh loadedIPConfigBtnLabel;

    public TextMesh loadedGPSIPConfigBtnLabel;

    public TextMesh quickConnectBtnLabel;

    bool hasLoadedConfig;

    private void Start()
    {

        ConnectionController.Instance.playerConfig = ConnectionController.Instance.LoadPlayerConfig();
        if (ConnectionController.Instance.playerConfig != null)
        {
            loadedIPConfigBtnLabel.text = ConnectionController.Instance.playerConfig.partnerDeviceIP;
            loadedGPSIPConfigBtnLabel.text = Config.Instance.conf.PhoneGPS["IP"];

            if(ConnectionController.Instance.playerConfig.playerSelection != PlayerConfig.PlayerSelection.Undefined)
            {
                DisplayConnectionSettings(true);
                DisplayQuickConnectionMenu(true);
            }
            else 
            {
                // this is executed when the config is not null, but the playerselection is still undefined
                DisplayConnectionSettings(true);
                DisplayPlayerSelectionMenu(true);
            }

            InitMarkModeUI();

            hasLoadedConfig = true;
        }
        else
        {
            hasLoadedConfig = false;
            ConnectionController.Instance.statusLabel.text = "Could not load configuration. Please set up.";
            ConnectionController.Instance.playerConfig = new PlayerConfig();
            DisplayConnectionSettings(true);
            DisplayPlayerSelectionMenu(true);

            // DisplayLoadedConfigurationMenu(false);
            // playerConfigUI.SetActive(true);
            // setupConnectionBtn.SetActive(false);

        }
    }

    void HideMenus()
    {
        mainMenu.SetActive(false);
        generalMenu.SetActive(false);
        multiPlayerMenu.SetActive(false);
        debugMenu.SetActive(false);
        gpsSettings.SetActive(false);
        connectionSettings.SetActive(false);
    }
    // all sub menu inside connectionSettings
     void HideConnectionSettingsMenus()
    {
        quickConnectMenu.SetActive(false);
        playerSelectionMenu.SetActive(false);
        ipSettingsMenu.SetActive(false);

    }
    public void DisplayMainMenu(bool show)
    {
        if (show)
            HideMenus();

        mainMenu.SetActive(show);
    }
    #region GenrealMenu
    public void DisplayGeneralMenu(bool show)
    {
        if (show)
            HideMenus();

        generalMenu.SetActive(show);
    }
    public void DisplayGPSSettings(bool show)
    {
        if (show)
        {
            HideMenus();

            //logic for showing loaded GPS if not null, otherwise manual settings
            if (!string.IsNullOrEmpty(Config.Instance.conf.PhoneGPS["IP"]))
                DisplayGPSLoadedSettings(true);
            else
                DisplayGPSManualSettings(true);
        }

        gpsSettings.SetActive(show);
    }
    public void DisplayGPSLoadedSettings(bool show)
    {
        if (show)
            DisplayGPSManualSettings(false);

        gpsLoadedSettings.SetActive(show);
    }
    public void DisplayGPSManualSettings(bool show)
    {
        if (show)
            DisplayGPSLoadedSettings(false);

        gpsManualSettings.SetActive(show);
    }
    #endregion
    #region Multiplayer Menu
    public void DisplayMultiPlayerMenu(bool show)
    {
        if (show)
            HideMenus();

        multiPlayerMenu.SetActive(show);
    }
    public void DisplayConnectionSettings(bool show)
    {
        if (show)
        {
            HideMenus();
            if (!string.IsNullOrEmpty(Instance.playerConfig.partnerDeviceIP) && !string.IsNullOrEmpty(Instance.playerConfig.playerSelection.ToString()))
            {
                DisplayQuickConnectionMenu(true);
            }
            else
                DisplayPlayerSelectionMenu(true);
        }

        connectionSettings.SetActive(show);
    }
    public void WhatsMyIP(TextMesh whatsmyipBtnLabel)
    {
        //ipLabel.text = "Your IP is: " + LocalIPAddress() + "\n" + "Please enter this IP on the partner device.";
        whatsmyipBtnLabel.text = "Your IP:" + "\n" + LocalIPAddress();

    }


    public void DisplayQuickConnectionMenu(bool show)
    {

        if (show)
        {
            HideConnectionSettingsMenus();
            quickConnectBtnLabel.text = "Connect" + "\n"
                + "(as Player " + ConnectionController.Instance.playerConfig.playerSelection.ToString() + "\n"
                + " to: " + ConnectionController.Instance.playerConfig.partnerDeviceIP + ")";
        }

        quickConnectMenu.SetActive(show);
    }
    // UI for selecting Player1 or Player2
    public void DisplayPlayerSelectionMenu(bool show)
    {
        if (show)
        {
            HideConnectionSettingsMenus();
        }
        playerSelectionMenu.SetActive(show);
    }

    // UI for selecting IP to connect to (other device's IP)
    public void DisplayIpSettingsMenu(bool show)
    {
        if (show)
        {
            HideConnectionSettingsMenus();
            if (hasLoadedConfig)
                DisplayIPLoadedSettings(true);
            else
                DisplayIPManualSettings(true);
        }

        ipSettingsMenu.SetActive(show);

    }
    // UI for connecting to same IP as previously
    public void DisplayIPLoadedSettings(bool show)
    {
        if (show)
        {
            manualIPConfig.SetActive(false);
            loadedIPConfig.SetActive(true);
        }
        else if (!show)
        {
            manualIPConfig.SetActive(true);
            loadedIPConfig.SetActive(false);
        }
    }
    public void DisplayIPManualSettings(bool show)
    {
        if (show)
        {
            manualIPConfig.SetActive(true);
            loadedIPConfig.SetActive(false);
        }
        else if (!show)
        {
            manualIPConfig.SetActive(false);
            loadedIPConfig.SetActive(true);
        }
    }

    #endregion
    public void DisplayDebugMenu(bool show)
    {
        if (show)
            HideMenus();

        debugMenu.SetActive(show);
    }


    string _tempIP;
    public void EnterIPCharacter(string val)
    {
        if (!string.IsNullOrWhiteSpace(_tempIP))
        {
            _tempIP = _tempIP + val;
            ipLabel.text = _tempIP;
        }
        else
            _tempIP = val;
    }
    public void RemoveIPCharacter()
    {
        _tempIP = _tempIP.Remove(_tempIP.Length - 1);
        ipLabel.text = _tempIP;

    }
    public void ResetIPCharacters()
    {
        _tempIP = null;
        ipLabel.text = "";
    }
    // quick connect method using same configuration as previously
    public void QuickConnect()
    {
        ConnectionController.Instance.TryConnect();
    }
    // connect to the loaded IP from previouslym which has populated the textMesh "loadedIPConfigBtnLabel"
    public void ConnectLoadedConfiguration()
    {
        _tempIP = loadedIPConfigBtnLabel.text;
        Connect();
    }
    // classic Connect method which uses the tempIP string, which has been populated from user button input
    public void Connect()
    {
        if (!string.IsNullOrWhiteSpace(_tempIP) && IsValidIP(_tempIP))
        {
            ConnectionController.Instance.playerConfig.partnerDeviceIP = _tempIP;
            ConnectionController.Instance.TryConnect();
        }
        else Debug.LogWarning("couldnt connect since _tempIP was empty.");
    }
    //GPS IP settings
    string _tempGPSIP;
    public void EnterGPSIPCharacter(string val)
    {
        if (!string.IsNullOrWhiteSpace(_tempGPSIP))
        {
            _tempGPSIP = _tempGPSIP + val;
            ipLabel.text = _tempGPSIP;
        }
        else
            _tempGPSIP = val;
    }
    public void RemoveGPSIPCharacter()
    {
        _tempGPSIP = _tempGPSIP.Remove(_tempGPSIP.Length - 1);
        ipLabel.text = _tempGPSIP;

    }
    public void ResetGPSIPCharacters()
    {
        _tempGPSIP = null;
        ipLabel.text = "";
    }
    public void ConnectLoadedGPSConfiguration()
    {
        //  _tempGPSIP = PlayerPrefs.GetString("GPSIP", "");
        //  ConnectionController.Instance.playerConfig.gpsIP = _tempGPSIP;
        _tempGPSIP = Config.Instance.conf.PhoneGPS["IP"]; //ConnectionController.Instance.playerConfig.gpsIP;
        loadedGPSIPConfigBtnLabel.text = _tempGPSIP;
    }
    public void ConnectGPS()
    {
        if (!string.IsNullOrWhiteSpace(_tempGPSIP) && IsValidIP(_tempGPSIP))
        {
            Config.Instance.conf.PhoneGPS["IP"] = _tempGPSIP;
            PlayerPrefs.SetString("GPSIP", _tempGPSIP);
            PlayerPrefs.Save();
            

            Player.Instance.InitializeGPSRetriever();

            //ToggleConnectionSettingsMenu();
            //connectionController.TryConnect();

            //call here the GPS initialization

        }
    }
    //end GPS
    void InitMarkModeUI()
    {
       bool nextstate = MarkerMode.Instance.allowMarking;
        if (nextstate == true)
        {
            markModeBtnLabel.text = "Deactivate Marking";
        }
        else if (nextstate == false)
        {
            markModeBtnLabel.text = "Activate Marking";
        } 
    }
    public void SetMarkMode()
    {
        bool nextstate = MarkerMode.Instance.SetMarkMode();
        if (nextstate == true)
        {
            markModeBtnLabel.text = "Deactivate Marking";
        }
        else if (nextstate == false)
        {
            markModeBtnLabel.text = "Activate Marking";
        }
    }
    public void SetEyeTrackingMode(TextMesh buttonLabel)
    {
        bool nextstate = EyeTrackingMode.Instance.SetEyeTrackingMode();
        if (nextstate == true)
        {
            buttonLabel.text = "Deactivate EyeTracking";
        }
        else if (nextstate == false)
        {
            buttonLabel.text = "Activate EyeTracking";
        }
    }


}
