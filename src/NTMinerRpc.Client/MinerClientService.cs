using NTMiner.MinerClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private MinerClientService() {
        }

        public void ShowMainWindowAsync(string host, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://{host}:3336/api/MinerClient/ShowMainWindow", null);
                        bool response = message.Result.Content.ReadAsAsync<bool>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e){
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void StartMineAsync(string host, Guid workId, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        StartMineRequest request = new StartMineRequest() {
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
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(false);
                }
            });
        }

        public void StopMineAsync(string host, Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        StopMineRequest request = new StopMineRequest {
                            Timestamp = DateTime.Now
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:3336/api/MinerClient/StopMine", request);
                        Write.DevLine("StopMineAsync " + message.Result.ReasonPhrase);
                        callback?.Invoke();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke();
                }
            });
        }

        public void SetMinerProfilePropertyAsync(string host, string propertyName, object value, Action callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest {
                            PropertyName = propertyName,
                            Value = value,
                            Timestamp = DateTime.Now
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{host}:3336/api/MinerClient/SetMinerProfileProperty", request);
                        Write.DevLine("SetMinerProfilePropertyAsync " + message.Result.ReasonPhrase);
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
