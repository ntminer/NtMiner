using NTMiner.Core.Daemon;

namespace NTMiner.Controllers {
    public interface INTMinerDaemonController {
        ResponseBase EnableWindowsRemoteDesktop();
        void CloseDaemon();
        string GetGpuProfilesJson();
        void SaveGpuProfilesJson();
        void SetAutoBootStart(bool autoBoot, bool autoStart);
        ResponseBase RestartWindows(SignRequest request);
        ResponseBase ShutdownWindows(SignRequest request);
        ResponseBase RestartNTMiner(WorkRequest request);
        ResponseBase UpgradeNTMiner(UpgradeNTMinerRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(SignRequest request);
        ResponseBase SetWallet(SetWalletRequest request);
    }
}
