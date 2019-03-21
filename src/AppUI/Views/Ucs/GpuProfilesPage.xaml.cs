using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class GpuProfilesPage : UserControl {
        public static void ShowWindow(GpuProfilesPageViewModel vm) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IsDialogWindow = true,
                IconName = "Icon_OverClock",
                Width = 600,
                Height = 400,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => new GpuProfilesPage(vm), fixedSize: true);
        }

        public GpuProfilesPageViewModel Vm {
            get {
                return (GpuProfilesPageViewModel)this.DataContext;
            }
        }

        public GpuProfilesPage(GpuProfilesPageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
