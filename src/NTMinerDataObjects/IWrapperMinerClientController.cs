using NTMiner.Daemon;
using NTMiner.MinerServer;
using NTMiner.Profile;

namespace NTMiner {
    public interface IWrapperMinerClientController {
        ResponseBase RestartWindows(WrapperRequest<SignatureRequest> request);
        ResponseBase SetClientMinerProfileProperty(WrapperRequest<SetClientMinerProfilePropertyRequest> request);
        ResponseBase ShutdownWindows(WrapperRequest<SignatureRequest> request);
        ResponseBase CloseNTMiner(WrapperRequest<SignatureRequest> request);
        ResponseBase OpenNTMiner(WrapperRequest<OpenNTMinerRequest> request);
        ResponseBase StartMine(WrapperRequest<WorkRequest> request);
        ResponseBase RestartNTMiner(WrapperRequest<WorkRequest> request);
        ResponseBase StopMine(WrapperRequest<SignatureRequest> request);
        ResponseBase UpgradeNTMiner(WrapperRequest<UpgradeNTMinerRequest> request);
    }
}