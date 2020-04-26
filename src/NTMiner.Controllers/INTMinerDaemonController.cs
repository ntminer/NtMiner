using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.VirtualMemory;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface INTMinerDaemonController {
        DataResponse<IntPtr> ShowConsole();
        WsClientState GetWsDaemonState();
        ResponseBase EnableRemoteDesktop();
        ResponseBase BlockWAU();
        ResponseBase AtikmdagPatcher();
        ResponseBase SwitchRadeonGpu(bool on);
        void CloseDaemon();
        string GetLocalJson();
        string GetGpuProfilesJson();
        void SaveGpuProfilesJson();
        // TODO:应该新增一个Action用于处理配置合集
        void SetAutoBootStart(bool autoBoot, bool autoStart);
        void StartOrCloseWs(bool isResetFailCount);
        ResponseBase RestartWindows(object request);
        ResponseBase ShutdownWindows(object request);
        ResponseBase UpgradeNTMiner(UpgradeNTMinerRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(object request);
        List<OperationResultDto> GetOperationResults(long afterTime);
        List<DriveDto> GetDrives();
        ResponseBase SetVirtualMemory(DataRequest<Dictionary<string, int>> request);
        List<LocalIpDto> GetLocalIps();
        ResponseBase SetLocalIps(DataRequest<List<LocalIpInput>> request);
    }
}
