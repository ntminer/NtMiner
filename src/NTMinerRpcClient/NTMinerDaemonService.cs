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

        public void RestartWindowsAsync(string clientHost, int clientPort, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/RestartWindows", null);
                        bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void ShutdownWindowsAsync(string clientHost, int clientPort, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/ShutdownWindows", null);
                        bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void OpenNTMinerAsync(string clientHost, int clientPort, Guid workId, Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        OpenNTMinerRequest request = new OpenNTMinerRequest {
                            WorkId = workId
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/OpenNTMiner", request);
                        Write.DevLine("OpenNTMinerAsync " + message.Result.ReasonPhrase);
                        callback?.Invoke();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke();
                }
            });
        }

        public void RestartNTMinerAsync(string clientHost, int clientPort, Guid workId, Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        RestartNTMinerRequest request = new RestartNTMinerRequest {
                            WorkId = workId
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/RestartNTMiner", request);
                        Write.DevLine("RestartNTMinerAsync " + message.Result.ReasonPhrase);
                        callback?.Invoke();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke();
                }
            });
        }

        public void UpgradeNTMinerAsync(string clientHost, int clientPort, string ntminerFileName, Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        UpgradeNTMinerRequest request = new UpgradeNTMinerRequest {
                            NTMinerFileName = ntminerFileName
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/UpgradeNTMiner", request);
                        Write.DevLine("UpgradeNTMinerAsync " + message.Result.ReasonPhrase);
                        callback?.Invoke();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke();
                }
            });
        }

        public void CloseNTMinerAsync(string clientHost, int clientPort, Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/CloseNTMiner", null);
                        Write.DevLine("CloseNTMinerAsync " + message.Result.ReasonPhrase);
                        callback?.Invoke();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke();
                }
            });
        }

        public void StartNoDevFeeAsync(StartNoDevFeeRequest request, Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3337/api/NTMinerDaemon/StartNoDevFee", request);
                        Write.DevLine("StartNoDevFeeAsync " + message.Result.ReasonPhrase);
                        callback?.Invoke();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke();
                }
            });
        }

        public void StopNoDevFeeAsync(Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:3337/api/NTMinerDaemon/StopNoDevFee", null);
                        Write.DevLine("StopAsync " + message.Result.ReasonPhrase);
                        callback?.Invoke();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke();
                }
            });
        }
    }
}
