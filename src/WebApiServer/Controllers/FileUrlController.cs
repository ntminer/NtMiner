using NTMiner.Core.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    // 注意该控制器不能重命名
    public class FileUrlController : ApiControllerBase, IFileUrlController {
        [Role.Public]
        [HttpPost]
        public string NTMinerUrl([FromBody]NTMinerUrlRequest request) {
            if (request == null || string.IsNullOrEmpty(request.FileName)) {
                return string.Empty;
            }
            return AppRoot.NTMinerFileUrlGenerater.GeneratePresignedUrl("ntminer", request.FileName);
        }

        [Role.Public]
        [HttpPost]
        public string NTMinerUpdaterUrl() {
            try {
                string fileName;
                if (!VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.NTMinerUpdaterFileNameAppSettingKey, out IAppSetting setting)) {
                    fileName = NTKeyword.NTMinerUpdaterFileName;
                }
                else {
                    fileName = (string)setting.Value;
                }
                return AppRoot.NTMinerFileUrlGenerater.GeneratePresignedUrl("ntminer", $"tools/{fileName}");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [Role.Public]
        [HttpPost]
        public string MinerClientFinderUrl() {
            try {
                string fileName;
                if (!VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.MinerClientFinderFileNameAppSettingKey, out IAppSetting setting)) {
                    fileName = NTKeyword.MinerClientFinderFileName;
                }
                else {
                    fileName = (string)setting.Value;
                }
                return AppRoot.NTMinerFileUrlGenerater.GeneratePresignedUrl("ntminer", $"tools/{fileName}");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [Role.Public]
        [HttpPost]
        public string LiteDbExplorerUrl() {
            try {
                return AppRoot.NTMinerFileUrlGenerater.GeneratePresignedUrl("ntminer", "tools/LiteDBExplorerPortable.zip");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [Role.Public]
        [HttpPost]
        public string AtikmdagPatcherUrl() {
            try {
                string fileName;
                if (!VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.MinerClientFinderFileNameAppSettingKey, out IAppSetting setting)) {
                    fileName = NTKeyword.AtikmdagPatcherFileName;
                }
                else {
                    fileName = (string)setting.Value;
                }
                return AppRoot.NTMinerFileUrlGenerater.GeneratePresignedUrl("ntminer", $"tools/{fileName}");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [Role.Public]
        [HttpPost]
        public string SwitchRadeonGpuUrl() {
            try {
                string fileName;
                if (!VirtualRoot.LocalAppSettingSet.TryGetAppSetting(NTKeyword.MinerClientFinderFileNameAppSettingKey, out IAppSetting setting)) {
                    fileName = NTKeyword.SwitchRadeonGpuFileName;
                }
                else {
                    fileName = (string)setting.Value;
                }
                return AppRoot.NTMinerFileUrlGenerater.GeneratePresignedUrl("ntminer", $"tools/{fileName}");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        [Role.Public]
        [HttpPost]
        public string ToolFileUrl(string fileCode) {
            // TODO:fileCode是白名单中的项，如果这个白名单是可以管理维护的，则通过这个通用的Action可以避免硬编码的Action们
            // 这个fileCode应包含相对目录，形如packages/Claymore15.0.zip和tools/SwitchRadeonGpu.exe
            throw new NotImplementedException();
        }

        [Role.Public]
        [HttpPost]
        public string PackageUrl([FromBody]PackageUrlRequest request) {
            try {
                if (request == null || string.IsNullOrEmpty(request.Package)) {
                    return string.Empty;
                }
                return AppRoot.NTMinerFileUrlGenerater.GeneratePresignedUrl("ntminer", $"packages/{request.Package}");
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }
    }
}
