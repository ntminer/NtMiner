using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Profile;

namespace NTMiner {
    public interface IWrapperMinerClientController {
        ResponseBase CloseNTMiner(WrapperRequest<CloseNTMinerRequest> request);
        ResponseBase OpenNTMiner(WrapperRequest<OpenNTMinerRequest> request);
        ResponseBase RestartNTMiner(WrapperRequest<RestartNTMinerRequest> request);
        ResponseBase RestartWindows(WrapperRequest<RestartWindowsRequest> request);
        ResponseBase SetClientMinerProfileProperty(WrapperRequest<SetClientMinerProfilePropertyRequest> request);
        ResponseBase ShutdownWindows(WrapperRequest<ShutdownWindowsRequest> request);
        ResponseBase StartMine(WrapperRequest<StartMineRequest> request);
        ResponseBase StopMine(WrapperRequest<StopMineRequest> request);
        ResponseBase UpgradeNTMiner(WrapperRequest<UpgradeNTMinerRequest> request);
    }
}