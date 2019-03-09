using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using NTMiner.Profile;

namespace NTMiner {
    public interface IWrapperMinerClientController {
        ResponseBase RestartWindows(WrapperRequest<RestartWindowsRequest> request);
        ResponseBase SetClientMinerProfileProperty(WrapperRequest<SetClientMinerProfilePropertyRequest> request);
        ResponseBase ShutdownWindows(WrapperRequest<ShutdownWindowsRequest> request);
        ResponseBase CloseNTMiner(WrapperRequest<CloseNTMinerRequest> request);
        ResponseBase OpenNTMiner(WrapperRequest<MinerServer.OpenNTMinerRequest> request);
        ResponseBase StartMine(WrapperRequest<MinerClient.StartMineRequest> request);
        ResponseBase RestartNTMiner(WrapperRequest<MinerServer.RestartNTMinerRequest> request);
        ResponseBase StopMine(WrapperRequest<StopMineRequest> request);
        ResponseBase UpgradeNTMiner(WrapperRequest<UpgradeNTMinerRequest> request);
    }
}