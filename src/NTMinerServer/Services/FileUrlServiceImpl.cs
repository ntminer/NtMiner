using Aliyun.OSS;
using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.Services {
    public class FileUrlServiceImpl : IFileUrlService {
        public string GetMinerJsonPutUrl(string fileName) {
            try {
                var req = new GeneratePresignedUriRequest("minerjson", fileName, SignHttpMethod.Put);
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }

        public string GetNTMinerUrl(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                return string.Empty;
            }
            var req = new GeneratePresignedUriRequest("ntminer", fileName, SignHttpMethod.Get) {
                Expiration = DateTime.Now.AddMinutes(10)
            };
            var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
            return uri.ToString();
        }

        public List<NTMinerFileData> GetNTMinerFiles() {
            var list = HostRoot.Current.NTMinerFileSet.GetNTMinerFiles();
            return list;
        }

        public ResponseBase AddOrUpdateNTMinerFile(AddOrUpdateNTMinerFileRequest request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                ResponseBase response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.NTMinerFileSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        public ResponseBase RemoveNTMinerFile(RemoveNTMinerFileRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput(Guid.Empty, "参数错误");
            }
            try {
                LoadClientsResponse response;
                if (!request.IsValid(HostRoot.Current.UserSet, out response)) {
                    return response;
                }
                HostRoot.Current.NTMinerFileSet.Remove(request.NTMinerId);
                return ResponseBase.Ok(request.MessageId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(request.MessageId, e.Message);
            }
        }

        public string GetNTMinerUpdaterUrl() {
            try {
                var req = new GeneratePresignedUriRequest("ntminer", "NTMinerUpdater.exe", SignHttpMethod.Get);
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }

        public string GetLiteDBExplorerUrl() {
            try {
                var req = new GeneratePresignedUriRequest("ntminer", "LiteDBExplorerPortable.zip", SignHttpMethod.Get);
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }

        public string GetPackageUrl(string package) {
            try {
                if (string.IsNullOrEmpty(package)) {
                    return string.Empty;
                }
                var req = new GeneratePresignedUriRequest("ntminer", $"packages/{package}", SignHttpMethod.Get) {
                    Expiration = DateTime.Now.AddMinutes(10)
                };
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }

        public void Dispose() {
        }
    }
}
