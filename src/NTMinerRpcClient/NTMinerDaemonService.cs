using NTMiner.Daemon;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public class NTMinerDaemonService {
        public static readonly NTMinerDaemonService Instance = new NTMinerDaemonService();

        private NTMinerDaemonService() { }

        public void GetDaemonVersionAsync(Action<string> callback) {
            Process[] processes = Process.GetProcessesByName("NTMinerDaemon");
            if (processes.Length == 0) {
                callback?.Invoke(string.Empty);
            }
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:3337/api/NTMinerDaemon/GetDaemonVersion", null);
                        string response = message.Result.Content.ReadAsAsync<string>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(string.Empty);
                }
            });
        }

        public void RestartWindowsAsync(string clientHost, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        RequestBase request = new RequestBase();
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3337/api/NTMinerDaemon/RestartWindows", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void ShutdownWindowsAsync(string clientHost, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        RequestBase request = new RequestBase();
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3337/api/NTMinerDaemon/ShutdownWindows", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void OpenNTMinerAsync(string clientHost, Guid workId, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        OpenNTMinerRequest request = new OpenNTMinerRequest {
                            WorkId = workId
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3337/api/NTMinerDaemon/OpenNTMiner", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void RestartNTMinerAsync(string clientHost, Guid workId, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        RestartNTMinerRequest request = new RestartNTMinerRequest {
                            WorkId = workId
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3337/api/NTMinerDaemon/RestartNTMiner", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void UpgradeNTMinerAsync(string clientHost, string ntminerFileName, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        UpgradeNTMinerRequest request = new UpgradeNTMinerRequest {
                            NTMinerFileName = ntminerFileName
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3337/api/NTMinerDaemon/UpgradeNTMiner", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void CloseNTMinerAsync(string clientHost, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        RequestBase request = new RequestBase();
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3337/api/NTMinerDaemon/CloseNTMiner", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void StartNoDevFeeAsync(StartNoDevFeeRequest request, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3337/api/NTMinerDaemon/StartNoDevFee", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void StopNoDevFeeAsync(Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        RequestBase request = new RequestBase();
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3337/api/NTMinerDaemon/StopNoDevFee", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }
    }
}
