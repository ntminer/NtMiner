using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class OfficialServer {
        public class FileUrlServiceFace {
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IFileUrlController>();

            private readonly string _host;
            private readonly int _port;

            public FileUrlServiceFace(string host, int port) {
                _host = host;
                _port = port;
            }

            #region GetNTMinerUrlAsync
            // ReSharper disable once InconsistentNaming
            public void GetNTMinerUrlAsync(string fileName, Action<string, Exception> callback) {
                NTMinerUrlRequest request = new NTMinerUrlRequest {
                    FileName = fileName
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.NTMinerUrl), request, callback);
            }
            #endregion

            #region GetNTMinerFilesAsync
            // ReSharper disable once InconsistentNaming
            public void GetNTMinerFilesAsync(NTMinerAppType appType, Action<List<NTMinerFileData>, Exception> callback) {
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.NTMinerFiles), callback: (List<NTMinerFileData> data, Exception e) => {
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
                    Data = entity
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.AddOrUpdateNTMinerFile), request, request, callback);
            }
            #endregion

            #region RemoveNTMinerFileAsync
            // ReSharper disable once InconsistentNaming
            public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase, Exception> callback) {
                DataRequest<Guid> request = new DataRequest<Guid>() {
                    Data = id
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.RemoveNTMinerFile), request, request, callback);
            }
            #endregion

            #region GetLiteDbExplorerUrlAsync
            public void GetLiteDbExplorerUrlAsync(Action<string, Exception> callback) {
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.LiteDbExplorerUrl), callback);
            }
            #endregion

            #region GetNTMinerUpdaterUrlAsync
            // ReSharper disable once InconsistentNaming
            public void GetNTMinerUpdaterUrlAsync(Action<string, Exception> callback) {
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.NTMinerUpdaterUrl), callback, timeountMilliseconds: 2000);
            }
            #endregion

            #region GetMinerClientFinderUrlAsync
            public void GetMinerClientFinderUrlAsync(Action<string, Exception> callback) {
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.MinerClientFinderUrl), callback, timeountMilliseconds: 2000);
            }
            #endregion

            #region GetPackageUrlAsync
            public void GetPackageUrlAsync(string package, Action<string, Exception> callback) {
                PackageUrlRequest request = new PackageUrlRequest {
                    Package = package
                };
                RpcRoot.PostAsync(_host, _port, SControllerName, nameof(IFileUrlController.PackageUrl), request, callback);
            }
            #endregion
        }
    }
}
