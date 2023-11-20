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

namespace Assets.DataManagement
{
    abstract class Connection
    {
        public bool connected = false;
        private float connectDelay = 5; // in seconds
        private DateTime lastConnectAttempt = DateTime.Now;
        protected Connection()
        {
            this.connect();
        }

        protected async virtual void connect()
        {
            while (!this.connected)
            {
                DateTime now = DateTime.Now;
                if ((now - lastConnectAttempt).TotalSeconds > connectDelay)
                {
                    lastConnectAttempt = now;
                    this.connected = await myConnect();
                }
            }
        }
        protected abstract Task<bool> myConnect();
        public abstract Task<string> get(params string[] param);
        public virtual void OnDestroy()
        {

        }
    }

    class HardcodedGPSConnection : Connection
    {
        protected override Task<bool> myConnect()
        {
            return Task.Run(() => true);
        }
        public override Task<string> get(params string[] param)
        {
            // FROM ED: return "$GPRMC,071228.00,A,5402.6015,N,00025.9797,E,0.2,332.1,180921,0.2,W,A,S*50";
            return Task.Run(() => "$GPRMC,071228.00,A,60.403029,N,5.322799,E,0.2,0,180921,0.2,W,A,S*50");
        }
    }

    class BarentswatchAISConnection : Connection
    {
        private HttpClient httpClient = new HttpClient();
        private string token = "";

        protected override async Task<bool> myConnect()
        {
            bool conn = false;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Config.Instance.barentswatch.token_url),
                Content = new StringContent(
                    String.Format(
                        Config.Instance.barentswatch.auth_format,
                        Config.Instance.barentswatch.client_id,
                        Config.Instance.barentswatch.client_secret
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return await Task.Run(() => conn);
        }

        private Uri getUriwithParams(string lonMin, string lonMax, string latMin, string latMax)
        {
            string uri = String.Format(Config.Instance.barentswatch.ais_url, lonMin, lonMax, latMin, latMax);
            return new Uri(uri);
        }

        // Lat Min, Lon Min, Lat Max, Lon Max
        public override async Task<string> get(params string[] param)
        {
            //return await Task.Run(() => "[{\"timeStamp\":\"2021-10-26T18:04:11Z\",\"sog\":0.0,\"rot\":0.0,\"navstat\":5,\"mmsi\":258465000,\"cog\":142.3,\"geometry\":{\"type\":\"Point\",\"coordinates\":[5.317615,60.398463]},\"shipType\":60,\"name\":\"TROLLFJORD\",\"imo\":9233258,\"callsign\":\"LLVT\",\"country\":\"Norge\",\"eta\":\"2021-05-03T17:00:00\",\"destination\":\"BERGEN\",\"isSurvey\":false,\"heading\":142,\"draught\":5.5,\"a\":19,\"b\":117,\"c\":11,\"d\":11}]");


            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = getUriwithParams(param[1], param[3], param[0], param[2])
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
    }

    class PhoneGPSConnection : Connection
    {
        private TcpClient tcpClient;
        private Thread clientReceiveThread;
        public volatile bool running;
        private string lastReading = "";

        protected override Task<bool> myConnect()
        {
            try
            {
                running = true;
                Config.Instance.EnsureInstance();
                clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            } catch (Exception e)
            {
                Debug.Log("On client connect exception " + e);
            }

            return Task.Run(() => true);
        }

        private void ListenForData()
        {
            try
            {
                tcpClient = new TcpClient(Config.Instance.conf.PhoneGPS["IP"], int.Parse(Config.Instance.conf.PhoneGPS["port"])); //new TcpClient(ConnectionController.Instance.playerConfig.gpsIP, ConnectionController.Instance.playerConfig.gpsPort); 
                Byte[] bytes = new Byte[1024];
                //if (DebugOnHead.Instance.gameObject.activeInHierarchy)
                //{  
                    //we want to always set this debug text, even if debug toggle is inactive
                    DebugOnHead.Instance.DebugTextOnHead_1("Connected.. Listening for GPS data via TCP from: " + Config.Instance.conf.PhoneGPS["IP"] + " on port: " + int.Parse(Config.Instance.conf.PhoneGPS["port"]));
                               Debug.Log("Connected.. Listening for GPS data via TCP from: " + Config.Instance.conf.PhoneGPS["IP"] + " on port: " + int.Parse(Config.Instance.conf.PhoneGPS["port"]));
                             
               // }
                while (running)
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            // Convert byte array to string message. 						
                            string gpsString = Encoding.ASCII.GetString(incomingData);

                            
                            
                            // Only store GPRMC strings
                            if (gpsString.Length > 5 && gpsString.Substring(0, 5).Contains("GPRMC"))
                            {
                                lastReading = gpsString.Substring(0, gpsString.IndexOf(Environment.NewLine));
                              //  Debug.Log("received: " + gpsString);
                                //if (!string.IsNullOrEmpty(gpsString))
                                //{
                               //     DebugOnHead.Instance.DebugTextOnHead_1("received: " + gpsString);
                               // }
                            }
                            //this is for testing via the testing app and skips the "$"
                            else if (gpsString.Length > 6 && gpsString.Substring(1, 5).Contains("GPRMC"))
                            {
                                gpsString.TrimStart('$');
                                lastReading = gpsString.Substring(0, gpsString.IndexOf(Environment.NewLine));
                               // Debug.Log("received: " + gpsString);
                                //if (!string.IsNullOrEmpty(gpsString))
                                //{
                               //     DebugOnHead.Instance.DebugTextOnHead_1("received: " + gpsString);
                               // }
                            }

                            if (!running)
                            {
                                break;
                            }
                        }
                    }
                }

                tcpClient.Close();
            } catch (SocketException e)
            {
                //if (DebugOnHead.Instance.gameObject.activeInHierarchy)
                //{
                    //we want to always set this debug text, even if debug toggle is inactive
                    DebugOnHead.Instance.DebugTextOnHead_1("SocketException. Trying to listen for GPS data via TCP from: " + Config.Instance.conf.PhoneGPS["IP"] + " on port: " + int.Parse(Config.Instance.conf.PhoneGPS["port"]));
                //}
                Debug.Log("Trying to listen for GPS data via TCP from: " + Config.Instance.conf.PhoneGPS["IP"]+ " on port: "+int.Parse(Config.Instance.conf.PhoneGPS["port"])); // ConnectionController.Instance.playerConfig.gpsIP + " on port: "+ ConnectionController.Instance.playerConfig.gpsPort + "...but: " + "Socket Exception: " + e);
            }
        }

        public override void OnDestroy()
        {
            running = false;
            clientReceiveThread.Join();
        }

        public override async Task<string> get(params string[] param)
        {
            return await Task.Run(() => lastReading);
        }
    }

    //-----new code:
     class BarentswatchNAVAIDConnection : Connection
    {
        private HttpClient httpClient = new HttpClient();
        private string token = "";

        protected override async Task<bool> myConnect()
        {
            bool conn = false;
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Config.Instance.barentswatch.navaid_token_url),
                Content = new StringContent(
                    String.Format(
                        Config.Instance.barentswatch.navaid_auth_format,
                        Config.Instance.barentswatch.navaid_client_id,
                        Config.Instance.barentswatch.navaid_client_secret
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
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return await Task.Run(() => conn);
        }
/* 
        private Uri getUriwithParams(string lonMin, string lonMax, string latMin, string latMax)
        {
            string uri = String.Format(Config.Instance.barentswatch.navaid_ais_url, lonMin, lonMax, latMin, latMax); // not the final one yet!
            return new Uri(uri);
        } */

        // Lat Min, Lon Min, Lat Max, Lon Max
        public override async Task<string> get(params string[] param)
        {
            //return await Task.Run(() => "[{\"timeStamp\":\"2021-10-26T18:04:11Z\",\"sog\":0.0,\"rot\":0.0,\"navstat\":5,\"mmsi\":258465000,\"cog\":142.3,\"geometry\":{\"type\":\"Point\",\"coordinates\":[5.317615,60.398463]},\"shipType\":60,\"name\":\"TROLLFJORD\",\"imo\":9233258,\"callsign\":\"LLVT\",\"country\":\"Norge\",\"eta\":\"2021-05-03T17:00:00\",\"destination\":\"BERGEN\",\"isSurvey\":false,\"heading\":142,\"draught\":5.5,\"a\":19,\"b\":117,\"c\":11,\"d\":11}]");


            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(Config.Instance.barentswatch.navaid_ais_url)// getUriwithParams(param[1], param[3], param[0], param[2])
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
    }

    class MockNMEAConnection : Connection
    {
        string text = @"$GPGGA,153415.692,6023.762,N,00519.311,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153415.692,A,6023.762,N,00519.311,E,082.0,289.8,151121,000.0,W * 7C
                        $GPGGA,153416.692,6023.776,N,00519.274,E,1,12,1.0,0.0,M,0.0,M,,*6A
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153416.692,A,6023.776,N,00519.274,E,112.0,286.5,151121,000.0,W * 72
                        $GPGGA,153417.692,6023.792,N,00519.220,E,1,12,1.0,0.0,M,0.0,M,,*60
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153417.692,A,6023.792,N,00519.220,E,119.1,291.2,151121,000.0,W * 73
                        $GPGGA,153418.692,6023.812,N,00519.167,E,1,12,1.0,0.0,M,0.0,M,,*68
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153418.692,A,6023.812,N,00519.167,E,108.6,294.4,151121,000.0,W * 7F
                        $GPGGA,153419.692,6023.832,N,00519.122,E,1,12,1.0,0.0,M,0.0,M,,*6A
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153419.692,A,6023.832,N,00519.122,E,098.0,290.8,151121,000.0,W * 7B
                        $GPGGA,153420.692,6023.849,N,00519.078,E,1,12,1.0,0.0,M,0.0,M,,*62
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153420.692,A,6023.849,N,00519.078,E,160.3,292.9,151121,000.0,W * 75
                        $GPGGA,153421.692,6023.878,N,00519.009,E,1,12,1.0,0.0,M,0.0,M,,*67
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153421.692,A,6023.878,N,00519.009,E,116.8,294.0,151121,000.0,W * 75
                        $GPGGA,153422.692,6023.899,N,00518.961,E,1,12,1.0,0.0,M,0.0,M,,*6D
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153422.692,A,6023.899,N,00518.961,E,092.4,294.1,151121,000.0,W * 7F
                        $GPGGA,153423.692,6023.916,N,00518.922,E,1,12,1.0,0.0,M,0.0,M,,*6D
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153423.692,A,6023.916,N,00518.922,E,108.5,295.7,151121,000.0,W * 7B
                        $GPGGA,153424.692,6023.937,N,00518.878,E,1,12,1.0,0.0,M,0.0,M,,*67
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153424.692,A,6023.937,N,00518.878,E,103.7,295.0,151121,000.0,W * 7F
                        $GPGGA,153425.692,6023.957,N,00518.836,E,1,12,1.0,0.0,M,0.0,M,,*6A
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153425.692,A,6023.957,N,00518.836,E,138.1,292.8,151121,000.0,W * 73
                        $GPGGA,153426.692,6023.982,N,00518.776,E,1,12,1.0,0.0,M,0.0,M,,*6A
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153426.692,A,6023.982,N,00518.776,E,135.3,285.0,151121,000.0,W * 72
                        $GPGGA,153427.692,6024.000,N,00518.710,E,1,12,1.0,0.0,M,0.0,M,,*6F
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153427.692,A,6024.000,N,00518.710,E,145.5,287.5,151121,000.0,W * 71
                        $GPGGA,153428.692,6024.021,N,00518.640,E,1,12,1.0,0.0,M,0.0,M,,*67
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153428.692,A,6024.021,N,00518.640,E,166.1,282.7,151121,000.0,W * 7B
                        $GPGGA,153429.692,6024.040,N,00518.555,E,1,12,1.0,0.0,M,0.0,M,,*66
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153429.692,A,6024.040,N,00518.555,E,129.1,284.5,151121,000.0,W * 75
                        $GPGGA,153430.692,6024.057,N,00518.491,E,1,12,1.0,0.0,M,0.0,M,,*61
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153430.692,A,6024.057,N,00518.491,E,131.2,282.9,151121,000.0,W * 72
                        $GPGGA,153431.692,6024.072,N,00518.424,E,1,12,1.0,0.0,M,0.0,M,,*69
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153431.692,A,6024.072,N,00518.424,E,166.5,282.2,151121,000.0,W * 74
                        $GPGGA,153432.692,6024.090,N,00518.338,E,1,12,1.0,0.0,M,0.0,M,,*6C
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153432.692,A,6024.090,N,00518.338,E,195.8,276.9,151121,000.0,W * 70
                        $GPGGA,153433.692,6024.103,N,00518.231,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153433.692,A,6024.103,N,00518.231,E,142.9,276.0,151121,000.0,W * 70
                        $GPGGA,153434.692,6024.112,N,00518.153,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153434.692,A,6024.112,N,00518.153,E,139.4,277.2,151121,000.0,W * 72
                        $GPGGA,153435.692,6024.121,N,00518.077,E,1,12,1.0,0.0,M,0.0,M,,*68
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153435.692,A,6024.121,N,00518.077,E,138.9,276.7,151121,000.0,W * 7C
                        $GPGGA,153436.692,6024.130,N,00518.001,E,1,12,1.0,0.0,M,0.0,M,,*6A
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153436.692,A,6024.130,N,00518.001,E,142.2,277.5,151121,000.0,W * 7B
                        $GPGGA,153437.692,6024.140,N,00517.924,E,1,12,1.0,0.0,M,0.0,M,,*6D
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153437.692,A,6024.140,N,00517.924,E,135.2,279.4,151121,000.0,W * 73
                        $GPGGA,153438.692,6024.152,N,00517.852,E,1,12,1.0,0.0,M,0.0,M,,*61
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153438.692,A,6024.152,N,00517.852,E,144.2,278.9,151121,000.0,W * 75
                        $GPGGA,153439.692,6024.164,N,00517.774,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153439.692,A,6024.164,N,00517.774,E,159.4,279.4,151121,000.0,W * 7C
                        $GPGGA,153440.692,6024.178,N,00517.689,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153440.692,A,6024.178,N,00517.689,E,162.4,278.3,151121,000.0,W * 72
                        $GPGGA,153441.692,6024.191,N,00517.602,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153441.692,A,6024.191,N,00517.602,E,175.8,276.7,151121,000.0,W * 77
                        $GPGGA,153442.692,6024.202,N,00517.506,E,1,12,1.0,0.0,M,0.0,M,,*66
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153442.692,A,6024.202,N,00517.506,E,180.0,277.4,151121,000.0,W * 7A
                        $GPGGA,153443.692,6024.215,N,00517.408,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153443.692,A,6024.215,N,00517.408,E,180.6,277.8,151121,000.0,W * 78
                        $GPGGA,153444.692,6024.228,N,00517.310,E,1,12,1.0,0.0,M,0.0,M,,*69
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153444.692,A,6024.228,N,00517.310,E,153.4,276.4,151121,000.0,W * 7E
                        $GPGGA,153445.692,6024.238,N,00517.226,E,1,12,1.0,0.0,M,0.0,M,,*6D
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153445.692,A,6024.238,N,00517.226,E,147.3,275.8,151121,000.0,W * 77
                        $GPGGA,153446.692,6024.246,N,00517.145,E,1,12,1.0,0.0,M,0.0,M,,*61
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153446.692,A,6024.246,N,00517.145,E,153.7,271.7,151121,000.0,W * 71
                        $GPGGA,153447.692,6024.248,N,00517.059,E,1,12,1.0,0.0,M,0.0,M,,*62
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153447.692,A,6024.248,N,00517.059,E,141.4,271.1,151121,000.0,W * 74
                        $GPGGA,153448.692,6024.250,N,00516.979,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153448.692,A,6024.250,N,00516.979,E,165.1,271.2,151121,000.0,W * 78
                        $GPGGA,153449.692,6024.252,N,00516.886,E,1,12,1.0,0.0,M,0.0,M,,*6C
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153449.692,A,6024.252,N,00516.886,E,169.8,271.5,151121,000.0,W * 78
                        $GPGGA,153450.692,6024.254,N,00516.791,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153450.692,A,6024.254,N,00516.791,E,177.1,272.6,151121,000.0,W * 79
                        $GPGGA,153451.692,6024.259,N,00516.692,E,1,12,1.0,0.0,M,0.0,M,,*65
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153451.692,A,6024.259,N,00516.692,E,150.0,273.5,151121,000.0,W * 71
                        $GPGGA,153452.692,6024.264,N,00516.608,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153452.692,A,6024.264,N,00516.608,E,178.2,275.5,151121,000.0,W * 71
                        $GPGGA,153453.692,6024.273,N,00516.510,E,1,12,1.0,0.0,M,0.0,M,,*66
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153453.692,A,6024.273,N,00516.510,E,183.2,270.0,151121,000.0,W * 78
                        $GPGGA,153454.692,6024.273,N,00516.407,E,1,12,1.0,0.0,M,0.0,M,,*66
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153454.692,A,6024.273,N,00516.407,E,252.6,272.1,151121,000.0,W * 70
                        $GPGGA,153455.692,6024.278,N,00516.265,E,1,12,1.0,0.0,M,0.0,M,,*6E
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153455.692,A,6024.278,N,00516.265,E,243.0,271.1,151121,000.0,W * 7D
                        $GPGGA,153456.692,6024.281,N,00516.129,E,1,12,1.0,0.0,M,0.0,M,,*60
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153456.692,A,6024.281,N,00516.129,E,229.5,271.7,151121,000.0,W * 7C
                        $GPGGA,153457.692,6024.285,N,00516.000,E,1,12,1.0,0.0,M,0.0,M,,*6F
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153457.692,A,6024.285,N,00516.000,E,248.4,272.6,151121,000.0,W * 77
                        $GPGGA,153458.692,6024.291,N,00515.861,E,1,12,1.0,0.0,M,0.0,M,,*69
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153458.692,A,6024.291,N,00515.861,E,284.6,271.8,151121,000.0,W * 7E
                        $GPGGA,153459.692,6024.296,N,00515.702,E,1,12,1.0,0.0,M,0.0,M,,*65
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153459.692,A,6024.296,N,00515.702,E,277.9,274.3,151121,000.0,W * 7F
                        $GPGGA,153500.692,6024.308,N,00515.547,E,1,12,1.0,0.0,M,0.0,M,,*6D
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153500.692,A,6024.308,N,00515.547,E,275.0,270.9,151121,000.0,W * 72
                        $GPGGA,153501.692,6024.310,N,00515.393,E,1,12,1.0,0.0,M,0.0,M,,*6A
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153501.692,A,6024.310,N,00515.393,E,221.1,273.0,151121,000.0,W * 7F
                        $GPGGA,153502.692,6024.316,N,00515.269,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153502.692,A,6024.316,N,00515.269,E,261.1,270.0,151121,000.0,W * 79
                        $GPGGA,153503.692,6024.316,N,00515.122,E,1,12,1.0,0.0,M,0.0,M,,*66
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153503.692,A,6024.316,N,00515.122,E,229.1,275.8,151121,000.0,W * 75
                        $GPGGA,153504.692,6024.329,N,00514.996,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153504.692,A,6024.329,N,00514.996,E,210.7,270.0,151121,000.0,W * 79
                        $GPGGA,153505.692,6024.329,N,00514.878,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153505.692,A,6024.329,N,00514.878,E,165.9,273.2,151121,000.0,W * 77
                        $GPGGA,153506.692,6024.334,N,00514.785,E,1,12,1.0,0.0,M,0.0,M,,*69
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153506.692,A,6024.334,N,00514.785,E,214.1,268.4,151121,000.0,W * 74
                        $GPGGA,153507.692,6024.331,N,00514.665,E,1,12,1.0,0.0,M,0.0,M,,*62
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153507.692,A,6024.331,N,00514.665,E,177.9,264.0,151121,000.0,W * 79
                        $GPGGA,153508.692,6024.321,N,00514.567,E,1,12,1.0,0.0,M,0.0,M,,*6D
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153508.692,A,6024.321,N,00514.567,E,197.0,270.0,151121,000.0,W * 74
                        $GPGGA,153509.692,6024.321,N,00514.456,E,1,12,1.0,0.0,M,0.0,M,,*6F
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153509.692,A,6024.321,N,00514.456,E,215.4,270.6,151121,000.0,W * 7D
                        $GPGGA,153510.692,6024.322,N,00514.335,E,1,12,1.0,0.0,M,0.0,M,,*66
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153510.692,A,6024.322,N,00514.335,E,208.1,268.1,151121,000.0,W * 73
                        $GPGGA,153511.692,6024.318,N,00514.218,E,1,12,1.0,0.0,M,0.0,M,,*60
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153511.692,A,6024.318,N,00514.218,E,179.6,272.9,151121,000.0,W * 74
                        $GPGGA,153512.692,6024.323,N,00514.118,E,1,12,1.0,0.0,M,0.0,M,,*68
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153512.692,A,6024.323,N,00514.118,E,179.6,267.1,151121,000.0,W * 70
                        $GPGGA,153513.692,6024.318,N,00514.018,E,1,12,1.0,0.0,M,0.0,M,,*60
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153513.692,A,6024.318,N,00514.018,E,151.2,270.0,151121,000.0,W * 71
                        $GPGGA,153514.692,6024.318,N,00513.933,E,1,12,1.0,0.0,M,0.0,M,,*60
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153514.692,A,6024.318,N,00513.933,E,170.5,273.1,151121,000.0,W * 77
                        $GPGGA,153515.692,6024.323,N,00513.837,E,1,12,1.0,0.0,M,0.0,M,,*6C
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153515.692,A,6024.323,N,00513.837,E,143.8,274.6,151121,000.0,W * 76
                        $GPGGA,153516.692,6024.330,N,00513.758,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153516.692,A,6024.330,N,00513.758,E,140.2,291.5,151121,000.0,W * 70
                        $GPGGA,153517.692,6024.354,N,00513.696,E,1,12,1.0,0.0,M,0.0,M,,*6B
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153517.692,A,6024.354,N,00513.696,E,090.3,286.3,151121,000.0,W * 7D
                        $GPGGA,153518.692,6024.366,N,00513.652,E,1,12,1.0,0.0,M,0.0,M,,*6D
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153518.692,A,6024.366,N,00513.652,E,075.8,316.8,151121,000.0,W * 78
                        $GPGGA,153519.692,6024.386,N,00513.634,E,1,12,1.0,0.0,M,0.0,M,,*62
                        $GPGSA,A,3,01,02,03,04,05,06,07,08,09,10,11,12,1.0,1.0,1.0 * 30
                        $GPRMC,153519.692,A,6024.386,N,00513.634,E,075.8,316.8,151121,000.0,W * 77";
        string[] splitText;
        private string lastReading = "";
        int index = 3;
        DateTime last;
        float span;

        public override async Task<string> get(params string[] param)
        {
            DateTime now = DateTime.Now;
            if ((now - last).TotalSeconds > span)
            {
                last = now;
                TakeNext();
            }
            return await Task.Run(() => lastReading);
        }

        protected override Task<bool> myConnect()
        {
            last = DateTime.Now;
            span = 2;
            splitText = text.Split('$');
            return Task.Run(() => true);
        }

        protected void TakeNext()
        {
            lastReading = $"${splitText[index]}";
            index += 3;
            if (index > splitText.Length) index = 0;
        }

    }
}


   //Sjoer only storing GPRMC data..
   /*
   GPRMC (Recommended Minimum Specific GNSS Data): This sentence provides the essential fix data which is recommended for GPS applications. The data it can contain includes:
        Time of fix
        Latitude and Longitude
        Speed over ground
        Course over ground (True north reference)
        Date
        Magnetic variation (in some cases)
        Mode (A=Autonomous, D=Differential, E=Estimated, N=Invalid)

    GPGGA (Global Positioning System Fix Data): This sentence provides detailed fix information, including time, location, and fix-related data. The data it can contain includes:
        Time of fix
        Latitude and Longitude
        Fix quality (0=invalid, 1=GPS fix, 2=Differential GPS fix, and others)
        Number of satellites being tracked
        Horizontal dilution of precision (a measure of the geometric quality of the satellite configuration)
        Altitude (above mean sea level)
        Height of geoid above WGS84 ellipsoid

    GPGSA (GNSS DOP and Active Satellites): This sentence provides information on the nature of the fix, including the satellites that were used and the dilution of precision. The data it can contain includes:
        Mode (M=Manual, A=Automatic)
        Fix type (1=Not fixed, 2=2D fix, 3=3D fix)
        Satellite PRNs used for the fix (up to 12)
        Positional dilution of precision (PDOP)
        Horizontal dilution of precision (HDOP)
        Vertical dilution of precision (VDOP) */