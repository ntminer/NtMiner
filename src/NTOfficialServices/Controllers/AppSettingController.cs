using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class AppSettingController : ApiControllerBase, IReadonlyAppSettingController {
        public DateTime GetTime() {
            return DateTime.Now;
        }

        [HttpPost]
        public string GetJsonFileVersion(AppSettingRequest request) {
            string jsonVersion = string.Empty;
            string minerClientVersion = string.Empty;
            try {
                var fileData = HostRoot.Instance.NTMinerFileSet.LatestMinerClientFile;
                minerClientVersion = fileData != null ? fileData.Version : string.Empty;
                if (!HostRoot.Instance.AppSettingSet.TryGetAppSetting(request.Key, out IAppSetting data) || data.Value == null) {
                    jsonVersion = string.Empty;
                }
                else {
                    jsonVersion = data.Value.ToString();
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return $"{jsonVersion}|{minerClientVersion}";
        }
    }
}
