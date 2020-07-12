using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Services.Official {
    public class FileUrlService {
        private readonly string _controllerName = RpcRoot.GetControllerName<IFileUrlController>();

        public FileUrlService() {
        }

        #region GetNTMinerUrlAsync
        // ReSharper disable once InconsistentNaming
        public void GetNTMinerUrlAsync(string fileName, Action<string, Exception> callback) {
            NTMinerUrlRequest request = new NTMinerUrlRequest {
                FileName = fileName
            };
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.NTMinerUrl), request, callback);
        }
        #endregion

        #region GetNTMinerFilesAsync
        // ReSharper disable once InconsistentNaming
        public void GetNTMinerFilesAsync(NTMinerAppType appType, Action<List<NTMinerFileData>> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.NTMinerFiles), callback: (List<NTMinerFileData> data, Exception e) => {
                if (data != null) {
                    data = data.Where(a => a.AppType == appType).ToList();
                }
                else {
                    data = new List<NTMinerFileData>();
                }
                callback?.Invoke(data);
            });
        }
        #endregion

        #region AddOrUpdateNTMinerFileAsync
        // ReSharper disable once InconsistentNaming
        public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase, Exception> callback) {
            DataRequest<NTMinerFileData> request = new DataRequest<NTMinerFileData>() {
                Data = entity
            };
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.AddOrUpdateNTMinerFile), data: request, callback);
        }
        #endregion

        #region RemoveNTMinerFileAsync
        // ReSharper disable once InconsistentNaming
        public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase, Exception> callback) {
            DataRequest<Guid> request = new DataRequest<Guid>() {
                Data = id
            };
            JsonRpcRoot.SignPostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.RemoveNTMinerFile), data: request, callback);
        }
        #endregion

        #region GetLiteDbExplorerUrlAsync
        public void GetLiteDbExplorerUrlAsync(Action<string, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.LiteDbExplorerUrl), callback);
        }
        #endregion

        #region GetAtikmdagPatcherUrlAsync
        public void GetAtikmdagPatcherUrlAsync(Action<string, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.AtikmdagPatcherUrl), callback);
        }
        #endregion

        #region GetSwitchRadeonGpuUrlAsync
        public void GetSwitchRadeonGpuUrlAsync(Action<string, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.SwitchRadeonGpuUrl), callback);
        }
        #endregion

        #region GetNTMinerUpdaterUrlAsync
        // ReSharper disable once InconsistentNaming
        public void GetNTMinerUpdaterUrlAsync(Action<string, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.NTMinerUpdaterUrl), callback, timeountMilliseconds: 5000);
        }
        #endregion

        #region GetMinerClientFinderUrlAsync
        public void GetMinerClientFinderUrlAsync(Action<string, Exception> callback) {
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.MinerClientFinderUrl), callback, timeountMilliseconds: 5000);
        }
        #endregion

        #region GetPackageUrlAsync
        public void GetPackageUrlAsync(string package, Action<string, Exception> callback) {
            PackageUrlRequest request = new PackageUrlRequest {
                Package = package
            };
            JsonRpcRoot.PostAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, _controllerName, nameof(IFileUrlController.PackageUrl), request, callback);
        }
        #endregion
    }
}
