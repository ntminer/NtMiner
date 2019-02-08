using NTMiner.NTMinerDaemon;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public class NTMinerClientDaemon {
        public static readonly NTMinerClientDaemon Instance = new NTMinerClientDaemon();

        private NTMinerClientDaemon() { }

        public void GetDaemonVersionAsync(string clientHost, int clientPort, Action<string> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/GetDaemonVersion", null);
                    string response = message.Result.Content.ReadAsAsync<string>().Result;
                    callback?.Invoke(response);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                callback?.Invoke(string.Empty);
            }
        }

        public void RestartWindowsAsync(string clientHost, int clientPort, Action<bool> callback) {
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
        }

        public void ShutdownWindowsAsync(string clientHost, int clientPort, Action<bool> callback) {
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
        }

        public void OpenNTMinerAsync(string clientHost, int clientPort, Guid workId, Action callback) {
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
        }

        public void RestartNTMinerAsync(string clientHost, int clientPort, Guid workId, Action callback) {
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
        }

        public void UpgradeNTMinerAsync(string clientHost, int clientPort, string ntminerFileName, Action callback) {
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
        }

        public void CloseNTMinerAsync(string clientHost, int clientPort, Action callback) {
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
        }

        public void StartAsync(Action callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    if (!NTMinerRoot.Current.IsMining) {
                        callback?.Invoke();
                        return;
                    }
                    var context = NTMinerRoot.Current.CurrentMineContext;
                    StartRequest request = new StartRequest {
                        ContextId = context.Id.GetHashCode(),
                        MinerName = context.MinerName,
                        Coin = context.MainCoin.Code,
                        OurWallet = context.MainCoinWallet,
                        TestWallet = context.MainCoin.TestWallet,
                        KernelName = context.Kernel.FullName
                    };
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3337/api/NTMinerDaemon/StartNoDevFee", request);
                    Write.DevLine(message.Result.ReasonPhrase);
                    callback?.Invoke();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                callback?.Invoke();
            }
        }

        public void StopAsync(Action callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    if (NTMinerRoot.Current.IsMining) {
                        callback?.Invoke();
                        return;
                    }
                    Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:3337/api/NTMinerDaemon/StopNoDevFee", null);
                    Write.DevLine("StopAsync " + message.Result.ReasonPhrase);
                    callback?.Invoke();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                callback?.Invoke();
            }
        }
    }
}
