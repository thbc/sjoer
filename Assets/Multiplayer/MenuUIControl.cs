using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConnectionController;

public class MenuUIControl : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject markModeBtn;
    public GameObject eyeTrackingModeBtn;


    public GameObject connectionSettingsMenu;

    public GameObject playerSelectionMenu;

    public GameObject ipSettingsMenu;
    public TextMesh ipLabel;

    public ConnectionController connectionController;

    public GameObject manualIPConfig;
    public GameObject loadedIPConfig;
    public TextMesh loadedIPConfigBtnLabel;

    public TextMesh loadedGPSIPConfigBtnLabel;

    bool hasLoadedConfig;

    private void Start()
    {
        

        connectionController.playerConfig = connectionController.LoadPlayerConfig();
        if(connectionController.playerConfig!= null)
        {
            loadedIPConfigBtnLabel.text = connectionController.playerConfig.partnerDeviceIP;
            loadedGPSIPConfigBtnLabel.text = connectionController.playerConfig.gpsIP;

            hasLoadedConfig = true;
        }
        else
        {
            hasLoadedConfig = false;
            connectionController.statusLabel.text = "Could not load configuration. Please set up.";
            connectionController.playerConfig = new PlayerConfig();
           // DisplayLoadedConfigurationMenu(false);
            // playerConfigUI.SetActive(true);
            // setupConnectionBtn.SetActive(false);

        }
    }

    public void WhatsMyIP()
    {
        ipLabel.text = "Your IP is: " + LocalIPAddress() + "\n" + "Please enter this IP on the partner device.";
    }

    public void ToggleConnectionSettingsMenu()
    {
        bool nextstate = !connectionSettingsMenu.activeSelf;
        connectionSettingsMenu.SetActive(nextstate);
        if (nextstate == true)
        {
            playerSelectionMenu.SetActive(true);
            ipSettingsMenu.SetActive(false);
            mainMenu.SetActive(false);
        }
        else if (nextstate == false)
        {
            mainMenu.SetActive(true);
        }
    }
    public void DisplayPlayerSelectionMenu(bool nextstate)
    {
        playerSelectionMenu.SetActive (nextstate);
        if (nextstate == false)
        {
            DisplayIpSettingsMenu(true);
            if (!hasLoadedConfig)
                DisplayLoadedConfigurationMenu(false);  
            else if(hasLoadedConfig)
                DisplayLoadedConfigurationMenu(true);

        }
    }
    public void DisplayIpSettingsMenu(bool nextstate)
    {
        ipSettingsMenu.SetActive(nextstate);
        //if (nextstate == true) ResetIPCharacters();
    }
    public void DisplayLoadedConfigurationMenu(bool nextstate)
    {
        if (nextstate == true)
        {
            manualIPConfig.SetActive(false);
            loadedIPConfig.SetActive(true);
        }
        else if (nextstate == false)
        {
            manualIPConfig.SetActive(true);
            loadedIPConfig.SetActive(false);
        }
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
    public void ConnectLoadedConfiguration()
    {
        _tempIP = loadedIPConfigBtnLabel.text;
        DisplayLoadedConfigurationMenu(false);
        Connect();
    }
    public void Connect()
    {
        if (!string.IsNullOrWhiteSpace(_tempIP) && IsValidIP(_tempIP))
        {
            connectionController.playerConfig.partnerDeviceIP = _tempIP;
            ToggleConnectionSettingsMenu();
            connectionController.TryConnect();

        }
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
        _tempGPSIP = PlayerPrefs.GetString("GPSIP", "");
        connectionController.playerConfig.gpsIP = _tempGPSIP;
        loadedGPSIPConfigBtnLabel.text = _tempGPSIP;
    }
    public void ConnectGPS()
    {
        if (!string.IsNullOrWhiteSpace(_tempGPSIP) && IsValidIP(_tempGPSIP))
        {
            connectionController.playerConfig.gpsIP = _tempGPSIP;
            PlayerPrefs.SetString("GPSIP", _tempGPSIP);

            //ToggleConnectionSettingsMenu();
            //connectionController.TryConnect();

            //call here the GPS initialization

        }
    }
    //end GPS

    public void SetMarkMode(TextMesh buttonLabel)
    {
        bool nextstate = MarkerMode.Instance.SetMarkMode();
        if (nextstate == true)
        {
            buttonLabel.text = "Deactivate Marking";
        }
        else if(nextstate == false)
        {
            buttonLabel.text = "Activate Marking";
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
