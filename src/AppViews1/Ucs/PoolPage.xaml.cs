using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class PoolPage : UserControl {
        private CoinPageViewModel Vm {
            get {
                return (CoinPageViewModel)this.DataContext;
            }
        }

        public PoolPage() {
            InitializeComponent();
            AppContext.Instance.CoinVms.PropertyChanged += Current_PropertyChanged;
            this.Unloaded += CoinPage_Unloaded;
        }

        private void CoinPage_Unloaded(object sender, System.Windows.RoutedEventArgs e) {
            AppContext.Instance.CoinVms.PropertyChanged -= Current_PropertyChanged;
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(AppContext.Instance.CoinVms.AllCoins)) {
                Vm.OnPropertyChanged(nameof(Vm.List));
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<CoinViewModel>(sender, e);
        }

        private void PoolDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Wpf.Util.DataGrid_MouseDoubleClick<PoolViewModel>(sender, e);
        }
    }
}
