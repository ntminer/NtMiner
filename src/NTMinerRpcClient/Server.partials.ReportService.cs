using NTMiner.Controllers;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IReportController>();

            private ReportServiceFace() { }

            public void ReportSpeedAsync(string host, SpeedData data) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IReportController.ReportSpeed)}", data);
                            Write.DevLine("ReportSpeedAsync " + message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }

            public void ReportStateAsync(string host, Guid clientId, bool isMining) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            ReportState request = new ReportState {
                                ClientId = clientId,
                                IsMining = isMining
                            };
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IReportController.ReportState)}", request);
                            Write.DevLine("ReportStateAsync " + message.Result.ReasonPhrase);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }
        }
    }
}