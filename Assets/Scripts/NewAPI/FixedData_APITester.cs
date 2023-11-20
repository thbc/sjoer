using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WFSRequestHelper;
public class FixedData_APITester : MonoBehaviour
{
    string requestURLbase = "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer?service=WFS&request=GetFeature&version=2.0.0&";

    private HttpClient httpClient = new HttpClient();
    private CancellationTokenSource cancellationTokenSource;
    async void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        try
        {
            await RepeatedlyAsyncRequest(cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Request was canceled.");
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred: " + ex.ToString());
        }
    }

    private async Task RepeatedlyAsyncRequest(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                string result = await AsyncRequest(cancellationToken);
                var data = KystverketData.GetFeatureCollection(result);
                KystverketData.DebugFeaturesInFeatureCollection(data);
                //Debug.Log(result);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Request was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in AsyncRequest: {ex.Message}");
            }
            await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
        }
    }

    public async Task<string> AsyncRequest(CancellationToken cancellationToken)
    {
        string wfsRequestUrlFastsjømerke = WFSRequestBuilder.Request_Seamarkers(5.135830, 5.167121, 60.417901, 60.430572);
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(wfsRequestUrlFastsjømerke)
        };

        try
        {
            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Request was canceled.");
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return "err";
        }
    }




    void OnDestroy()
    {
        // Cancel all pending requests when the object is destroyed
        cancellationTokenSource?.Cancel();
        httpClient?.Dispose();
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


}
