using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static partial class Server {
        public partial class FileUrlServiceFace {
            public static readonly FileUrlServiceFace Instance = new FileUrlServiceFace();

            private FileUrlServiceFace() { }

            #region GetNTMinerUrlAsync
            public void GetNTMinerUrlAsync(string fileName, Action<string> callback) {
                NTMinerUrlRequest request = new NTMinerUrlRequest {
                    FileName = fileName
                };
                string response = Request<string>("FileUrl", "NTMinerUrl", request);
                callback?.Invoke(response);
            }
            #endregion

            #region GetNTMinerFilesAsync
            public void GetNTMinerFilesAsync(Action<List<NTMinerFileData>> callback) {
                List<NTMinerFileData> response = Request<List<NTMinerFileData>>("FileUrl", "NTMinerFiles", null);
                callback?.Invoke(response);
            }
            #endregion

            #region AddOrUpdateNTMinerFileAsync
            public void AddOrUpdateNTMinerFileAsync(NTMinerFileData entity, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                AddOrUpdateNTMinerFileRequest request = new AddOrUpdateNTMinerFileRequest() {
                    MessageId = messageId,
                    Data = entity,
                    LoginName = LoginName,
                    Timestamp = DateTime.Now
                };
                request.SignIt(PasswordSha1);
                ResponseBase response = Request<ResponseBase>("FileUrl", "AddOrUpdateNTMinerFile", request);
                callback?.Invoke(response);
            }
            #endregion

            #region RemoveNTMinerFileAsync
            public void RemoveNTMinerFileAsync(Guid id, Action<ResponseBase> callback) {
                Guid messageId = Guid.NewGuid();
                RemoveNTMinerFileRequest request = new RemoveNTMinerFileRequest {
                    MessageId = messageId,
                    LoginName = LoginName,
                    NTMinerId = id,
                    Timestamp = DateTime.Now
                };
                request.SignIt(PasswordSha1);
                ResponseBase response = Request<ResponseBase>("FileUrl", "RemoveNTMinerFile", request);
                callback?.Invoke(response);
            }
            #endregion

            #region GetLiteDBExplorerUrlAsync
            public void GetLiteDBExplorerUrlAsync(Action<string> callback) {
                string response = Request<string>("FileUrl", "LiteDBExplorerUrl", null);
                callback?.Invoke(response);
            }
            #endregion

            #region GetNTMinerUpdaterUrlAsync
            public void GetNTMinerUpdaterUrlAsync(Action<string> callback) {
                string response = Request<string>("FileUrl", "NTMinerUpdaterUrl", null);
                callback?.Invoke(response);
            }
            #endregion

            #region GetPackageUrlAsync
            public void GetPackageUrlAsync(string package, Action<string> callback) {
                PackageUrlRequest request = new PackageUrlRequest {
                    Package = package
                };
                string response = Request<string>("FileUrl", "PackageUrl", request);
                callback?.Invoke(response);
            }
            #endregion
        }
    }
}