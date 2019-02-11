using NTMiner.MinerServer;
using NTMiner.Hashrate;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();
            private readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api/Report";

            private ReportServiceFace() { }

            public void ReportSpeedAsync(SpeedData data) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/ReportSpeed", data);
                            Write.DevLine("ReportSpeedAsync " + message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }

            public void ReportStateAsync(Guid clientId, bool isMining) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            ReportStateRequest request = new ReportStateRequest {
                                ClientId = clientId,
                                IsMining = isMining
                            };
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/ReportState", request);
                            Write.DevLine("ReportStateAsync " + message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }
        }
    }
}