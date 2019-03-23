using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class GpuOverClock : UserControl {
        public MinerProfileViewModel Vm {
            get {
                return (MinerProfileViewModel)this.DataContext;
            }
        }

        public GpuOverClock() {
            InitializeComponent();
            switch (NTMinerRoot.Current.GpuSet.GpuType) {
                case GpuType.NVIDIA:
                    this.TbRedText.Text = "超频有风险，操作需谨慎";
                    break;
                case GpuType.AMD:
                    this.TbRedText.Text = "暂不支持A卡超频";
                    break;
                case GpuType.Empty:
                default:
                    this.TbRedText.Text = "没有显卡";
                    break;
            }
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
