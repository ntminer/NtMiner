using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientViewModel : ViewModelBase {
        private Visibility _isReNameVisible = Visibility.Collapsed;

        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        public ICommand StartNTMiner { get; private set; }
        public ICommand RestartNTMiner { get; private set; }
        public ICommand CloseNTMiner { get; private set; }
        public ICommand ShowReName { get; private set; }
        public ICommand CancelReName { get; private set; }
        public ICommand ReName { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        public MinerClientViewModel(ClientData clientData) {
            this.ClientDataVm = new ClientDataViewModel(clientData);
            this.RestartWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定重启{this.ClientDataVm.MinerName}电脑吗？", title: "确认", onYes: () => {
                    NTMinerDaemonService.Instance.RestartWindowsAsync(this.ClientDataVm.MinerIp, 3337, null);
                }, icon: "Icon_Confirm");
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关机{this.ClientDataVm.MinerName}电脑吗？", title: "确认", onYes: () => {
                    NTMinerDaemonService.Instance.ShutdownWindowsAsync(this.ClientDataVm.MinerIp, 3337, null);
                }, icon: "Icon_Confirm");
            });
            this.StartNTMiner = new DelegateCommand(() => {
                NTMinerDaemonService.Instance.OpenNTMinerAsync(this.ClientDataVm.MinerIp, 3337, this.ClientDataVm.WorkId, null);
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                MinerClientRestart.ShowWindow(this);
            });
            this.CloseNTMiner = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关闭{this.ClientDataVm.MinerName}挖矿客户端吗？关闭客户端软件，并非关闭电脑。", title: "确认", onYes: () => {
                    NTMinerDaemonService.Instance.CloseNTMinerAsync(this.ClientDataVm.MinerIp, 3337, null);
                }, icon: "Icon_Confirm");
            });
            this.ShowReName = new DelegateCommand(() => {
                this.ClientDataVm.MinerNameCopy = this.ClientDataVm.MinerName;
                this.IsReNameVisible = Visibility.Visible;
            });
            this.ReName = new DelegateCommand(() => {
                MinerClientService.Instance.SetMinerProfilePropertyAsync(
                    this.ClientDataVm.MinerIp, nameof(ClientDataVm.MinerName), this.ClientDataVm.MinerNameCopy, response => {
                        if (!response.IsSuccess()) {
                            Write.UserLine($"{this.ClientDataVm.MinerIp} {response?.Description}", ConsoleColor.Red);
                        }
                    });
                TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                    Refresh();
                });
                this.IsReNameVisible = Visibility.Collapsed;
            });
            this.CancelReName = new DelegateCommand(() => {
                this.IsReNameVisible = Visibility.Collapsed;
            });
            this.StartMine = new DelegateCommand(() => {
                ClientDataVm.IsMining = true;
                MinerClientService.Instance.StartMineAsync(this.ClientDataVm.MinerIp, ClientDataVm.WorkId, response=> {
                    if (!response.IsSuccess()) {
                        Write.UserLine($"{this.ClientDataVm.MinerIp} {response?.Description}", ConsoleColor.Red);
                    }
                });
                TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                    Refresh();
                });
            });
            this.StopMine = new DelegateCommand(() => {
                ClientDataVm.IsMining = false;
                MinerClientService.Instance.StopMineAsync(this.ClientDataVm.MinerIp, response => {
                    if (!response.IsSuccess()) {
                        Write.UserLine($"{this.ClientDataVm.MinerIp} {response?.Description}", ConsoleColor.Red);
                    }
                });
                TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                    Refresh();
                });
            });
        }

        private void Refresh() {
            Server.ControlCenterService.LoadClientAsync(this.ClientDataVm.GetId(), data => {
                if (data != null) {
                    this.ClientDataVm.Update(data);
                }
            });
        }

        public Visibility IsReNameVisible {
            get => _isReNameVisible;
            set {
                if (_isReNameVisible != value) {
                    _isReNameVisible = value;
                    OnPropertyChanged(nameof(IsReNameVisible));
                }
            }
        }

        public ClientDataViewModel ClientDataVm {
            get; private set;
        }
    }
}
