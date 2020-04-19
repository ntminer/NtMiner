using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IDaemonOperation {
        bool IsNTMinerOpened();
        void CloseDaemon();
        ResponseBase EnableRemoteDesktop();
        ResponseBase BlockWAU();
        ResponseBase AtikmdagPatcher();
        ResponseBase SwitchRadeonGpu(bool on);
        string GetGpuProfilesJson();
        ResponseBase RestartWindows();
        bool SaveGpuProfilesJson(string json);
        bool SetAutoBootStart(bool autoBoot, bool autoStart);
        ResponseBase ShutdownWindows();
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine();
        ResponseBase UpgradeNTMiner(UpgradeNTMinerRequest request);
        ResponseBase SetVirtualMemory(Dictionary<string, int> data);
        ResponseBase SetLocalIps(List<LocalIpInput> data);
    }
}