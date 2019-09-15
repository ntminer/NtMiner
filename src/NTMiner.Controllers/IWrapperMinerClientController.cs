using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;

namespace NTMiner.Controllers {
    public interface IWrapperMinerClientController {
        ResponseBase RestartWindows(WrapperRequest<SignRequest> request);
        ResponseBase SetClientMinerProfileProperty(WrapperRequest<SetClientMinerProfilePropertyRequest> request);
        ResponseBase ShutdownWindows(WrapperRequest<SignRequest> request);
        ResponseBase StartMine(WrapperRequest<WorkRequest> request);
        ResponseBase RestartNTMiner(WrapperRequest<WorkRequest> request);
        ResponseBase StopMine(WrapperRequest<SignRequest> request);
        ResponseBase UpgradeNTMiner(WrapperRequest<UpgradeNTMinerRequest> request);
    }
}