using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Assets.DataManagement.Navaids;
using Assets.DataManagement.Navaids.WFSRequestHelper;
using Assets.Positional;
/// <summary>
/// Since the navaid data is fixed/static it does not have the same requirements as the vessel data. Therefore, it was decided to implement it via MonoBehaviour as it's own class,
/// allowing for simplicity and ease-of-use.
/// </summary>
namespace Assets.DataManagement.Navaids
{
    public class NavaidDataRetriever : MonoBehaviour
    {
        string requestURLbase = "https://nfs.kystverket.no/arcgis/services/nfs/NFSSistOperativ/MapServer/WFSServer?service=WFS&request=GetFeature&version=2.0.0&";

        private HttpClient httpClient = new HttpClient();
        private CancellationTokenSource cancellationTokenSource;

        public enum NavaidTypes
        {
            Seamarkers, CardinalMarkers, Lights
        }
        async void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await CheckPositionBbox(cancellationTokenSource.Token);
                //string result_seamarkers = await AsyncRequest(cancellationTokenSource.Token, NavaidTypes.Seamarkers, boundingBox);

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
        private Tuple<Vector2, Vector2> previousLatLonArea = null;
        double[] boundingBox;
        ///
        public NavaidData.FeatureCollection fixedSeamarkers = new NavaidData.FeatureCollection();
        public NavaidDataFactory navaidDataFactory;
        private async Task CheckPositionBbox(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Player.Instance.HasAreaChanged())
                {
                    try
                    {
                        // latmin lonmin latmax lonmax
                        /* latLonArea.Item1.x.ToString(),
                        latLonArea.Item1.y.ToString(),
                        latLonArea.Item2.x.ToString(),
                        latLonArea.Item2.y.ToString() */
                        //boundingBox = new double[4] { 5.135830, 5.167121, 60.417901, 60.430572 };
                        Tuple<Vector2, Vector2> currLatLonArea = Player.Instance.GetCurrentLatLonArea();//(10000);
                        boundingBox = new double[4] { currLatLonArea.Item1.y, currLatLonArea.Item2.y, currLatLonArea.Item1.x, currLatLonArea.Item2.x };
                        string result_seamarkers = await AsyncRequest(cancellationToken, NavaidTypes.Seamarkers, boundingBox);
                        fixedSeamarkers = NavaidData.GetFeatureCollection(result_seamarkers);
                        //NavaidData.DebugNavaidsInNavaidCollection(fixedSeamarkers);
                        navaidDataFactory.UpdateFeatures(fixedSeamarkers);
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
                }
                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
            }
        }

        public async Task<string> AsyncRequest(CancellationToken cancellationToken, NavaidTypes navaidType, double[] bbox)
        {
            string wfsRequestUrl = "";
            switch (navaidType)
            {
                case NavaidTypes.Seamarkers:
                    wfsRequestUrl = WFSRequestBuilder.Request_Seamarkers(bbox[0], bbox[1], bbox[2], bbox[3]);
                    break;
            }

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(wfsRequestUrl)
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(wfsRequestUrl, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Request was canceled.");
                throw;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request Exception: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e.Message}");
                return "err";
            }
        }
        void OnDestroy()
        {
            // Cancel all pending requests when the object is destroyed
            cancellationTokenSource?.Cancel();
            httpClient?.Dispose();
        }

    }
}