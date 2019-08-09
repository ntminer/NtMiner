using NTMiner.Core;
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
            this.DataContext = AppContext.Instance.MinerProfileVm;
            InitializeComponent();
            switch (NTMinerRoot.Instance.GpuSet.GpuType) {
                case GpuType.NVIDIA:
                case GpuType.AMD:
                    this.TbRedText.Text = "超频有风险，操作需谨慎";
                    break;
                case GpuType.Empty:
                default:
                    this.TbRedText.Text = "没有矿卡或矿卡未驱动";
                    break;
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }

        private void CheckBoxIsAutoFanSpeed_Click(object sender, RoutedEventArgs e) {
            FrameworkElement checkBox = (FrameworkElement)sender;
            GpuProfileViewModel gpuProfileVm = (GpuProfileViewModel)checkBox.Tag;
            VirtualRoot.Execute(new AddOrUpdateGpuProfileCommand(gpuProfileVm));
        }
    }
}
