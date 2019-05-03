using NTMiner.Controllers;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Net;
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
                    TimeSpan timeSpan = TimeSpan.FromSeconds(3);
                    try {
                        using (HttpClient client = new HttpClient()) {
                            // 可能超过3秒钟，查查原因。因为我的网络不稳经常断线。
                            client.Timeout = timeSpan;
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IReportController.ReportSpeed)}", data);
                            Write.DevDebug($"{nameof(ReportSpeedAsync)} {message.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        if (e is TaskCanceledException) {
                            Write.DevError($"本次ReportSpeedAsync已取消，因为耗时超过{timeSpan.TotalSeconds}秒");
                        }
                        else {
                            e = e.GetInnerException();
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                    }
                });
            }

            public void ReportStateAsync(string host, Guid clientId, bool isMining) {
                Task.Factory.StartNew(() => {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(3);
                    try {
                        using (HttpClient client = new HttpClient()) {
                            client.Timeout = timeSpan;
                            ReportState request = new ReportState {
                                ClientId = clientId,
                                IsMining = isMining
                            };
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:{WebApiConst.ControlCenterPort}/api/{SControllerName}/{nameof(IReportController.ReportState)}", request);
                            Write.DevDebug($"{nameof(ReportStateAsync)} {message.Result.ReasonPhrase}");
                        }
                    }
                    catch (Exception e) {
                        if (e is TaskCanceledException) {
                            Write.DevError($"本次ReportStateAsync已取消，因为耗时超过{timeSpan.TotalSeconds}秒");
                        }
                        else {
                            e = e.GetInnerException();
                            Logger.ErrorDebugLine(e.Message, e);
                        }
                    }
                });
            }
        }
    }
}