using NTMiner.Daemon;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Client {
        public class NTMinerDaemonServiceFace {
            public static readonly NTMinerDaemonServiceFace Instance = new NTMinerDaemonServiceFace();

            private NTMinerDaemonServiceFace() { }

            public void GetDaemonVersionAsync(Action<string, Exception> callback) {
                Process[] processes = Process.GetProcessesByName("NTMinerDaemon");
                if (processes.Length == 0) {
                    callback?.Invoke(string.Empty, null);
                }
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:3337/api/NTMinerDaemon/GetDaemonVersion", null);
                            string response = message.Result.Content.ReadAsAsync<string>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(string.Empty, e);
                    }
                });
            }

            public void SetMinerNameAsync(MinerClient.SetMinerNameRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3337/api/NTMinerDaemon/SetMinerName", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void RefreshUserSetAsync(Action<Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:3337/api/NTMinerDaemon/RefreshUserSet", null);
                            Write.DevLine("RefreshUserSetAsync " + message.Result.ReasonPhrase);
                            callback?.Invoke(null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(e);
                    }
                });
            }

            public void RestartWindowsAsync(RestartWindowsRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3337/api/NTMinerDaemon/RestartWindows", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void ShutdownWindowsAsync(ShutdownWindowsRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3337/api/NTMinerDaemon/ShutdownWindows", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void OpenNTMinerAsync(OpenNTMinerRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3337/api/NTMinerDaemon/OpenNTMiner", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void RestartNTMinerAsync(RestartNTMinerRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3337/api/NTMinerDaemon/RestartNTMiner", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void UpgradeNTMinerAsync(UpgradeNTMinerRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3337/api/NTMinerDaemon/UpgradeNTMiner", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void CloseNTMinerAsync(CloseNTMinerRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{request.ClientIp}:3337/api/NTMinerDaemon/CloseNTMiner", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void StartNoDevFeeAsync(StartNoDevFeeRequest request, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3337/api/NTMinerDaemon/StartNoDevFee", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }

            public void StopNoDevFeeAsync(Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            RequestBase request = new RequestBase();
                            Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3337/api/NTMinerDaemon/StopNoDevFee", request);
                            ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
        }
    }
}
