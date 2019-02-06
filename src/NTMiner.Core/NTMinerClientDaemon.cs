using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public class NTMinerClientDaemon {
        public static readonly NTMinerClientDaemon Instance = new NTMinerClientDaemon();
        private readonly string baseUrl = $"http://localhost:3337/api/NTMinerDaemon";

        private NTMinerClientDaemon() { }

        public void GetDaemonVersionAsync(string clientHost, int clientPort, Action<string> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.GetAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/GetDaemonVersion");
                    string response = message.Result.Content.ReadAsAsync<string>().Result;
                    callback?.Invoke(response);
                }
            }
            catch {
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
                callback?.Invoke(false);
            }
        }

        public void OpenNTMinerAsync(string clientHost, int clientPort, Guid workId, Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/OpenNTMiner", new { workId });
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch (Exception e) {
                callback?.Invoke(false);
            }
        }

        public void RestartNTMinerAsync(string clientHost, int clientPort, Guid workId, Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/RestartNTMiner", new { workId });
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch (Exception e) {
                callback?.Invoke(false);
            }
        }

        public void CloseNTMinerAsync(string clientHost, int clientPort, Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/CloseNTMiner", null);
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch (Exception e) {
                callback?.Invoke(false);
            }
        }

        public void IsNTMinerDaemonOnlineAsync(string clientHost, int clientPort, Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/IsNTMinerDaemonOnline", null);
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch (Exception e) {
                callback?.Invoke(false);
            }
        }

        public void IsNTMinerOnlineAsync(string clientHost, int clientPort, Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:{clientPort}/api/NTMinerDaemon/IsNTMinerOnline", null);
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch (Exception e) {
                callback?.Invoke(false);
            }
        }

        public void StartAsync(Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    if (!NTMinerRoot.Current.IsMining) {
                        callback?.Invoke(false);
                        return;
                    }
                    var context = NTMinerRoot.Current.CurrentMineContext;
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:3337/api/NTMinerDaemon/StartNoDevFee", new {
                        contextId = context.Id.GetHashCode(),
                        minerName = context.MinerName,
                        coin = context.MainCoin.Code,
                        ourWallet = context.MainCoinWallet,
                        testWallet = context.MainCoin.TestWallet,
                        kernelName = context.Kernel.FullName
                    });
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch {
                callback?.Invoke(false);
            }
        }

        public void StopAsync(Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    if (NTMinerRoot.Current.IsMining) {
                        callback?.Invoke(false);
                        return;
                    }
                    Task<HttpResponseMessage> message = client.PostAsync($"http://localhost:3337/api/NTMinerDaemon/StopNoDevFee", null);
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch {
                callback?.Invoke(false);
            }
        }
    }
}
