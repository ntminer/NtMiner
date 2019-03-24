using NTMiner.Daemon;
using NTMiner.MinerClient;

namespace NTMiner.Controllers {
    public interface IMinerClientController : IShowMainWindow {
        ResponseBase CloseNTMiner(SignatureRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(SignatureRequest request);
        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);
        SpeedData GetSpeed();
        void RefreshAutoBootStart();
        void OverClock();
    }
}
