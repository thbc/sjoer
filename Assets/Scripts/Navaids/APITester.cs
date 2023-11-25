using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Assets.Resources;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
public class APITester : MonoBehaviour
{
    [Serializable]
    public class BWAPI_Connection : API_ConnectionBase
    {

        public BWAPI_Connection()
        {
            client_id = "bpj%40tutamail.com%3ASjoer-UiB"; //private readonly
            client_secret = "SjoerARVesselNavigation";
            auth_format = "client_id={0}&scope=api&client_secret={1}&grant_type=client_credentials";
            request_url = "https://www.barentswatch.no/bwapi/v2/geodata/ais/openpositions?Xmin={0}&Xmax={1}&Ymin={2}&Ymax={3}";

        }
    }
    [Serializable]
    public class AISAPI_Connection : API_ConnectionBase
    {
        public AISAPI_Connection()
        {
            client_id = "bpj%40tutamail.com%3ASjoer-UiBNavaids"; //private readonly
            client_secret = "SjoerARVesselNavigation";
            auth_format = "client_id={0}&scope=ais&client_secret={1}&grant_type=client_credentials";
            request_url = "https://historic.ais.barentswatch.no/v1/historic/mmsiinarea";
        }

    }
    [Serializable]
    public class API_ConnectionBase
    {
        public string token_url = "https://id.barentswatch.no/connect/token";
        public string client_id = "";
        public string client_secret = "";
        public string auth_format = "";
        public string request_url;
    }

    [SerializeField]
    public API_ConnectionBase ourAPIConnection;

    public enum APIConnectionType
    {
        BWAPI, AISAPI
    }
    public APIConnectionType connectionType = APIConnectionType.BWAPI;

    private HttpClient httpClient = new HttpClient();
    private string token = "";
    void OnEnable()
    {
        switch (connectionType)
        {
            case APIConnectionType.BWAPI:
                ourAPIConnection = new BWAPI_Connection();
                return;
            case APIConnectionType.AISAPI:
                ourAPIConnection = new AISAPI_Connection();
                return;
        }

    }
    async void Start()
    {

        try
        {
            bool isconn = await myConnect();
            Debug.Log(await getAllAtons());

           // await RepeatedlyLogAtonsAsync();

            //  Debug.Log(await getAllAtonsFromArea());

            //   Debug.Log(await getAllAtonsFromArea());
            //               Debug.Log(await get());
            //Debug.Log(await GetLatestPositionOfOneVesselAsync());
            //            await YourCallingMethod();


        }
        catch (Exception ex)
        {
            // Log the full exception
            Debug.LogError("An error occurred: " + ex.ToString());
        }
    }

    private async Task RepeatedlyLogAtonsAsync()
    {
        while (true)
        {
            try
            {
                string result = await getAllAtons();
                Debug.Log(result);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in getAllAtons: {ex.Message}");
            }
            await Task.Delay(TimeSpan.FromSeconds(15));
        }
    }


    protected async Task<bool> myConnect()
    {
        bool conn = false;
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(ourAPIConnection.token_url),
            Content = new StringContent(
                String.Format(
                    ourAPIConnection.auth_format,
                    ourAPIConnection.client_id,
                    ourAPIConnection.client_secret
                ),
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            )
        };

        string content = "";

        try
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            content = await response.Content.ReadAsStringAsync();
            this.token = JObject.Parse(content)["access_token"].ToString();
            conn = true;
            Debug.Log("WE GOT A TOKEN!: " + this.token);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return await Task.Run(() => conn);
    }



    // Lat Min, Lon Min, Lat Max, Lon Max
    public async Task<string> get(params string[] param)
    {
        //return await Task.Run(() => "[{\"timeStamp\":\"2021-10-26T18:04:11Z\",\"sog\":0.0,\"rot\":0.0,\"navstat\":5,\"mmsi\":258465000,\"cog\":142.3,\"geometry\":{\"type\":\"Point\",\"coordinates\":[5.317615,60.398463]},\"shipType\":60,\"name\":\"TROLLFJORD\",\"imo\":9233258,\"callsign\":\"LLVT\",\"country\":\"Norge\",\"eta\":\"2021-05-03T17:00:00\",\"destination\":\"BERGEN\",\"isSurvey\":false,\"heading\":142,\"draught\":5.5,\"a\":19,\"b\":117,\"c\":11,\"d\":11}]");


        HttpRequestMessage httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://live.ais.barentswatch.no/live/v1/latest/combined?modelType=Full&modelFormat=Json")//"https://historic.ais.barentswatch.no/v1/historic/trackslast24hours/257111020")// getUriwithParams(param[1], param[3], param[0], param[2])
        };

        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

        try
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return await Task.Run(() => "err");

        }

    }



    public async Task<string> GetLatestPositionOfOneVesselAsync()//int[] mmsiNumbers)
    {
        int[] fakearray = new int[1];
        fakearray[0] = 257111020;
        // The URL of the API endpoint for getting the latest position of a vessel
        string requestUrl = "https://live.ais.barentswatch.no/v1/latest/combined";

        // Prepare the request body data
        var requestBody = new
        {
            mmsi = fakearray
        };

        // Convert the request body to a JSON content
        string requestBodyJson = JsonConvert.SerializeObject(requestBody);
        HttpContent httpContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
        {
            // Add the content to the request
            request.Content = httpContent;

            // Add the authorization header with the Bearer token
            if (!string.IsNullOrEmpty(this.token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            }
            else
            {
                Debug.LogError("Token is not obtained or is empty. Cannot proceed with the request.");
                return null;
            }

            try
            {
                // Send the request and get the response
                using (HttpResponseMessage response = await httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode(); // Throws an exception if not successful

                    // Read response content (the vessel position data)
                    string result = await response.Content.ReadAsStringAsync();

                    // Return the result or process it as needed before returning
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (if the request fails, for instance)
                Debug.LogError($"An error occurred while getting the vessel position: {ex}");
                return null;
            }
        }

    }

    public async Task YourCallingMethod()
    {
        // ... (setup if necessary)

        // Call the method and expect an OutputAtonMessage object.
        List<OutputAtonMessage> atonMessages = await GetAtonMessageAsync(/* your params here */);

        if (atonMessages != null && atonMessages.Count > 0)
        {
            // Successfully retrieved and deserialized the messages. You can now access each message.
            foreach (var message in atonMessages)
            {
                Debug.Log($"Received message : {message.Mmsi + "|" + message.MessageType + "|" + message.TypeOfAidsToNavigation + "|" + message.Name}");
            }
        }
        else
        {
            // Handle the case where the messages were not successfully retrieved.
            Debug.LogError("Failed to retrieve the ATON messages.");
        }

        // ... (rest of your method)
    }

    // Change the return type from 'Task<OutputAtonMessage>' to 'Task<List<OutputAtonMessage>>'
    public async Task<List<OutputAtonMessage>> GetAtonMessageAsync(params string[] param)
    {
        int[] fakearray = new int[1];
        fakearray[0] = 257111020;
        // The URL of the API endpoint for getting the latest position of a vessel
        string requestUrl = "https://live.ais.barentswatch.no/v1/latest/combined";

        // Prepare the request body data
        var requestBody = new
        {
            mmsi = fakearray
        };

        // Convert the request body to a JSON content
        string requestBodyJson = JsonConvert.SerializeObject(requestBody);
        HttpContent httpContent = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
        {
            // Add the content to the request
            request.Content = httpContent;

            // Add the authorization header with the Bearer token
            if (!string.IsNullOrEmpty(this.token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            }
            else
            {
                Debug.LogError("Token is not obtained or is empty. Cannot proceed with the request.");
                return null;
            }

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Throws an exception if not successful.

                // Read response content (the JSON string).
                string jsonResult = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string to a list of OutputAtonMessage objects.
                List<OutputAtonMessage> messages = JsonConvert.DeserializeObject<List<OutputAtonMessage>>(jsonResult);

                // Returning the deserialized list of OutputAtonMessage objects instead of a single object.
                return messages;
            }
            catch (Exception ex)
            {
                // Handle exceptions (if the request fails, for instance).
                Debug.LogError($"An error occurred while getting the data: {ex}");

                // Return null or some default instance of List<OutputAtonMessage> in case of an error.
                // Alternatively, you can throw the exception to the calling method.
                return null;
            }

        }
    }

    // Lat Min, Lon Min, Lat Max, Lon Max
    public async Task<string> getAllAtons()
    {
        //return await Task.Run(() => "[{\"timeStamp\":\"2021-10-26T18:04:11Z\",\"sog\":0.0,\"rot\":0.0,\"navstat\":5,\"mmsi\":258465000,\"cog\":142.3,\"geometry\":{\"type\":\"Point\",\"coordinates\":[5.317615,60.398463]},\"shipType\":60,\"name\":\"TROLLFJORD\",\"imo\":9233258,\"callsign\":\"LLVT\",\"country\":\"Norge\",\"eta\":\"2021-05-03T17:00:00\",\"destination\":\"BERGEN\",\"isSurvey\":false,\"heading\":142,\"draught\":5.5,\"a\":19,\"b\":117,\"c\":11,\"d\":11}]");


        HttpRequestMessage httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://live.ais.barentswatch.no/live/v1/latest/ais")//"https://historic.ais.barentswatch.no/v1/historic/trackslast24hours/257111020")// getUriwithParams(param[1], param[3], param[0], param[2])
        };

        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

        try
        {
            // Define the enum for Navaid Types

            var currentLatitude = 60.547975;
            var currentLongitude = 2.731947;
    Debug.Log("query for: "+httpRequestMessage);
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();

            JArray navaids = JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync());
//           Debug.LogWarning(navaids);
          // return navaids.ToString();

            string allTypes = "";
            int amount=0;
            foreach (var item in navaids)
            {
                /* if(!String.IsNullOrEmpty(item["navigationId"].ToString()))
                {
                   allTypes = allTypes + item;
                } */

                if (item["type"] != null)
                {
                    if (item["type"].ToString() == "Aton")
                    {
                       // Debug.Log(item);
                       //if((int)item["mmsi"] == 992572062)
                      //   Debug.LogWarning($"Type: {item["type"].ToString()}\n{item}");
                       
                         if(!allTypes.Contains(item["typeOfAidsToNavigation"].ToString()))
                        {
                            allTypes = allTypes + " | " + item["typeOfAidsToNavigation"].ToString();
                        }
                            amount++; 
                        


                    }
                    if (item["type"].ToString() == "Staticdata")
                    {
                       // if(item["id"].ToString() == "115156")
                  //      Debug.LogWarning($"Type: {item["type"].ToString()}\n{item}");
                       /* if(!allTypes.Contains(item["type"].ToString()))
                        {
                            allTypes = allTypes + " | " + item["Staticdata"].ToString();
                        }
                            amount++; */

                    }
                    if (item["type"].ToString() == "SafetyRelated")
                    {
                     //   Debug.LogWarning($"Type: {item["type"].ToString()}\n{item}");

                    }

                }

                /* if (item["type"] != null && item["type"].ToString() == "Aton" &&
                    item["typeOfAidsToNavigation"] != null &&
                    Enum.IsDefined(typeof(NavaidType), (int)item["typeOfAidsToNavigation"]))
                {
                    // Calculate distance from current position to the aton (assuming you have a method to do so)
                   
                double distance = CalculateDistanceBetweenPoints(currentLatitude, currentLongitude, 0,(double)item["latitude"], (double)item["longitude"],0);
                    // Displaying the log as per requirement with the distance above
                    Debug.LogWarning($"Distance: {distance*0.001}km\n{item}");
                } */
            }
            //Debug.LogWarning(allTypes);
            allTypes = allTypes + " ["+amount+"]";




            return allTypes;// await response.Content.ReadAsStringAsync();

        }
        catch (Exception e)
        {
            Debug.Log(e);
            return await Task.Run(() => "err");

        }

    }

    public double CalculateDistanceBetweenPoints(double lat1, double lon1, double h1,
                                                 double lat2, double lon2, double h2)
    {
        // Convert both points to ENU coordinates using the first point as the reference
        double xEast1, yNorth1, zUp1, xEast2, yNorth2, zUp2;

        Assets.HelperClasses.GPSUtils.Instance.GeodeticToEnu(lat1, lon1, h1, lat1, lon1, h1, out xEast1, out yNorth1, out zUp1);
        Assets.HelperClasses.GPSUtils.Instance.GeodeticToEnu(lat2, lon2, h2, lat1, lon1, h1, out xEast2, out yNorth2, out zUp2);

        // Calculate the Euclidean distance between the two ENU points
        double dx = xEast2 - xEast1;
        double dy = yNorth2 - yNorth1;
        double dz = zUp2 - zUp1;

        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }


    public enum NavaidType
    {
        LightWithoutSectors = 6,
        CardinalMarkN = 20,
        CardinalMarkE = 21,
        CardinalMarkS = 22,
        CardinalMarkW = 23,
        BeaconPortHand = 13,
        BeaconStarboardHand = 14//,
                                // SpecialMark = 30
    }


    public class AtonMessage
    {
        public string type { get; set; }
        public int messageType { get; set; }
        public int mmsi { get; set; }
        public string msgtime { get; set; }  // You can change this to DateTime if it's in standard datetime format
        public int? dimensionA { get; set; }
        public int? dimensionB { get; set; }
        public int? dimensionC { get; set; }
        public int? dimensionD { get; set; }
        public int typeOfAidsToNavigation { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string name { get; set; }
        public int? typeOfElectronicFixingDevice { get; set; } // nullable because of "nullLabel: true"
    }

    private Uri getUriwithParams(string requrl, string lonMin, string lonMax, string latMin, string latMax)
    {
        string uri = String.Format(requrl, lonMin, lonMax, latMin, latMax);
        return new Uri(uri);
    }
    public Tuple<Vector2, Vector2> GetCurrentLatLonArea(double lat, double lon)
    {
        //offsets in meters
        double dn = 3000;
        double de = 3000;

        return new Tuple<Vector2, Vector2>(
            OffsetLatLonByMeter(lat, lon, -dn, -de),
            OffsetLatLonByMeter(lat, lon, dn, de)
        );
    }

    private Vector2 OffsetLatLonByMeter(double lat, double lon, double dn, double de)
    {
        // Code retrieved from https://gis.stackexchange.com/a/2980

        // Earth’s radius, sphere
        double R = 6378137;

        // Coordinate offsets in radians
        double dLat = dn / R;
        double dLon = de / (R * Math.Cos(Math.PI * lat / 180));

        // OffsetPosition, decimal degrees
        double latO = lat + dLat * 180 / Math.PI;
        double lonO = lon + dLon * 180 / Math.PI;

        return new Vector2((float)latO, (float)lonO);
    }
    // Lat Min, Lon Min, Lat Max, Lon Max
    public async Task<string> getAllAtonsFromArea()
    {
        string query = "https://live.ais.barentswatch.no/live/v1/latest/ais";
        double lat = 60.397908;
        double lon = 5.317065;
        var latLonArea = GetCurrentLatLonArea(lat, lon);

        var bottomLeft = latLonArea.Item1; // assuming Item1 is the bottom left
        var topRight = latLonArea.Item2;   // assuming Item2 is the top right

        var payload = new
        {
            geometry = new
            {
                type = "Polygon",
                coordinates = new List<List<List<double>>>
                {
                        new List<List<double>>
                        {
                            new List<double> { bottomLeft.y, bottomLeft.x }, // bottom left
                            new List<double> { bottomLeft.y, topRight.x },   // bottom right
                            new List<double> { topRight.y, topRight.x },     // top right
                            new List<double> { topRight.y, bottomLeft.y },   // top left
                            new List<double> { bottomLeft.y, bottomLeft.x }  // closing with bottom left again
                        }
                    }
            },
            since = DateTime.UtcNow.ToString("o"), // current time in ISO 8601 format
            includePosition = false, //true,
            includeStatic = true, //true,
            includeAton = true,
            includeSafetyRelated = true,//true,
            includeBinaryBroadcastMetHyd = true//true
        };

        HttpRequestMessage httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(query),
            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        };

        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

        try
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return await Task.Run(() => "err");
        }
    }


    public class OutputAtonMessage
    {
        public string Type { get; set; }
        public string MessageType { get; set; }
        public int Mmsi { get; set; }
        public DateTime Msgtime { get; set; }
        public int DimensionA { get; set; }
        public int DimensionB { get; set; }
        public int DimensionC { get; set; }
        public int DimensionD { get; set; }
        public string TypeOfAidsToNavigation { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string TypeOfElectronicFixingDevice { get; set; }
    }


}
