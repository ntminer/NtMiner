using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api/AppSetting";
            private AppSettingServiceFace() { }
            
            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<GetAppSettingResponse> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.GetAsync($"{baseUrl}/AppSetting?messageId={Guid.NewGuid()}&key={key}");
                        GetAppSettingResponse response = message.Result.Content.ReadAsAsync<GetAppSettingResponse>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch {
                    callback?.Invoke(null);
                }
            }
            #endregion

            #region GetAppSettingsAsync
            public void GetAppSettingsAsync(Action<GetAppSettingsResponse> callback) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.GetAsync($"{baseUrl}/AppSettings?messageId={Guid.NewGuid()}");
                        GetAppSettingsResponse response = message.Result.Content.ReadAsAsync<GetAppSettingsResponse>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch {
                    callback?.Invoke(null);
                }
            }
            #endregion

            #region SetAppSettingAsync
            public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                try {
                    using (HttpClient client = new HttpClient()) {
                        SetAppSettingRequest request = new SetAppSettingRequest() {
                            MessageId = messageId,
                            Data = entity,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"{baseUrl}/SetAppSetting", request);
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        callback?.Invoke(response);
                    }
                }
                catch (Exception e) {
                    callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                }
            }
            #endregion
        }
    }
}
