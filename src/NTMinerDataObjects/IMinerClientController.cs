using NTMiner.Daemon;
using NTMiner.Hashrate;
using NTMiner.MinerClient;

namespace NTMiner {
    public interface IMinerClientController {
        ResponseBase CloseNTMiner(CloseNTMinerRequest request);
        ResponseBase StartMine(StartMineRequest request);
        ResponseBase StopMine(StopMineRequest request);
        ResponseBase SetMinerName(SetMinerNameRequest request);
        ResponseBase SetMinerProfileProperty(SetMinerProfilePropertyRequest request);
        SpeedData GetSpeed();
    }
}
