using NTMiner.Daemon;

namespace NTMiner.Controllers {
    public interface INTMinerDaemonController {
        string GetDaemonVersion();
        void CloseDaemon();
        void RefreshNotifyIcon();
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
