using NTMiner.MinerStudio.Vms;
using NTMiner.Views;
using NTMiner.Vms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Views.Ucs {
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
                IsMaskTheParent = false,
                IsChildWindow = true,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                var vm = new GpuProfilesPageViewModel(minerClientsWindowVm);
                window.BuildCloseWindowOnecePath(vm.Id);
                var uc = new GpuProfilesPage(vm);
                var client = minerClientsWindowVm.SelectedMinerClients[0];
                void onSelectedMinerClientsChanged(object sender, PropertyChangedEventArgs e) {
                    if (e.PropertyName == nameof(minerClientsWindowVm.SelectedMinerClients)) {
                        List<MinerClientViewModel> toRemoves = new List<MinerClientViewModel>();
                        foreach (var item in vm.MinerClientVms) {
                            if (item != minerClientVm) {
                                var exist = minerClientsWindowVm.SelectedMinerClients.FirstOrDefault(a => a == item);
                                if (exist == null) {
                                    toRemoves.Add(item);
                                }
                            }
                        }
                        foreach (var item in toRemoves) {
                            vm.MinerClientVms.Remove(item);
                        }
                        List<MinerClientViewModel> toAdds = new List<MinerClientViewModel>();
                        foreach (var item in minerClientsWindowVm.SelectedMinerClients) {
                            var exist = vm.MinerClientVms.FirstOrDefault(a => a == item);
                            if (exist == null) {
                                toAdds.Add(item);
                            }
                        }
                        foreach (var item in toAdds) {
                            vm.MinerClientVms.Add(item);
                        }
                    }
                }

                minerClientsWindowVm.PropertyChanged += onSelectedMinerClientsChanged;
                uc.Unloaded += (object sender, RoutedEventArgs e) => {
                    minerClientsWindowVm.PropertyChanged -= onSelectedMinerClientsChanged;
                };
                window.BuildEventPath<GetGpuProfilesResponsedEvent>("收到GetGpuProfilesJson的响应", LogEnum.DevConsole, path: message => {
                    if (message.ClientId != minerClientVm.ClientId) {
                        return;
                    }
                    vm.SetData(message.Data);
                }, typeof(GpuProfilesPage));
                MinerStudioRoot.MinerStudioService.GetGpuProfilesJsonAsync(minerClientVm);
                return uc;
            }, fixedSize: false);
        }

        public GpuProfilesPageViewModel Vm { get; private set; }

        public GpuProfilesPage(GpuProfilesPageViewModel vm) {
            this.Vm = vm;
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
