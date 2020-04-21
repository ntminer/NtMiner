using NTMiner.Controllers;
using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.VirtualMemory;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NTMiner {
    /// <summary>
    /// 端口号：<see cref="NTKeyword.NTMinerDaemonPort"/>
    /// </summary>
    public class NTMinerDaemonController : ApiController, INTMinerDaemonController {
        [HttpPost]
        public DataResponse<IntPtr> ShowConsole() {
            if (!Write.IsEnabled) {
                Write.Enable();
                Write.UserOk("打开控制台窗口");
                Console.Title = "开源矿工守护程序";
            }
            IntPtr intPtr = NTMinerConsole.GetOrAlloc();
            return DataResponse<IntPtr>.Ok(intPtr);
        }

        [HttpGet]
        [HttpPost]
        public WsClientState GetWsDaemonState() {
            return VirtualRoot.DaemonWsClient.GetState();
        }

        [HttpPost]
        public ResponseBase EnableRemoteDesktop() {
            return VirtualRoot.DaemonOperation.EnableRemoteDesktop();
        }

        [HttpPost]
        public ResponseBase BlockWAU() {
            return VirtualRoot.DaemonOperation.BlockWAU();
        }

        [HttpPost]
        public ResponseBase AtikmdagPatcher() {
            return VirtualRoot.DaemonOperation.AtikmdagPatcher();
        }

        [HttpPost]
        public ResponseBase SwitchRadeonGpu(bool on) {
            return VirtualRoot.DaemonOperation.SwitchRadeonGpu(on);
        }

        [HttpPost]
        public void CloseDaemon() {
            VirtualRoot.DaemonOperation.CloseDaemon();
        }

        #region GetGpuProfilesJson
        [HttpPost]
        public string GetGpuProfilesJson() {
            return VirtualRoot.DaemonOperation.GetGpuProfilesJson();
        }
        #endregion

        #region SaveGpuProfilesJson
        [HttpPost]
        public void SaveGpuProfilesJson() {
            string json = Request.Content.ReadAsStringAsync().Result;
            VirtualRoot.DaemonOperation.SaveGpuProfilesJson(json);
        }
        #endregion

        // 注意：已经通过url传参了，为了版本兼容性不能去掉[FromUri]了
        [HttpPost]
        public void SetAutoBootStart([FromUri]bool autoBoot, [FromUri]bool autoStart) {
            VirtualRoot.DaemonOperation.SetAutoBootStart(autoBoot, autoStart);
        }

        [HttpPost]
        public void StartOrCloseWs(bool isResetFailCount) {
            VirtualRoot.DaemonWsClient.OpenOrCloseWs(isResetFailCount);
        }

        [HttpPost]
        public ResponseBase RestartWindows([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.RestartWindows();
        }

        [HttpPost]
        public ResponseBase ShutdownWindows([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.ShutdownWindows();
        }

        [HttpPost]
        public ResponseBase StartMine([FromBody]WorkRequest request) {
            return VirtualRoot.DaemonOperation.StartMine(request);
        }

        [HttpPost]
        public ResponseBase StopMine([FromBody]SignRequest request) {
            if (request == null) {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.StopMine();
        }

        [HttpPost]
        public ResponseBase UpgradeNTMiner([FromBody]UpgradeNTMinerRequest request) {
            return VirtualRoot.DaemonOperation.UpgradeNTMiner(request);
        }

        [HttpGet]
        [HttpPost]
        public List<OperationResultDto> GetOperationResults(long afterTime) {
            return VirtualRoot.OperationResultSet.Gets(afterTime);
        }

        [HttpGet]
        [HttpPost]
        public List<DriveDto> GetDrives() {
            return VirtualRoot.DriveSet.AsEnumerable().ToList();
        }

        [HttpPost]
        public ResponseBase SetVirtualMemory([FromBody]DataRequest<Dictionary<string, int>> request) {
            if (request == null || request.Data == null || request.Data.Count == 0) {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.SetVirtualMemory(request.Data);
        }

        [HttpGet]
        [HttpPost]
        public List<LocalIpDto> GetLocalIps() {
            return VirtualRoot.LocalIpSet.AsEnumerable().ToList();
        }

        [HttpPost]
        public ResponseBase SetLocalIps(DataRequest<List<LocalIpInput>> request) {
            if (request == null || request.Data == null || request.Data.Count == 0) {
                return ResponseBase.InvalidInput("参数错误");
            }
            return VirtualRoot.DaemonOperation.SetLocalIps(request.Data);
        }
    }
}
