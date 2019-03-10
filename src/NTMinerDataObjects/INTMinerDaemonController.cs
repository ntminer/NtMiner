using NTMiner.Daemon;

namespace NTMiner {
    public interface INTMinerDaemonController {
        string GetDaemonVersion();
        void CloseDaemon();
        void RefreshNotifyIcon();
        ResponseBase SetMinerName(MinerClient.SetMinerNameRequest request);
        void RefreshUserSet();
        ResponseBase RestartWindows(SignatureRequest request);
        ResponseBase ShutdownWindows(SignatureRequest request);
        ResponseBase OpenNTMiner(WorkRequest request);
        ResponseBase RestartNTMiner(WorkRequest request);
        ResponseBase CloseNTMiner(SignatureRequest request);
        ResponseBase UpgradeNTMiner(UpgradeNTMinerRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StartNoDevFee(StartNoDevFeeRequest request);
        ResponseBase StopNoDevFee(RequestBase request);
    }
}
