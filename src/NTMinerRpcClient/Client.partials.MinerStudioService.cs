using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Client {
        public partial class MinerStudioServiceFace {
            public static readonly MinerStudioServiceFace Instance = new MinerStudioServiceFace();
            private static readonly string s_controllerName = "MinerStudio";

            private MinerStudioServiceFace() {
            }

            public void ShowMainWindowAsync(int clientPort, Action<bool, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:{clientPort}/api/{s_controllerName}/ShowMainWindow", null);
                            bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        e = e.GetInnerException();
                        callback?.Invoke(false, e);
                    }
                });
            }

            public void CloseMinerStudio() {
                bool isClosed = false;
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.MinerStudioPort}/api/{s_controllerName}/CloseMinerStudio", new SignatureRequest {});
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        isClosed = response.IsSuccess();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
                if (!isClosed) {
                    try {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                            string processName = Path.GetFileNameWithoutExtension(location);
                            Windows.TaskKill.Kill(processName);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                }
            }
        }
    }
}
