using NTMiner.MinerStudio.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
    public partial class GpuNames : UserControl {
        public GpuNames() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            InitializeComponent();
            this.OnLoaded(onLoad: window => {
                window.AddEventPath<GpuNameAddedEvent>("添加了显卡特征名后刷新显卡特征名列表", LogEnum.DevConsole, action: message => {
                    UIThread.Execute(() => {
                        ((GpuNamesViewModel)this.DataContext).Query();
                    });
                }, typeof(GpuNames));
            });
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            ((GpuNamesViewModel)this.DataContext).Search.Execute(null);
        }

        private void TbKeyword_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                ((GpuNamesViewModel)this.DataContext).Keyword = this.TbKeyword.Text;
            }
        }
    }
}
