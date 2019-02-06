using NTMiner;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();

            private AppSettingServiceFace() { }

            private IAppSettingService CreateService() {
                return new EmptyAppSettingService();
            }

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<GetAppSettingResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var client = CreateService()) {
                            var response = client.GetAppSetting(Guid.NewGuid(), key);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region GetAppSettingsAsync
            public void GetAppSettingsAsync(Action<GetAppSettingsResponse> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var client = CreateService()) {
                            var response = client.GetAppSettings(Guid.NewGuid());
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(null);
                    }
                });
            }
            #endregion

            #region SetAppSettingAsync
            public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    Guid messageId = Guid.NewGuid();
                    try {
                        SetAppSettingRequest request = new SetAppSettingRequest() {
                            MessageId = messageId,
                            Data = entity,
                            LoginName = LoginName,
                            Timestamp = DateTime.Now
                        };
                        request.SignIt(PasswordSha1);
                        using (var service = CreateService()) {
                            ResponseBase response = service.SetAppSetting(request);
                            callback?.Invoke(response);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(ResponseBase.ClientError(messageId, e.Message));
                    }
                });
            }
            #endregion
        }

        public class EmptyAppSettingService : IAppSettingService {
            public void Dispose() {
                
            }

            public GetAppSettingResponse GetAppSetting(Guid messageId, string key) {
                return null;
            }

            public GetAppSettingsResponse GetAppSettings(Guid messageId) {
                return null;
            }

            public ResponseBase SetAppSetting(SetAppSettingRequest request) {
                return null;
            }
        }
    }
}
