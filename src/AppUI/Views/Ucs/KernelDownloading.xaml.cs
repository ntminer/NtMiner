using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelDownloading : UserControl {
        public static string ViewId = nameof(KernelDownloading);

        public KernelPageViewModel Vm {
            get {
                return (KernelPageViewModel)this.DataContext;
            }
        }

        public KernelDownloading() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void BtnHide_Click(object sender, System.Windows.RoutedEventArgs e) {
            Vm.KernelDownloadingVisible = System.Windows.Visibility.Collapsed;
        }
    }
}
