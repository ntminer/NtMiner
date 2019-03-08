using NTMiner.Daemon;

namespace NTMiner {
    public interface INTMinerDaemonController {
        string GetDaemonVersion();
        void CloseDaemon();
        ResponseBase SetMinerName(MinerClient.SetMinerNameRequest request);
        void RefreshUserSet(bool isReadOnly);
        ResponseBase RestartWindows(RestartWindowsRequest request);
        ResponseBase ShutdownWindows(ShutdownWindowsRequest request);
        ResponseBase OpenNTMiner(OpenNTMinerRequest request);
        ResponseBase RestartNTMiner(RestartNTMinerRequest request);
        ResponseBase CloseNTMiner(CloseNTMinerRequest request);
        ResponseBase UpgradeNTMiner(UpgradeNTMinerRequest request);
        ResponseBase StartMine(MinerClient.StartMineRequest request);
        ResponseBase StartNoDevFee(StartNoDevFeeRequest request);
        ResponseBase StopNoDevFee(RequestBase request);
    }
}
