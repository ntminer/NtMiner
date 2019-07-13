namespace NTMiner.Vms {
    public class CalcViewModel : ViewModelBase {
        public AppContext.CoinViewModels CoinVms {
            get {
                return AppContext.Instance.CoinVms;
            }
        }
    }
}
