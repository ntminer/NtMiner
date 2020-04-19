using NTMiner.AppSetting;
using NTMiner.Core.Impl;

namespace NTMiner.Core.MinerStudio.Impl {
    public class MinerStudioContext : IMinerStudioContext {
        public MinerStudioContext() {
            this.ServerAppSettingSet = new ServerAppSettingSet();
            this.UserAppSettingSet = new UserAppSettingSet();
            this.MineWorkSet = new MineWorkSet();
            this.MinerGroupSet = new MinerGroupSet();
            this.ColumnsShowSet = new ColumnsShowSet();
            this.NTMinerWalletSet = new NTMinerWalletSet();
        }

        public IAppSettingSet ServerAppSettingSet { get; private set; }

        public IUserAppSettingSet UserAppSettingSet { get; private set; }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public INTMinerWalletSet NTMinerWalletSet { get; private set; }
    }
}
