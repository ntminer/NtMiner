using NTMiner.MinerClient;

namespace NTMiner.MinerServer {
    public interface IClientData : IMinerData, ISpeedData, IEntity<string> {
    }
}
