using NTMiner.MinerStudio.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class GpuNameCounts : UserControl {
        public GpuNameCounts() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            InitializeComponent();
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            ((GpuNameCountsViewModel)this.DataContext).Search.Execute(null);
        }

        private void TbKeyword_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                ((GpuNameCountsViewModel)this.DataContext).Keyword = this.TbKeyword.Text;
            }
        }
    }
}
