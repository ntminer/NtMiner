namespace NTMiner.Core.Impl {
    public class CoinSnapshotSet : CoinSnapshotSetBase, ICoinSnapshotSet {
        public CoinSnapshotSet(IClientDataSetBase clientSet) : base(isPull: false, clientSet) {
        }
    }
}
