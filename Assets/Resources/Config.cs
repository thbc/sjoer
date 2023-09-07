using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Assets.Resources
{
    class Config : Assets.HelperClasses.CSSingleton<Config>
    {
        public Conf conf;
        public BarentsConf barentswatch;
        public Config()
        {
            conf = JsonConvert.DeserializeObject<Conf>(AssetManager.Instance.config["generic"].text);
            // we have to set this from here to make sure this is set on Start when config is initialized
            conf.PhoneGPS["IP"] = PlayerPrefs.GetString("GPSIP", "");
            Debug.LogWarning("Attention: We overwrite the config file PhoneGPS IP value and take the value from the PlayerPrefs..");
            barentswatch = JsonConvert.DeserializeObject<BarentsConf>(AssetManager.Instance.config["barentswatch"].text);
        }

        public void EnsureInstance()
        {
            // Callable to instantiate instance if it does not exists yet
        }
    }

    [Serializable]
    public class Conf
    {
        public bool VesselMode;
        public Dictionary<string, double> DataSettings;
        // Double typed vessel settings
        public Dictionary<string, double> VesselSettingsD;
        // String typed vessel settings
        public Dictionary<string, string> VesselSettingsS;
        public Dictionary<string, double> NonVesselSettings;
        public Dictionary<string, int> SceneSettings;
        public Dictionary<string, int> CalibrationSettings;
        public Dictionary<string, double> UISettings;
        public Dictionary<string, string> PhoneGPS;

    }

    [Serializable]
    public class BarentsConf
    {
        public string token_url;
        public string ais_url;
        public string auth_format;
        public string client_id;
        public string client_secret;
    }
}
