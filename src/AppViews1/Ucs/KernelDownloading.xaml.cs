using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelDownloading : UserControl {
        public KernelsWindowViewModel Vm {
            get {
                return (KernelsWindowViewModel)this.DataContext;
            }
        }

        public KernelDownloading() {
            InitializeComponent();
        }

        private void BtnHide_Click(object sender, System.Windows.RoutedEventArgs e) {
            
        }
    }
}
