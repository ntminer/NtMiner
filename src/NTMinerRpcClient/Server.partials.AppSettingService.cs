using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public partial class Server {
        public partial class AppSettingServiceFace {
            public static readonly AppSettingServiceFace Instance = new AppSettingServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IAppSettingController>();

            private AppSettingServiceFace() { }

            public void GetTimeAsync(Action<DateTime, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.GetAsync($"http://{MinerServerHost}:{WebApiConst.MinerServerPort}/api/{SControllerName}/{nameof(IAppSettingController.GetTime)}");
                            DateTime response = message.Result.Content.ReadAsAsync<DateTime>().Result;
                            callback?.Invoke(response, null);
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(DateTime.Now, e);
                    }
                });
            }

            #region GetAppSettingAsync
            public void GetAppSettingAsync(string key, Action<DataResponse<AppSettingData>, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        AppSettingRequest request = new AppSettingRequest {
                            MessageId = Guid.NewGuid(),
                            Key = key
                        };
                        DataResponse<AppSettingData> response = Request<DataResponse<AppSettingData>>(SControllerName, nameof(IAppSettingController.AppSetting), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion

            #region GetAppSettingsAsync
            public List<AppSettingData> GetAppSettings() {
                try {
                    AppSettingsRequest request = new AppSettingsRequest {
                        MessageId = Guid.NewGuid()
                    };
                    DataResponse<List<AppSettingData>> response = Request<DataResponse<List<AppSettingData>>>(SControllerName, nameof(IAppSettingController.AppSettings), request);
                    return response.Data;
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<AppSettingData>();
                }
            }
            #endregion

            #region SetAppSettingAsync
            public void SetAppSettingAsync(AppSettingData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        DataRequest<AppSettingData> request = new DataRequest<AppSettingData>() {
                            Data = entity,
                            LoginName = SingleUser.LoginName
                        };
                        request.SignIt(SingleUser.PasswordSha1);
                        ResponseBase response = Request<ResponseBase>(SControllerName, nameof(IAppSettingController.SetAppSetting), request);
                        callback?.Invoke(response, null);
                    }
                    catch (Exception e) {
                        callback?.Invoke(null, e);
                    }
                });
            }
            #endregion
        }
    }
}
