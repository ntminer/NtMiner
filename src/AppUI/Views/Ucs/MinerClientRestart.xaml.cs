using NTMiner.Notifications;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class MinerClientRestart : UserControl {
        public static void ShowWindow(List<MinerClientViewModel> minerClients) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_Restart",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                Guid workId = Guid.Empty;
                string title = "你确定重启选中的客户端吗？";
                if (minerClients.Count == 1) {
                    var minerClientVm = minerClients[0];
                    workId = minerClientVm.WorkId;
                    title = $"你确定重启 {minerClientVm.MinerName}({minerClientVm.MinerIp}) 吗？";
                }
                MinerClientRestartViewModel vm = null;
                vm = new MinerClientRestartViewModel(title, workId, ()=> {
                    if (minerClients.Count == 1) {
                        var minerClientVm = minerClients[0];
                        Server.MinerClientService.RestartNTMinerAsync(minerClientVm.MinerIp, vm.SelectedMineWork.Id, response => {
                            if (!response.IsSuccess()) {
                                if (response != null) {
                                    Write.UserLine(response.Description, ConsoleColor.Red);
                                    MinerClientsWindowViewModel.Current.Manager.CreateMessage()
                                         .Accent("#1751C3")
                                         .Background("Red")
                                         .HasBadge("Error")
                                         .HasMessage(response.Description)
                                         .Dismiss().WithButton("忽略", null)
                                         .Queue();
                                }
                            }
                        });
                        window.Close();
                    }
                    else {
                        foreach (var item in minerClients) {
                            Server.MinerClientService.RestartNTMinerAsync(item.MinerIp, vm.SelectedMineWork.Id, response => {
                                if (!response.IsSuccess()) {
                                    if (response != null) {
                                        Write.UserLine(response.Description, ConsoleColor.Red);
                                        MinerClientsWindowViewModel.Current.Manager.CreateMessage()
                                             .Accent("#1751C3")
                                             .Background("Red")
                                             .HasBadge("Error")
                                             .HasMessage($"{item.MinerName}({item.MinerIp}) {response.Description}")
                                             .Dismiss().WithButton("忽略", null)
                                             .Queue();
                                    }
                                }
                            });
                        }
                        window.Close();
                    }
                });
                vm.CloseWindow = () => window.Close();
                return new MinerClientRestart(vm);
            }, fixedSize: true);
        }

        public MinerClientRestartViewModel Vm {
            get {
                return (MinerClientRestartViewModel)this.DataContext;
            }
        }

        public MinerClientRestart(MinerClientRestartViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
        }
    }
}
