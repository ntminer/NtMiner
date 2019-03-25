using NTMiner.Daemon;

namespace NTMiner.Controllers {
    public interface INTMinerDaemonController {
        string GetDaemonVersion();
        void CloseDaemon();
        string GetGpuProfilesJson();
        void SaveGpuProfilesJson();
        void SetAutoBootStart(bool autoBoot, bool autoStart);
        ResponseBase RestartWindows(SignatureRequest request);
        ResponseBase ShutdownWindows(SignatureRequest request);
        ResponseBase RestartNTMiner(WorkRequest request);
        ResponseBase UpgradeNTMiner(UpgradeNTMinerRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(SignatureRequest request);
        ResponseBase StartNoDevFee(StartNoDevFeeRequest request);
        ResponseBase StopNoDevFee(RequestBase request);
    }
}
