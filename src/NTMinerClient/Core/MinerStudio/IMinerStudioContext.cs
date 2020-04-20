using NTMiner.AppSetting;

namespace NTMiner.Core.MinerStudio {
    public interface IMinerStudioContext {
        IUserAppSettingSet UserAppSettingSet { get; }
        IMineWorkSet MineWorkSet { get; }
        IMinerGroupSet MinerGroupSet { get; }
        IColumnsShowSet ColumnsShowSet { get; }
        INTMinerWalletSet NTMinerWalletSet { get; }
    }
}
