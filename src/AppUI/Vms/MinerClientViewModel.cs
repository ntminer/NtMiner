using NTMiner.ServiceContracts.DataObjects;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientViewModel : ViewModelBase {
        private Visibility _isReNameVisible = Visibility.Collapsed;
        private Visibility _isChangeGroupVisible = Visibility.Collapsed;
        private bool _isNTMinerDaemonOnline;

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
                    NTMinerClientDaemon.Instance.RestartWindows(this.ClientDataVm.MinerIp, Global.ClientPort, null);
                }, icon: "Icon_Confirm");
            });
            this.ShutdownWindows = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关机{this.ClientDataVm.MinerName}电脑吗？", title: "确认", onYes: () => {
                    NTMinerClientDaemon.Instance.ShutdownWindows(this.ClientDataVm.MinerIp, Global.ClientPort, null);
                }, icon: "Icon_Confirm");
            });
            this.StartNTMiner = new DelegateCommand(() => {
                NTMinerClientDaemon.Instance.OpenNTMiner(this.ClientDataVm.MinerIp, Global.ClientPort, this.ClientDataVm.WorkId, null);
            });
            this.RestartNTMiner = new DelegateCommand(() => {
                MinerClientRestart.ShowWindow(this);
            });
            this.CloseNTMiner = new DelegateCommand(() => {
                DialogWindow.ShowDialog(message: $"您确定关闭{this.ClientDataVm.MinerName}挖矿客户端吗？关闭客户端软件，并非关闭电脑。", title: "确认", onYes: () => {
                    NTMinerClientDaemon.Instance.CloseNTMiner(this.ClientDataVm.MinerIp, Global.ClientPort, null);
                }, icon: "Icon_Confirm");
            });
            this.ShowReName = new DelegateCommand(() => {
                this.ClientDataVm.MinerNameCopy = this.ClientDataVm.MinerName;
                this.IsReNameVisible = Visibility.Visible;
            });
            this.ReName = new DelegateCommand(() => {
                MinerClientService.Instance.SetMinerProfileProperty(
                            this.ClientDataVm.MinerIp,
                            this.ClientDataVm.PublicKey,
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
                Task.Factory.StartNew(() => {
                    try {
                        Server.ControlCenterService.UpdateClient(
                            this.ClientDataVm.Id,
                            nameof(ClientDataVm.GroupId),
                            this.ClientDataVm.SelectedMinerGroupCopy.Id, null);
                        this.ClientDataVm.GroupId = this.ClientDataVm.SelectedMinerGroupCopy.Id;
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                    TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                        Refresh();
                    });
                });
                this.IsChangeGroupVisible = Visibility.Collapsed;
            });
            this.CancelChangeGroup = new DelegateCommand(() => {
                this.IsChangeGroupVisible = Visibility.Collapsed;
            });
            this.StartMine = new DelegateCommand(() => {
                ClientDataVm.IsMining = true;
                MinerClientService.Instance.StartMine(this.ClientDataVm.MinerIp, this.ClientDataVm.PublicKey, ClientDataVm.WorkId, null);
                TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                    Refresh();
                });
            });
            this.StopMine = new DelegateCommand(() => {
                ClientDataVm.IsMining = false;
                MinerClientService.Instance.StopMine(this.ClientDataVm.MinerIp, this.ClientDataVm.PublicKey, null);
                TimeSpan.FromSeconds(2).Delay().ContinueWith((t) => {
                    Refresh();
                });
            });
            NTMinerClientDaemon.Instance.IsNTMinerDaemonOnline(this.ClientDataVm.MinerIp, Global.ClientPort, isOnline => {
                this.IsNTMinerDaemonOnline = isOnline;
            });
        }

        private void Refresh() {
            Server.ControlCenterService.LoadClient(this.ClientDataVm.GetId(), data => {
                if (data != null) {
                    this.ClientDataVm.Update(data);
                }
            });
        }

        public bool IsNTMinerDaemonOnline {
            get => _isNTMinerDaemonOnline;
            set {
                _isNTMinerDaemonOnline = value;
                OnPropertyChanged(nameof(IsNTMinerDaemonOnline));
            }
        }

        public Visibility IsReNameVisible {
            get => _isReNameVisible;
            set {
                _isReNameVisible = value;
                OnPropertyChanged(nameof(IsReNameVisible));
            }
        }

        public Visibility IsChangeGroupVisible {
            get { return _isChangeGroupVisible; }
            set {
                _isChangeGroupVisible = value;
                OnPropertyChanged(nameof(IsChangeGroupVisible));
            }
        }

        public ClientDataViewModel ClientDataVm {
            get; private set;
        }
    }
}
