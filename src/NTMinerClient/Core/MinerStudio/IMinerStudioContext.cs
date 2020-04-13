using NTMiner.Core.MinerServer;
using NTMiner.User;

namespace NTMiner.Core.MinerStudio {
    public interface IMinerStudioContext {
        IUserAppSettingSet UserAppSettingSet { get; }
        IMineWorkSet MineWorkSet { get; }
        IMinerGroupSet MinerGroupSet { get; }
        IColumnsShowSet ColumnsShowSet { get; }
        INTMinerWalletSet NTMinerWalletSet { get; }
    }
}
