using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static class OfficialServer {
        public static readonly FileUrlServiceFace FileUrlService = FileUrlServiceFace.Instance;
        public static readonly OverClockDataServiceFace OverClockDataService = OverClockDataServiceFace.Instance;

        #region private methods
        private static void PostAsync<T>(string controller, string action, object param, Action<T, Exception> callback, int timeountMilliseconds = 0) where T : class {
            Task.Factory.StartNew(() => {
                try {
                    using (HttpClient client = new HttpClient()) {
                        if (timeountMilliseconds != 0) {
                            client.Timeout = TimeSpan.FromMilliseconds(timeountMilliseconds);
                        }
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
            });
        }

        #region GetCalcConfigs
        public static void GetCalcConfigsAsync(Action<List<CalcConfigData>> callback) {
            Task.Factory.StartNew(() => {
                try {
                    CalcConfigsRequest request = new CalcConfigsRequest {
                    };
                    PostAsync("ControlCenter", nameof(IControlCenterController.CalcConfigs), request, (DataResponse<List<CalcConfigData>> response, Exception e) => {
                        if (response.IsSuccess()) {
                            callback?.Invoke(response.Data);
                        }
                        else {
                            callback?.Invoke(new List<CalcConfigData>());
                        }
                    }, timeountMilliseconds: 10 * 1000);
                }
                catch (Exception e) {
                    callback?.Invoke(new List<CalcConfigData>());
                }
            });
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

        public static void GetJsonFileVersionAsync(string key, Action<string, string> callback) {
            AppSettingRequest request = new AppSettingRequest {
                Key = key
            };
            PostAsync("AppSetting", nameof(IAppSettingController.GetJsonFileVersion), request, (string text, Exception e) => {
                string jsonFileVersion = string.Empty;
                string minerClientVersion = string.Empty;
                if (!string.IsNullOrEmpty(text)) {
                    string[] parts = text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0) {
                        jsonFileVersion = parts[0];
                    }
                    if (parts.Length > 1) {
                        minerClientVersion = parts[1];
                    }
                }
                callback?.Invoke(jsonFileVersion, minerClientVersion);
            }, timeountMilliseconds: 10 * 1000);
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
                PostAsync<List<NTMinerFileData>>(SControllerName, nameof(IFileUrlController.NTMinerFiles), null, callback: (data, e) => {
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
                PostAsync(SControllerName, nameof(IFileUrlController.NTMinerUpdaterUrl), null, callback, timeountMilliseconds: 2000);
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
            public void GetOverClockDatasAsync(Action<DataResponse<List<OverClockData>>, Exception> callback) {
                try {
                    OverClockDatasRequest request = new OverClockDatasRequest {
                    };
                    PostAsync(SControllerName, nameof(IOverClockDataController.OverClockDatas), request, callback);
                }
                catch (Exception e) {
                    e = e.GetInnerException();
                    Logger.ErrorDebugLine(e.Message, e);
                    callback?.Invoke(null, e);
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