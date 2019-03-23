using NTMiner.Daemon;
using NTMiner.MinerClient;
using NTMiner.MinerServer;

namespace NTMiner.Controllers {
    public interface IWrapperMinerClientController {
        ResponseBase RestartWindows(WrapperRequest<SignatureRequest> request);
        ResponseBase SetClientMinerProfileProperty(WrapperRequest<SetClientMinerProfilePropertyRequest> request);
        ResponseBase ShutdownWindows(WrapperRequest<SignatureRequest> request);
        ResponseBase StartMine(WrapperRequest<WorkRequest> request);
        ResponseBase RestartNTMiner(WrapperRequest<WorkRequest> request);
        ResponseBase StopMine(WrapperRequest<SignatureRequest> request);
        ResponseBase UpgradeNTMiner(WrapperRequest<UpgradeNTMinerRequest> request);
    }
}