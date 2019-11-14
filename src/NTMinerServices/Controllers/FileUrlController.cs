using Aliyun.OSS;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class FileUrlController : ApiControllerBase, IFileUrlController {
        [HttpPost]
        public string MinerJsonPutUrl([FromBody]MinerJsonPutUrlRequest request) {
            if (request == null || string.IsNullOrEmpty(request.FileName)) {
                return string.Empty;
            }
            try {
                var req = new GeneratePresignedUriRequest("minerjson", request.FileName, SignHttpMethod.Put);
                var uri = HostRoot.Instance.OssClient.GeneratePresignedUri(req);
                return OfficialServer.SignatureSafeUrl(uri);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [HttpPost]
        public string NTMinerUrl([FromBody]NTMinerUrlRequest request) {
            if (request == null || string.IsNullOrEmpty(request.FileName)) {
                return string.Empty;
            }
            var req = new GeneratePresignedUriRequest("ntminer", request.FileName, SignHttpMethod.Get) {
                Expiration = DateTime.Now.AddMinutes(10)
            };
            var uri = HostRoot.Instance.OssClient.GeneratePresignedUri(req);
            return OfficialServer.SignatureSafeUrl(uri);
        }

        [HttpPost]
        public List<NTMinerFileData> NTMinerFiles() {
            var list = HostRoot.Instance.NTMinerFileSet.GetAll();
            return list;
        }

        [HttpPost]
        public ResponseBase AddOrUpdateNTMinerFile([FromBody]DataRequest<NTMinerFileData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.NTMinerFileSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase RemoveNTMinerFile([FromBody]DataRequest<Guid> request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(User, Sign, Timestamp, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Instance.NTMinerFileSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public string NTMinerUpdaterUrl() {
            try {
                string ntminerUpdaterFileName;
                if (!VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.NTMinerUpdaterFileNameAppSettingKey, out IAppSetting ntminerUpdaterFileNameSetting)) {
                    ntminerUpdaterFileName = NTKeyword.NTMinerUpdaterFileName;
                }
                else {
                    ntminerUpdaterFileName = (string)ntminerUpdaterFileNameSetting.Value;
                }
                var req = new GeneratePresignedUriRequest("ntminer", ntminerUpdaterFileName, SignHttpMethod.Get);
                var uri = HostRoot.Instance.OssClient.GeneratePresignedUri(req);
                return OfficialServer.SignatureSafeUrl(uri);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [HttpPost]
        public string LiteDbExplorerUrl() {
            try {
                var req = new GeneratePresignedUriRequest("ntminer", "LiteDBExplorerPortable.zip", SignHttpMethod.Get);
                var uri = HostRoot.Instance.OssClient.GeneratePresignedUri(req);
                return OfficialServer.SignatureSafeUrl(uri);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [HttpPost]
        public string PackageUrl([FromBody]PackageUrlRequest request) {
            try {
                if (request == null || string.IsNullOrEmpty(request.Package)) {
                    return string.Empty;
                }
                var req = new GeneratePresignedUriRequest("ntminer", $"packages/{request.Package}", SignHttpMethod.Get) {
                    Expiration = DateTime.Now.AddMinutes(10)
                };
                var uri = HostRoot.Instance.OssClient.GeneratePresignedUri(req);
                return OfficialServer.SignatureSafeUrl(uri);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }
    }
}
