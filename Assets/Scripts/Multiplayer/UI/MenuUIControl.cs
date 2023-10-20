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

    public GameObject logMenu;

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

    //--vessel mode
    public GameObject vesselSettingsMenu;
    public GameObject vesselModeSettings;

    public TextMesh vesselModeBtnLabel;
    public TextMesh vesselNameBtnLabel;
    public TextMesh bridgeHeighConfigLabel;
    public TextMesh nightModeBtnLabel;

    public TextMesh apiReachLabel;


    string _tempVesselName;
    double _tempBridgeHeight;
    double _tempAPIReach;

    public TouchScreenKeyboard keyboard;
    public void ChangeVesselName()
    {
        ShowKeyboard(0);
        StartCoroutine(MonitorKeyboardInput(0));
    }
    public void ChangeBridgHeight()
    {
        ShowKeyboard(1);
        StartCoroutine(MonitorKeyboardInput(1));
    }
    public void ChangeAPIReach()
    {
        ShowKeyboard(1);
        StartCoroutine(MonitorKeyboardInput(2));
    }
    void ShowKeyboard(int _keyboardTypeMode)
    {
        if (_keyboardTypeMode == 0)
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        if (_keyboardTypeMode == 1)
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumbersAndPunctuation, false, false, false, false);
    }


    IEnumerator MonitorKeyboardInput(int inputMode)
    {
        // Wait while the keyboard is active
        while (keyboard != null && keyboard.status != TouchScreenKeyboard.Status.Done)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Visible)
            {
                if (inputMode == 0)
                    vesselNameBtnLabel.text = keyboard.text;
                if (inputMode == 1)
                    bridgeHeighConfigLabel.text = keyboard.text + " [bridge height]";
                if (inputMode == 2)
                    apiReachLabel.text = keyboard.text + " [API reach]";
            }
            yield return null; // Wait for the next frame and then continue the loop
        }

        // Optional: If you want to capture the final text after the keyboard is done
        if (keyboard != null)
        {
            if (inputMode == 0)
            {
                _tempVesselName = keyboard.text;
                StoreVesselName();
            }
            if (inputMode == 1)
            {
                if (double.TryParse(keyboard.text, out _tempBridgeHeight))
                {
                    Config.Instance.conf.VesselSettingsD["BridgeHeight"] = _tempBridgeHeight;
                    StoreBridgeHeight();
                }
            }
            if (inputMode == 2)
            {
                if (double.TryParse(keyboard.text, out _tempAPIReach))
                {
                    Config.Instance.Set_API_range(_tempAPIReach);
                    StoreAPIReach();
                }
            }
        }
    }
    void StoreVesselName()
    {
        Config.Instance.conf.VesselSettingsS["VesselName"] = _tempVesselName;
        PlayerPrefs.SetString("VesselName", _tempVesselName);
        PlayerPrefs.Save();
        vesselNameBtnLabel.text = keyboard.text;
        Debug.Log("set vessel name to: " + vesselNameBtnLabel.text);
    }
    void StoreBridgeHeight()
    {
        Config.Instance.conf.VesselSettingsD["BridgeHeight"] = _tempBridgeHeight;
        PlayerPrefs.SetString("BridgeHeight", _tempBridgeHeight.ToString());
        PlayerPrefs.Save();
        bridgeHeighConfigLabel.text = keyboard.text + " [bridge height]";
        Debug.Log("set bridge height to: " + bridgeHeighConfigLabel.text);
    }
    void StoreAPIReach()
    {
        Config.Instance.Set_API_range(_tempAPIReach);
        PlayerPrefs.SetString("APIReach", _tempAPIReach.ToString());
        PlayerPrefs.Save();
        apiReachLabel.text = keyboard.text + " [API reach]";
        Debug.Log("set api reach to: " + apiReachLabel.text);
    }
    public void SetVesselMode()
    {
        bool _state = !Config.Instance.conf.VesselMode;

        Config.Instance.conf.VesselMode = _state;
        if (!_state)
        {
            PlayerPrefs.SetInt("IsVesselMode", 0);
            vesselModeBtnLabel.text = "Not on a Vessel";

        }
        else if (_state)
        {
            PlayerPrefs.SetInt("IsVesselMode", 1);
            vesselModeBtnLabel.text = "On Vessel: ";

        }

        Debug.Log("set vessel mode to: " + _state);
        PlayerPrefs.Save();
    }
    public void SetNightMode()
    {
        if (Player.Instance.nightMode == 2)
            Player.Instance.nightMode = 0;
        else
            Player.Instance.nightMode++;


        if (Player.Instance.nightMode == 0)
        {
            PlayerPrefs.SetInt("IsNightMode", 0);
            nightModeBtnLabel.text = "NightMode Off";

        }
        else if (Player.Instance.nightMode == 1)
        {
            PlayerPrefs.SetInt("IsNightMode", 1);
            nightModeBtnLabel.text = "NightMode Half ON";
        }
        else if (Player.Instance.nightMode == 2)
        {
            PlayerPrefs.SetInt("IsNightMode", 2);
            nightModeBtnLabel.text = "NightMode ON";
        }
        Player.Instance.SetLightIntensity();

        Debug.Log("set night mode to: " + Player.Instance.nightMode);
        PlayerPrefs.Save();
    }




    private void Start()
    {

        ConnectionController.Instance.playerConfig = ConnectionController.Instance.LoadPlayerConfig();
        if (ConnectionController.Instance.playerConfig != null)
        {
            loadedIPConfigBtnLabel.text = ConnectionController.Instance.playerConfig.partnerDeviceIP;
            loadedGPSIPConfigBtnLabel.text = Config.Instance.conf.PhoneGPS["IP"];
            InitVesselSettingsUI();
            if (ConnectionController.Instance.playerConfig.playerSelection != PlayerConfig.PlayerSelection.Undefined)
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

    void InitVesselSettingsUI()
    {
        bool _isVesselMode = Config.PlayerPrefsInt_toBool(PlayerPrefs.GetInt("IsVesselMode", 0));
        if (!_isVesselMode)
            vesselModeBtnLabel.text = "Not on a Vessel";
        if (_isVesselMode)
            vesselModeBtnLabel.text = "On Vessel: ";

        Config.Instance.conf.VesselMode = _isVesselMode;

        if (double.TryParse(PlayerPrefs.GetString("BridgeHeight", "0"), out _tempBridgeHeight))
        {
            Config.Instance.conf.VesselSettingsD["BridgeHeight"] = _tempBridgeHeight;
        }
        bridgeHeighConfigLabel.text = Config.Instance.conf.VesselSettingsD["BridgeHeight"].ToString() + " [bridge height]";

        _tempVesselName = PlayerPrefs.GetString("VesselName", "");
        Config.Instance.conf.VesselSettingsS["VesselName"] = _tempVesselName;
        vesselNameBtnLabel.text = Config.Instance.conf.VesselSettingsS["VesselName"];


        //nightmode
        Player.Instance.nightMode = PlayerPrefs.GetInt("IsNightMode", 0);
        if (Player.Instance.nightMode == 0)
            nightModeBtnLabel.text = "NightMode Off";
        else if (Player.Instance.nightMode == 1)
            nightModeBtnLabel.text = "NightMode half ON";
        else if (Player.Instance.nightMode == 2)
            nightModeBtnLabel.text = "NightMode ON";
        Player.Instance.SetLightIntensity();

        //API reach
        if (double.TryParse(PlayerPrefs.GetString("APIReach", "0"), out _tempAPIReach))
        {
            Config.Instance.Set_API_range(_tempAPIReach);
        }
        apiReachLabel.text = Config.Instance.Get_API_range() + " [API reach]";

    }

    void HideMenus()
    {
        mainMenu.SetActive(false);
        generalMenu.SetActive(false);
        multiPlayerMenu.SetActive(false);
        debugMenu.SetActive(false);
        logMenu.SetActive(false);
        gpsSettings.SetActive(false);
        connectionSettings.SetActive(false);

        vesselSettingsMenu.SetActive(false);

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
    public void DisplayVesselSettingsMenu(bool show)
    {
        if (show)
        {
            HideMenus();
            DisplayVesselModeSettings(false);
        }

        vesselSettingsMenu.SetActive(show);
    }
    public void DisplayVesselModeSettings(bool show)
    {
        if (show)
        {
            vesselSettingsMenu.SetActive(false);
        }
        vesselModeSettings.SetActive(show);
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

    public void DisplayLogMenu(bool show)
    {
        if (show)
            HideMenus();

        logMenu.SetActive(show);
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
