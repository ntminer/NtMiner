using NTMiner.MinerClient;
using NTMiner.Hashrate;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class MinerClientService {
        public static readonly MinerClientService Instance = new MinerClientService();

        private MinerClientService() {
        }

        public void AddUserAsync(string clientHost, string loginName, string password, string description, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        AddUserRequest request = new AddUserRequest() {
                            LoginName = loginName,
                            Password = password,
                            Description = description
                        };
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/AddUser", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch {
                    callback?.Invoke(null);
                }
            });
        }

        public void ShowMainWindowAsync(string clientHost, Action<bool> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:3336/api/MinerClient/ShowMainWindow", null);
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

        public void StartMineAsync(string clientHost, Guid workId, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        StartMineRequest request = new StartMineRequest() {
                            LoginName = Server.LoginName,
                            WorkId = workId
                        };
                        request.SignIt(Server.PasswordSha1Sha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/StartMine", request);
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

        public void StopMineAsync(string clientHost, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        StopMineRequest request = new StopMineRequest() {
                            LoginName = Server.LoginName
                        };
                        request.SignIt(Server.PasswordSha1Sha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/StopMine", request);
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

        public void SetMinerProfilePropertyAsync(string clientHost, string propertyName, object value, Action<ResponseBase> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        SetMinerProfilePropertyRequest request = new SetMinerProfilePropertyRequest {
                            LoginName = Server.LoginName,
                            PropertyName = propertyName,
                            Value = value
                        };
                        request.SignIt(Server.PasswordSha1Sha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{clientHost}:3336/api/MinerClient/SetMinerProfileProperty", request);
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

        public void GetProfile(string clientHost, Action<ProfileData> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:3336/api/MinerClient/GetProfile", null);
                        ProfileData data = message.Result.Content.ReadAsAsync<ProfileData>().Result;
                        callback?.Invoke(data);
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null);
                }
            });
        }

        public void GetSpeed(string clientHost, Action<SpeedData> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsync($"http://{clientHost}:3336/api/MinerClient/GetSpeed", null);
                        SpeedData data = message.Result.Content.ReadAsAsync<SpeedData>().Result;
                        callback?.Invoke(data);
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
