using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class FileUrlServiceFace {
            public static readonly FileUrlServiceFace Instance = new FileUrlServiceFace();

            private FileUrlServiceFace() { }

            #region GetNTMinerUrlAsync
            public void GetNTMinerUrlAsync(string fileName, Action<string> callback) {
                Task.Factory.StartNew(() => {
                    NTMinerUrlRequest request = new NTMinerUrlRequest {
                        FileName = fileName
                    };
                    string response = Request<string>("FileUrl", "NTMinerUrl", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region GetNTMinerFilesAsync
            public void GetNTMinerFilesAsync(Action<List<NTMinerFileData>> callback) {
                Task.Factory.StartNew(() => {
                    List<NTMinerFileData> response = Request<List<NTMinerFileData>>("FileUrl", "NTMinerFiles", null);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region AddOrUpdateNTMinerFileAsync
            public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    AddOrUpdateNTMinerFileRequest request = new AddOrUpdateNTMinerFileRequest() {
                        Data = entity,
                        LoginName = LoginName
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("FileUrl", "AddOrUpdateNTMinerFile", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region RemoveNTMinerFileAsync
            public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase> callback) {
                Task.Factory.StartNew(() => {
                    RemoveNTMinerFileRequest request = new RemoveNTMinerFileRequest {
                        LoginName = LoginName,
                        NTMinerId = id
                    };
                    request.SignIt(PasswordSha1);
                    ResponseBase response = Request<ResponseBase>("FileUrl", "RemoveNTMinerFile", request);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region GetLiteDBExplorerUrlAsync
            public void GetLiteDBExplorerUrlAsync(Action<string> callback) {
                Task.Factory.StartNew(() => {
                    string response = Request<string>("FileUrl", "LiteDBExplorerUrl", null);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region GetNTMinerUpdaterUrlAsync
            public void GetNTMinerUpdaterUrlAsync(Action<string> callback) {
                Task.Factory.StartNew(() => {
                    string response = Request<string>("FileUrl", "NTMinerUpdaterUrl", null);
                    callback?.Invoke(response);
                });
            }
            #endregion

            #region GetPackageUrlAsync
            public void GetPackageUrlAsync(string package, Action<string> callback) {
                Task.Factory.StartNew(() => {
                    PackageUrlRequest request = new PackageUrlRequest {
                        Package = package
                    };
                    string response = Request<string>("FileUrl", "PackageUrl", request);
                    callback?.Invoke(response);
                });
            }
            #endregion
        }
    }
}