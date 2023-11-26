using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Assets.DataManagement.Navaids;
using Assets.DataManagement.Navaids.WFSRequestHelper;
using Assets.Positional;
using System.Collections.Generic;
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
        public double[] boundingBox;


        public List<NavaidData.Navaid> retrievedFeatures = new List<NavaidData.Navaid>();

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
                        Tuple<Vector2, Vector2> currLatLonArea = Player.Instance.GetCurrentLatLonArea(10000);//(10000);
                        Debug.LogWarning(currLatLonArea.ToString());
                        boundingBox = new double[4] { currLatLonArea.Item1.y, currLatLonArea.Item2.y, currLatLonArea.Item1.x, currLatLonArea.Item2.x };

                        //TODO:
                        // include other types of cardinal markers that are not Flytandemerke (floating devices)


                        string result_Lys = await AsyncRequest(cancellationToken, boundingBox, "Lys");

                        /* NavaidData.NavaidDataParser parser = new NavaidData.NavaidDataParser();
                        retrievedFeatures = parser.GetNavaidFeaturesFromString(result_seamarkersBeacon); */
                        NavaidData.NavaidDataXMLParser parser = new NavaidData.NavaidDataXMLParser();
                        retrievedFeatures = parser.ParseXml(result_Lys);
                        //    Debug.LogWarning(NavaidData.BabordLateralCounter+ "(BabordLateralCounter)||"+NavaidData.StyrbordLateralCounter+ "(StyrbordLateralCounter)||"+NavaidData.LyktefundamentCounter+ "(LyktefundamentCounter)||"+NavaidData.kardinalCounter+ "(kardinalCounter)||");

                        string result_IB = await AsyncRequest(cancellationToken, boundingBox, "IB");
                        retrievedFeatures.AddRange(parser.ParseXml(result_IB));
                        //    Debug.LogWarning(NavaidData.BabordLateralCounter+ "(BabordLateralCounter)||"+NavaidData.StyrbordLateralCounter+ "(StyrbordLateralCounter)||"+NavaidData.LyktefundamentCounter+ "(LyktefundamentCounter)||"+NavaidData.kardinalCounter+ "(kardinalCounter)||");


                        // seamarkersCardinal = new NavaidData.FeatureCollection();
                        // string result_seamarkersCardinal = await AsyncRequest(cancellationToken, NavaidTypes.CardinalMarker, boundingBox);

                        //   seamarkersLight = new NavaidData.FeatureCollection();
                        //   string result_seamarkersLight = await AsyncRequest(cancellationToken, NavaidTypes.Lights, boundingBox);

                        //  seamarkersBeacon = NavaidData.GetFeatureCollection(result_seamarkersBeacon);
                        //  seamarkersCardinal = NavaidData.GetFeatureCollection(result_seamarkersCardinal);
                        //   seamarkersLight = NavaidData.GetFeatureCollection(result_seamarkersLight);

                        //   var allSeamarkersFeatures = new List<NavaidData.Feature>(seamarkersBeacon.features);
                        // allSeamarkersFeatures.AddRange(seamarkersCardinal.features);
                        // allSeamarkersFeatures.AddRange(seamarkersLight.features);

                        //  var allSeamarkers = new NavaidData.FeatureCollection { features = allSeamarkersFeatures };

                        //NavaidData.DebugNavaidsInNavaidCollection(fixedSeamarkers);
                        navaidDataFactory.UpdateNavaids(retrievedFeatures);

                        navaidDataFactory.DisplayNavaids();

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

        public async Task<string> AsyncRequest(CancellationToken cancellationToken, double[] bbox, string _typename)
        {
            string wfsRequestUrl = WFSRequestBuilder.Request_SeamarkersAll(bbox[0], bbox[1], bbox[2], bbox[3], _typename);

            /* switch (navaidType)
            {
                case NavaidTypes.BeaconMarker:
                    wfsRequestUrl = WFSRequestBuilder.Request_SeamarkersBeacon(bbox[0], bbox[1], bbox[2], bbox[3]);
                    break;
                case NavaidTypes.CardinalMarker:
                    wfsRequestUrl = WFSRequestBuilder.Request_SeamarkersCardinal(bbox[0], bbox[1], bbox[2], bbox[3]);
                    break;
                case NavaidTypes.Lights:
                    wfsRequestUrl = WFSRequestBuilder.Request_SeamarkersLight(bbox[0], bbox[1], bbox[2], bbox[3]);
                    break;
            } */
            Debug.LogWarning("request: " + wfsRequestUrl);

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
                    Debug.LogWarning("response: " + responseBody);
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