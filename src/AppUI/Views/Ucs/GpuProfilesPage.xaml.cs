using NTMiner.MinerServer;
using NTMiner.Vms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class GpuProfilesPage : UserControl {
        public static void ShowWindow(IClientData client) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                HasOwner = true,
                IsTopMost = true,
                IconName = "Icon_OverClock",
                Width = 800,
                Height = 600,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                var vm = new GpuProfilesPageViewModel(client);
                vm.CloseWindow = () => {
                    window.Close();
                };
                var uc = new GpuProfilesPage(vm);
                ResourceDictionarySet.Instance.TryGetResourceDic(nameof(GpuProfilesPage), out ResourceDictionary resourceDictionary);
                resourceDictionary["WindowTitle"] = $"超频 - {client.MinerName}({client.MinerIp})";
                return uc;
            }, fixedSize: true);
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
