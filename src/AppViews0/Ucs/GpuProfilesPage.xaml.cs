using NTMiner.Vms;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class GpuProfilesPage : UserControl {
        public static void ShowWindow(MinerClientsWindowViewModel minerClientsWindowVm) {
            if (minerClientsWindowVm.SelectedMinerClients == null && minerClientsWindowVm.SelectedMinerClients.Length != 1) {
                return;
            }
            var minerClientVm = minerClientsWindowVm.SelectedMinerClients[0];
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = $"超频 - 基于矿机{minerClientVm.MinerName}({minerClientVm.MinerIp})",
                IconName = "Icon_OverClock",
                Width = 800,
                Height = 700,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                window.Owner = WpfUtil.GetTopWindow();
                var vm = new GpuProfilesPageViewModel(minerClientsWindowVm) {
                    CloseWindow = () => {
                        window.Close();
                    }
                };
                var uc = new GpuProfilesPage(vm);
                var client = minerClientsWindowVm.SelectedMinerClients[0];
                void handler(object sender, PropertyChangedEventArgs e) {
                    if (e.PropertyName == nameof(minerClientsWindowVm.SelectedMinerClients)) {
                        if (minerClientsWindowVm.SelectedMinerClients.Contains(minerClientVm)) {
                            vm.IsMinerClientVmVisible = Visibility.Collapsed;
                        }
                        else {
                            vm.IsMinerClientVmVisible = Visibility.Visible;
                        }
                    }
                }

                minerClientsWindowVm.PropertyChanged += handler;
                uc.Unloaded += (object sender, RoutedEventArgs e)=> {
                    minerClientsWindowVm.PropertyChanged -= handler;
                };
                return uc;
            }, fixedSize: false);
        }

        public GpuProfilesPageViewModel Vm {
            get {
                return (GpuProfilesPageViewModel)this.DataContext;
            }
        }

        public GpuProfilesPage(GpuProfilesPageViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
    }
}
