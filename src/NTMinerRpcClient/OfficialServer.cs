using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static class OfficialServer {
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly OverClockDataServiceFace OverClockDataService = OverClockDataServiceFace.Instance;

        #region private methods
        private static void PostAsync<T>(string controller, string action, object param, Action<T, Exception> callback) where T : class {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message =
                            client.PostAsJsonAsync($"http://{AssemblyInfo.OfficialServerHost}:{WebApiConst.ControlCenterPort}/api/{controller}/{action}", param);
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    callback?.Invoke(null, e);
                }
            });
        }

        private static T Post<T>(string controller, string action, object param) where T : class {
            try {
                using (HttpClient client = new HttpClient()) {
                    Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://{AssemblyInfo.OfficialServerHost}:{WebApiConst.ControlCenterPort}/api/{controller}/{action}", param);
                    T response = message.Result.Content.ReadAsAsync<T>().Result;
                    return response;
                }
            }
            catch {
                return null;
            }
        }

        private static void GetAsync<T>(string controller, string action, Dictionary<string, string> param, Action<T, Exception> callback) {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        string queryString = string.Empty;
                        if (param != null && param.Count != 0) {
                            queryString = "?" + string.Join("&", param.Select(a => a.Key + "=" + a.Value));
                        }

                        Task<HttpResponseMessage> message =
                            client.GetAsync($"http://{AssemblyInfo.OfficialServerHost}:{WebApiConst.ControlCenterPort}/api/{controller}/{action}{queryString}");
                        T response = message.Result.Content.ReadAsAsync<T>().Result;
                        callback?.Invoke(response, null);
                    }
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    callback?.Invoke(default(T), e);
                }
            });
        }
#endregion

        public static void GetTimeAsync(Action<DateTime> callback) {
            GetAsync("AppSetting", nameof(IAppSettingController.GetTime), null, callback: (DateTime datetime, Exception e) => {
                callback?.Invoke(datetime);
                if (e != null) {
                    Logger.ErrorDebugLine($"GetTimeAsync失败 {e?.Message}");
                }
            });
        }

#region GetCalcConfigs
        /// <summary>
        /// 同步方法
        /// </summary>
        /// <returns></returns>
        public static DataResponse<List<CalcConfigData>> GetCalcConfigs() {
            try {
                CalcConfigsRequest request = new CalcConfigsRequest {
                    MessageId = Guid.NewGuid()
                };
                DataResponse<List<CalcConfigData>> response = Post<DataResponse<List<CalcConfigData>>>("ControlCenter", nameof(IControlCenterController.CalcConfigs), request);
                return response;
            }
            catch (Exception e) {
                e = e.GetInnerException();
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
#endregion

#region SaveCalcConfigsAsync
        public static void SaveCalcConfigsAsync(List<CalcConfigData> configs, Action<ResponseBase, Exception> callback) {
            if (configs == null || configs.Count == 0) {
                return;
            }
            SaveCalcConfigsRequest request = new SaveCalcConfigsRequest {
                Data = configs,
                LoginName = SingleUser.LoginName
            };
            request.SignIt(SingleUser.PasswordSha1);
            PostAsync("ControlCenter", nameof(IControlCenterController.SaveCalcConfigs), request, callback);
        }
#endregion

        public static void GetJsonFileVersionAsync(string key, Action<string> callback) {
            AppSettingRequest request = new AppSettingRequest {
                MessageId = Guid.NewGuid(),
                Key = key
            };
            PostAsync("AppSetting", nameof(IAppSettingController.AppSetting), request, (DataResponse<AppSettingData> response, Exception e) => {
                string jsonFileVersion = string.Empty;
                if (response != null && response.Data != null && response.Data.Value != null) {
                    jsonFileVersion = response.Data.Value.ToString();
                }
                callback?.Invoke(jsonFileVersion);
                if (e != null) {
                    Logger.ErrorDebugLine($"GetJsonFileVersionAsync({AssemblyInfo.ServerJsonFileName})失败 {e?.Message}");
                }
            });
        }

        public class FileUrlServiceFace {
            public static readonly FileUrlServiceFace Instance = new FileUrlServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IFileUrlController>();

            private FileUrlServiceFace() { }

#region GetNTMinerUrlAsync
            // ReSharper disable once InconsistentNaming
            public void GetNTMinerUrlAsync(string fileName, Action<string, Exception> callback) {
                NTMinerUrlRequest request = new NTMinerUrlRequest {
                    FileName = fileName
                };
                PostAsync(SControllerName, nameof(IFileUrlController.NTMinerUrl), request, callback);
            }
#endregion

#region GetNTMinerFilesAsync
            // ReSharper disable once InconsistentNaming
            public void GetNTMinerFilesAsync(NTMinerAppType appType, Action<List<NTMinerFileData>, Exception> callback) {
                PostAsync<List<NTMinerFileData>>(SControllerName, nameof(IFileUrlController.NTMinerFiles), null, callback: (data, e)=> {
                    if (data != null) {
                        data = data.Where(a => a.AppType == appType).ToList();
                    }
                    callback?.Invoke(data, e);
                });
            }
#endregion

#region AddOrUpdateNTMinerFileAsync
            // ReSharper disable once InconsistentNaming
            public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<NTMinerFileData> request = new DataRequest<NTMinerFileData>() {
                    Data = entity,
                    LoginName = SingleUser.LoginName
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IFileUrlController.AddOrUpdateNTMinerFile), request, callback);
            }
#endregion

#region RemoveNTMinerFileAsync
            // ReSharper disable once InconsistentNaming
            public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IFileUrlController.RemoveNTMinerFile), request, callback);
            }
#endregion

#region GetLiteDbExplorerUrlAsync
            public void GetLiteDbExplorerUrlAsync(Action<string, Exception> callback) {
                PostAsync(SControllerName, nameof(IFileUrlController.LiteDbExplorerUrl), null, callback);
            }
#endregion

#region GetNTMinerUpdaterUrlAsync
            // ReSharper disable once InconsistentNaming
            public void GetNTMinerUpdaterUrlAsync(Action<string, Exception> callback) {
                PostAsync(SControllerName, nameof(IFileUrlController.NTMinerUpdaterUrl), null, callback);
            }
#endregion

#region GetPackageUrlAsync
            public void GetPackageUrlAsync(string package, Action<string, Exception> callback) {
                PackageUrlRequest request = new PackageUrlRequest {
                    Package = package
                };
                PostAsync(SControllerName, nameof(IFileUrlController.PackageUrl), request, callback);
            }
#endregion
        }

        public class OverClockDataServiceFace {
            public static readonly OverClockDataServiceFace Instance = new OverClockDataServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IOverClockDataController>();

            private OverClockDataServiceFace() { }

#region GetOverClockDatas
            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="messageId"></param>
            /// <returns></returns>
            public List<OverClockData> GetOverClockDatas(Guid messageId) {
                try {
                    OverClockDatasRequest request = new OverClockDatasRequest {
                        MessageId = Guid.NewGuid()
                    };
                    DataResponse<List<OverClockData>> response = Post<DataResponse<List<OverClockData>>>(SControllerName, nameof(IOverClockDataController.OverClockDatas), request);
                    if (response != null && response.Data != null) {
                        return response.Data;
                    }
                    return new List<OverClockData>();
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    return new List<OverClockData>();
                }
            }
#endregion

#region AddOrUpdateOverClockDataAsync
            public void AddOrUpdateOverClockDataAsync(OverClockData entity, Action<ResponseBase, Exception> callback) {
                DataRequest<OverClockData> request = new DataRequest<OverClockData>() {
                    LoginName = SingleUser.LoginName,
                    Data = entity
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IOverClockDataController.AddOrUpdateOverClockData), request, callback);
            }
#endregion

#region RemoveOverClockDataAsync
            public void RemoveOverClockDataAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    LoginName = SingleUser.LoginName,
                    Data = id
                };
                request.SignIt(SingleUser.PasswordSha1);
                PostAsync(SControllerName, nameof(IOverClockDataController.RemoveOverClockData), request, callback);
            }
#endregion
        }
    }
}