using NTMiner.Controllers;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class ReportServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IReportController>();

            public ReportServiceFace() {
            }

            public void ReportSpeedAsync(string host, SpeedData data, Action<ReportResponse> callback) {
                Task.Factory.StartNew(() => {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(3);
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            // 可能超过3秒钟，查查原因。因为我的网络不稳经常断线。
                            client.Timeout = timeSpan;
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{host}:{NTKeyword.ControlCenterPort.ToString()}/api/{SControllerName}/{nameof(IReportController.ReportSpeed)}", data);
                            ReportResponse response = getHttpResponse.Result.Content.ReadAsAsync<ReportResponse>().Result;
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        Write.DevException(e);
                    }
                });
            }

            public void ReportStateAsync(string host, Guid clientId, bool isMining) {
                Task.Factory.StartNew(() => {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(3);
                    try {
                        using (HttpClient client = RpcRoot.Create()) {
                            client.Timeout = timeSpan;
                            ReportState request = new ReportState {
                                ClientId = clientId,
                                IsMining = isMining
                            };
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://{host}:{NTKeyword.ControlCenterPort.ToString()}/api/{SControllerName}/{nameof(IReportController.ReportState)}", request);
                            Write.DevDebug($"{nameof(ReportStateAsync)} {getHttpResponse.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        Write.DevException(e);
                    }
                });
            }
        }
    }
}