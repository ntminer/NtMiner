using NTMiner.Hashrate;
using NTMiner.MinerServer;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IReportController>();
            private readonly string baseUrl = $"http://{MinerServerHost}:{WebApiConst.MinerServerPort}/api/{s_controllerName}";

            private ReportServiceFace() { }
            public void GetTimeAsync(Action<DateTime, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.GetAsync($"{baseUrl}/{nameof(IReportController.GetTime)}");
                            DateTime response = message.Result.Content.ReadAsAsync<DateTime>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(DateTime.Now, e);
                    }
                });
            }

            public void ReportSpeedAsync(SpeedData data) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/{nameof(IReportController.ReportSpeed)}", data);
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
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/{nameof(IReportController.ReportState)}", request);
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