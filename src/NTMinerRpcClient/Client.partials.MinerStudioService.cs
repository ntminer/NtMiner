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
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{clientPort}/api/{s_controllerName}/{nameof(IMinerStudioController.ShowMainWindow)}", null);
                            bool response = message.Result.Content.ReadAsAsync<bool>().Result;
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
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{Consts.MinerStudioPort}/api/{s_controllerName}/{nameof(IMinerStudioController.CloseMinerStudio)}", new SignRequest {});
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
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
