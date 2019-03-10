using NTMiner.Controllers;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class FileUrlServiceFace {
            public static readonly FileUrlServiceFace Instance = new FileUrlServiceFace();
            private static readonly string SControllerName = ControllerUtil.GetControllerName<IFileUrlController>();

            private FileUrlServiceFace() { }

            #region GetNTMinerUrlAsync
            public void GetNTMinerUrlAsync(string fileName, Action<string, Exception> callback) {
                Task.Factory.StartNew(() => {
                    NTMinerUrlRequest request = new NTMinerUrlRequest {
                        FileName = fileName
                    };
                    RequestAsync(SControllerName, nameof(IFileUrlController.NTMinerUrl), request, callback);
                });
            }
            #endregion

            #region GetNTMinerFilesAsync
            public void GetNTMinerFilesAsync(Action<List<NTMinerFileData>, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RequestAsync(SControllerName, nameof(IFileUrlController.NTMinerFiles), null, callback);
                });
            }
            #endregion

            #region AddOrUpdateNTMinerFileAsync
            public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    DataRequest<NTMinerFileData> request = new DataRequest<NTMinerFileData>() {
                        Data = entity,
                        LoginName = SingleUser.LoginName
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(SControllerName, nameof(IFileUrlController.AddOrUpdateNTMinerFile), request, callback);
                });
            }
            #endregion

            #region RemoveNTMinerFileAsync
            public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase, Exception> callback) {
                Task.Factory.StartNew(() => {
                    DataRequest<Guid> request = new DataRequest<Guid>() {
                        LoginName = SingleUser.LoginName,
                        Data = id
                    };
                    request.SignIt(SingleUser.PasswordSha1);
                    RequestAsync(SControllerName, nameof(IFileUrlController.RemoveNTMinerFile), request, callback);
                });
            }
            #endregion

            #region GetLiteDbExplorerUrlAsync
            public void GetLiteDbExplorerUrlAsync(Action<string, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RequestAsync(SControllerName, nameof(IFileUrlController.LiteDbExplorerUrl), null, callback);
                });
            }
            #endregion

            #region GetNTMinerUpdaterUrlAsync
            public void GetNTMinerUpdaterUrlAsync(Action<string, Exception> callback) {
                Task.Factory.StartNew(() => {
                    RequestAsync(SControllerName, nameof(IFileUrlController.NTMinerUpdaterUrl), null, callback);
                });
            }
            #endregion

            #region GetPackageUrlAsync
            public void GetPackageUrlAsync(string package, Action<string, Exception> callback) {
                Task.Factory.StartNew(() => {
                    PackageUrlRequest request = new PackageUrlRequest {
                        Package = package
                    };
                    RequestAsync(SControllerName, nameof(IFileUrlController.PackageUrl), request, callback);
                });
            }
            #endregion
        }
    }
}