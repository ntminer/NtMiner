using NTMiner.Vms;

namespace NTMiner {
    public class MinerStudioAppContext {
        /// <summary>
        /// 这个可以在关闭界面的时候释放
        /// </summary>
        public static readonly MinerStudioAppContext Current = new MinerStudioAppContext();

        private MinerStudioAppContext() {
        }

        private CoinSnapshotDataViewModels _coinSnapshotDataVms;
        public CoinSnapshotDataViewModels CoinSnapshotDataVms {
            get {
                return _coinSnapshotDataVms ?? (_coinSnapshotDataVms = new CoinSnapshotDataViewModels());
            }
        }
    }
}
