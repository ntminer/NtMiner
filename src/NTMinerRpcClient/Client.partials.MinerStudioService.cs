using NTMiner.Controllers;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Client {
        public partial class MinerStudioServiceFace {
            public static readonly MinerStudioServiceFace Instance = new MinerStudioServiceFace();
            private static readonly string s_controllerName = ControllerUtil.GetControllerName<IMinerStudioController>();

            private MinerStudioServiceFace() {
            }

            public void ShowMainWindowAsync(int clientPort, Action<bool, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> getHttpResponse = client.PostAsync($"http://localhost:{clientPort.ToString()}/api/{s_controllerName}/{nameof(IMinerStudioController.ShowMainWindow)}", null);
                            bool response = getHttpResponse.Result.Content.ReadAsAsync<bool>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(false, e);
                    }
                });
            }

            public void CloseMinerStudio() {
                string location = NTMinerRegistry.GetLocation();
                if (string.IsNullOrEmpty(location) || !File.Exists(location)) {
                    return;
                }
                string processName = Path.GetFileNameWithoutExtension(location);
                if (Process.GetProcessesByName(processName).Length == 0) {
                    return;
                }
                bool isClosed = false;
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> getHttpResponse = client.PostAsJsonAsync($"http://localhost:{NTKeyword.MinerStudioPort.ToString()}/api/{s_controllerName}/{nameof(IMinerStudioController.CloseMinerStudio)}", new SignRequest {});
                        ResponseBase response = getHttpResponse.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        isClosed = response.IsSuccess();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
                if (!isClosed) {
                    try {
                        Windows.TaskKill.Kill(processName);
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e);
                    }
                }
            }
        }
    }
}
