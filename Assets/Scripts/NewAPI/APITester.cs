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
            await myConnect();
            Debug.Log(await getAllAtons());
                       Debug.Log(await get());
 Debug.Log(await GetLatestPositionOfOneVesselAsync());
                        await YourCallingMethod();


        }
        catch (Exception ex)
        {
            // Log the full exception
            Debug.LogError("An error occurred: " + ex.ToString());
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

    private Uri getUriwithParams(string lonMin, string lonMax, string latMin, string latMax)
    {
        string uri = String.Format(ourAPIConnection.request_url, lonMin, lonMax, latMin, latMax);
        return new Uri(uri);
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
            Debug.Log($"Received message : {message.Mmsi +"|"+ message.MessageType+"|"+ message.TypeOfAidsToNavigation+"|" + message.Name}");
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
