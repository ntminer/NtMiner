using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Services.Official {
    public class FileUrlService {
        private readonly string _controllerName = ControllerUtil.GetControllerName<IFileUrlController>();

        internal FileUrlService() {
        }

        #region GetNTMinerUrlAsync
        // ReSharper disable once InconsistentNaming
        public void GetNTMinerUrlAsync(string fileName, Action<string, Exception> callback) {
            NTMinerUrlRequest request = new NTMinerUrlRequest {
                FileName = fileName
            };
            RpcRoot.JsonRpc.PostAsync(
                _controllerName,
                nameof(IFileUrlController.NTMinerUrl),
                request,
                callback);
        }
        #endregion

        #region GetLiteDbExplorerUrlAsync
        public void GetLiteDbExplorerUrlAsync(Action<string, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName, 
                nameof(IFileUrlController.LiteDbExplorerUrl), 
                callback);
        }
        #endregion

        #region GetAtikmdagPatcherUrlAsync
        public void GetAtikmdagPatcherUrlAsync(Action<string, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName, 
                nameof(IFileUrlController.AtikmdagPatcherUrl), 
                callback);
        }
        #endregion

        #region GetSwitchRadeonGpuUrlAsync
        public void GetSwitchRadeonGpuUrlAsync(Action<string, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName, 
                nameof(IFileUrlController.SwitchRadeonGpuUrl), 
                callback);
        }
        #endregion

        #region GetNTMinerUpdaterUrlAsync
        // ReSharper disable once InconsistentNaming
        public void GetNTMinerUpdaterUrlAsync(Action<string, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName, 
                nameof(IFileUrlController.NTMinerUpdaterUrl), 
                callback, 
                timeountMilliseconds: 5000);
        }
        #endregion

        #region GetMinerClientFinderUrlAsync
        public void GetMinerClientFinderUrlAsync(Action<string, Exception> callback) {
            RpcRoot.JsonRpc.PostAsync(
                _controllerName, 
                nameof(IFileUrlController.MinerClientFinderUrl), 
                callback, 
                timeountMilliseconds: 5000);
        }
        #endregion

        #region GetPackageUrlAsync
        public void GetPackageUrlAsync(string package, Action<string, Exception> callback) {
            PackageUrlRequest request = new PackageUrlRequest {
                Package = package
            };
            RpcRoot.JsonRpc.PostAsync(
                _controllerName, 
                nameof(IFileUrlController.PackageUrl), 
                request, 
                callback);
        }
        #endregion
    }
}
