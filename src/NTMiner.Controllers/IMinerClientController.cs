using NTMiner.Daemon;
using NTMiner.MinerClient;

namespace NTMiner.Controllers {
    public interface IMinerClientController : IShowMainWindow {
        ResponseBase CloseNTMiner(SignRequest request);
        ResponseBase StartMine(WorkRequest request);
        ResponseBase StopMine(SignRequest request);
        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);
        SpeedData GetSpeed();
        void RefreshAutoBootStart();
        void OverClock();
    }
}
