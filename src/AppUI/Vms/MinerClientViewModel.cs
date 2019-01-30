using NTMiner.ServiceContracts.DataObjects;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientViewModel : ViewModelBase {
        private Visibility _isReNameVisible = Visibility.Collapsed;
        private Visibility _isChangeGroupVisible = Visibility.Collapsed;

        public ICommand RestartWindows { get; private set; }
        public ICommand ShutdownWindows { get; private set; }
        public ICommand StartNTMiner { get; private set; }
        public ICommand RestartNTMiner { get; private set; }
        public ICommand CloseNTMiner { get; private set; }
        public ICommand ShowReName { get; private set; }
        public ICommand CancelReName { get; private set; }
        public ICommand ReName { get; private set; }
        public ICommand ShowChangeGroup { get; private set; }
        public ICommand ChangeGroup { get; private set; }
        public ICommand CancelChangeGroup { get; private set; }
        public ICommand StartMine { get; private set; }
        public ICommand StopMine { get; private set; }

        public MinerClientViewModel(ClientData clientData) {
            this.ClientDataVm = new ClientDataViewModel(clientData);
            this.RestartWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定重启{this.ClientDataVm.MinerName}电脑吗？", title: "确认", onYes: () => {
                    NTMinerClientDaemon.Instance.RestartWindowsAsync(this.ClientDataVm.MinerIp, Global.ClientPort, null);
                }, icon: "Icon_Confirm");
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关机{this.ClientDataVm.MinerName}电脑吗？", title: "确认", onYes: () => {
                    NTMinerClientDaemon.Instance.ShutdownWindowsAsync(this.ClientDataVm.MinerIp, Global.ClientPort, null);
                }, icon: "Icon_Confirm");
            });
            this.StartNTMiner = new DelegateCommand(() => {
                NTMinerClientDaemon.Instance.OpenNTMinerAsync(this.ClientDataVm.MinerIp, Global.ClientPort, this.ClientDataVm.WorkId, null);
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                MinerClientRestart.ShowWindow(this);
            });
            this.CloseNTMiner = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关闭{this.ClientDataVm.MinerName}挖矿客户端吗？关闭客户端软件，并非关闭电脑。", title: "确认", onYes: () => {
                    NTMinerClientDaemon.Instance.CloseNTMinerAsync(this.ClientDataVm.MinerIp, Global.ClientPort, null);
                }, icon: "Icon_Confirm");
            });
            this.ShowReName = new DelegateCommand(() => {
                this.ClientDataVm.MinerNameCopy = this.ClientDataVm.MinerName;
                this.IsReNameVisible = Visibility.Visible;
            });
            this.ReName = new DelegateCommand(() => {
                MinerClientService.Instance.SetMinerProfilePropertyAsync(
                            this.ClientDataVm.MinerIp,
                            nameof(ClientDataVm.MinerName),
                            this.ClientDataVm.MinerNameCopy, null);
                TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                    Refresh();
                });
                this.IsReNameVisible = Visibility.Collapsed;
            });
            this.CancelReName = new DelegateCommand(() => {
                this.IsReNameVisible = Visibility.Collapsed;
            });
            this.ShowChangeGroup = new DelegateCommand(() => {
                this.IsChangeGroupVisible = Visibility.Visible;
            });
            this.ChangeGroup = new DelegateCommand(() => {
                try {
                    Server.ControlCenterService.UpdateClientAsync(
                        this.ClientDataVm.Id,
                        nameof(ClientDataVm.GroupId),
                        this.ClientDataVm.SelectedMinerGroupCopy.Id, null);
                    this.ClientDataVm.GroupId = this.ClientDataVm.SelectedMinerGroupCopy.Id;
                    TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                        Refresh();
                    });
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                }
                this.IsChangeGroupVisible = Visibility.Collapsed;
            });
            this.CancelChangeGroup = new DelegateCommand(() => {
                this.IsChangeGroupVisible = Visibility.Collapsed;
            });
            this.StartMine = new DelegateCommand(() => {
                ClientDataVm.IsMining = true;
                MinerClientService.Instance.StartMineAsync(this.ClientDataVm.MinerIp, ClientDataVm.WorkId, null);
                TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                    Refresh();
                });
            });
            this.StopMine = new DelegateCommand(() => {
                ClientDataVm.IsMining = false;
                MinerClientService.Instance.StopMineAsync(this.ClientDataVm.MinerIp, null);
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

        public Visibility IsChangeGroupVisible {
            get { return _isChangeGroupVisible; }
            set {
                if (_isChangeGroupVisible != value) {
                    _isChangeGroupVisible = value;
                    OnPropertyChanged(nameof(IsChangeGroupVisible));
                }
            }
        }

        public ClientDataViewModel ClientDataVm {
            get; private set;
        }
    }
}
