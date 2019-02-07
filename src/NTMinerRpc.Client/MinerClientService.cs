using NTMiner.MinerClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private MinerClientService() {
        }

        public bool ShowMainWindow(string host) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsync($"http://{host}:3336/api/MinerClient/ShowMainWindow", null);
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    return response;
                }
            }
            catch {
                return false;
            }
        }

        public void StartMineAsync(string host, Guid workId, Action<bool> callback) {
            Guid messageId = Guid.NewGuid();
            try {
                using (HttpClient client = new HttpClient()) {
                    StartMineInput request = new StartMineInput() {
                        MessageId = messageId,
                        LoginName = "admin",
                        WorkId = workId,
                        Timestamp = DateTime.Now
                    };
                    request.SignIt(Server.PasswordSha1Sha1);
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:3336/api/MinerClient/StartMine", request);
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch {
                callback?.Invoke(false);
            }
        }

        public void StopMineAsync(string host, Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:3336/api/MinerClient/StopMine", new { timestamp = DateTime.Now });
                    bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                    callback?.Invoke(response);
                }
            }
            catch {
                callback?.Invoke(false);
            }
        }

        public void SetMinerProfilePropertyAsync(string host, string propertyName, object value, Action<bool> callback) {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:3336/api/MinerClient/SetMinerProfileProperty", new { propertyName, value });
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
