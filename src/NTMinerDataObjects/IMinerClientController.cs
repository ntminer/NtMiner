using NTMiner.Daemon;
using NTMiner.MinerClient;

namespace NTMiner {
    public interface IMinerClientController : IShowMainWindow {
        ResponseBase CloseNTMiner(SignatureRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(SignatureRequest request);
        ResponseBase SetMinerName(SetMinerNameRequest request);
        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);
        SpeedData GetSpeed();
    }
}
