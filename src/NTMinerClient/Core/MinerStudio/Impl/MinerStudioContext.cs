namespace NTMiner.Core.MinerStudio.Impl {
    public class MinerStudioContext : IMinerStudioContext {
        public MinerStudioContext() {
            this.UserAppSettingSet = new UserAppSettingSet();
            this.MineWorkSet = new MineWorkSet();
            this.MinerGroupSet = new MinerGroupSet();
            this.ColumnsShowSet = new ColumnsShowSet();
            this.NTMinerWalletSet = new NTMinerWalletSet();
        }

        // TODO:暂时没有用到，等用到时让WebApi登录Action返回UserAppSettingSet的数据
        public IUserAppSettingSet UserAppSettingSet { get; private set; }

        public IMineWorkSet MineWorkSet { get; private set; }

        public IMinerGroupSet MinerGroupSet { get; private set; }

        public IColumnsShowSet ColumnsShowSet { get; private set; }

        public INTMinerWalletSet NTMinerWalletSet { get; private set; }
    }
}
