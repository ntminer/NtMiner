using Aliyun.OSS;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class FileUrlController : ApiController, IFileUrlController {
        private string ClientIp {
            get {
                return Request.GetWebClientIp();
            }
        }

        [HttpPost]
        public string MinerJsonPutUrl([FromBody]MinerJsonPutUrlRequest request) {
            if (request == null || string.IsNullOrEmpty(request.FileName)) {
                return string.Empty;
            }
            try {
                var req = new GeneratePresignedUriRequest("minerjson", request.FileName, SignHttpMethod.Put);
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
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
            var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
            return uri.ToString();
        }

        [HttpPost]
        public List<NTMinerFileData> NTMinerFiles() {
            var list = HostRoot.Current.NTMinerFileSet.GetAll();
            return list;
        }

        [HttpPost]
        public ResponseBase AddOrUpdateNTMinerFile([FromBody]DataRequest<NTMinerFileData> request) {
            if (request == null || request.Data == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Current.NTMinerFileSet.AddOrUpdate(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public ResponseBase RemoveNTMinerFile([FromBody]DataRequest<Guid> request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            try {
                if (!request.IsValid(HostRoot.Current.UserSet.GetUser, ClientIp, out ResponseBase response)) {
                    return response;
                }
                HostRoot.Current.NTMinerFileSet.Remove(request.Data);
                return ResponseBase.Ok();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return ResponseBase.ServerError(e.Message);
            }
        }

        [HttpPost]
        public string NTMinerUpdaterUrl() {
            try {
                string ntminerUpdaterFileName;
                if (!HostRoot.Current.AppSettingSet.TryGetAppSetting("ntminerUpdaterFileName", out IAppSetting ntminerUpdaterFileNameSetting)) {
                    ntminerUpdaterFileName = "NTMinerUpdater.exe";
                }
                else {
                    ntminerUpdaterFileName = (string)ntminerUpdaterFileNameSetting.Value;
                }
                var req = new GeneratePresignedUriRequest("ntminer", ntminerUpdaterFileName, SignHttpMethod.Get);
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }

        [HttpPost]
        public string LiteDbExplorerUrl() {
            try {
                var req = new GeneratePresignedUriRequest("ntminer", "LiteDBExplorerPortable.zip", SignHttpMethod.Get);
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
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
                var uri = HostRoot.Current.OssClient.GeneratePresignedUri(req);
                return uri.ToString();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }
    }
}
