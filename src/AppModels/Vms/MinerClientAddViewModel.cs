using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerClientAddViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        private string _leftIp = "192.168.0.100";
        private string _rightIp = "192.168.0.200";
        private bool _isIpRange;
        private string _message;
        private Visibility _messageVisible = Visibility.Collapsed;

        public ICommand Save { get; private set; }

        public MinerClientAddViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                if (!IPAddress.TryParse(this.LeftIp, out _)) {
                    this.ShowMessage("IP格式不正确");
                    return;
                }

                if (this.IsIpRange) {
                    if (!IPAddress.TryParse(this.RightIp, out _)) {
                        this.ShowMessage("IP格式不正确");
                        return;
                    }

                    List<string> clientIps = Net.Util.CreateIpRange(this.LeftIp, this.RightIp);

                    if (clientIps.Count > 101) {
                        this.ShowMessage("最多支持一次添加101个IP");
                        return;
                    }

                    if (clientIps.Count == 0) {
                        this.ShowMessage("没有IP");
                        return;
                    }
                    Server.ClientService.AddClientsAsync(clientIps, (response, e) => {
                        if (!response.IsSuccess()) {
                            this.ShowMessage(response.ReadMessage(e));
                        }
                        else {
                            AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
                            UIThread.Execute(() => VirtualRoot.Execute(new CloseWindowCommand(this.Id)));
                        }
                    });
                }
                else {
                    Server.ClientService.AddClientsAsync(new List<string> { this.LeftIp }, (response, e) => {
                        if (!response.IsSuccess()) {
                            this.ShowMessage(response.ReadMessage(e));
                        }
                        else {
                            AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
                            UIThread.Execute(() => VirtualRoot.Execute(new CloseWindowCommand(this.Id)));
                        }
                    });
                }
            });
            var localIp = VirtualRoot.LocalIpSet.AsEnumerable().FirstOrDefault();
            if (localIp != null) {
                uint left = Net.Util.ConvertToIpNum(localIp.DefaultIPGateway) + 1;
                this._leftIp = Net.Util.ConvertToIpString(left);
                this._rightIp = Net.Util.ConvertToIpString(left + 100);
            }
        }

        private void ShowMessage(string message) {
            this.Message = message;
            MessageVisible = Visibility.Visible;
            TimeSpan.FromSeconds(4).Delay().ContinueWith(t => {
                UIThread.Execute(() => {
                    MessageVisible = Visibility.Collapsed;
                });
            });
        }

        public string Message {
            get => _message;
            set {
                if (_message != value) {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        public Visibility MessageVisible {
            get => _messageVisible;
            set {
                if (_messageVisible != value) {
                    _messageVisible = value;
                    OnPropertyChanged(nameof(MessageVisible));
                }
            }
        }

        public string LeftIp {
            get => _leftIp;
            set {
                _leftIp = value;
                OnPropertyChanged(nameof(LeftIp));
                if (!IPAddress.TryParse(value, out _)) {
                    throw new ValidationException("IP格式不正确");
                }
            }
        }

        public bool IsIpRange {
            get => _isIpRange;
            set {
                _isIpRange = value;
                OnPropertyChanged(nameof(IsIpRange));
            }
        }

        public string RightIp {
            get => _rightIp;
            set {
                _rightIp = value;
                OnPropertyChanged(nameof(RightIp));
                if (!IPAddress.TryParse(value, out _)) {
                    throw new ValidationException("IP格式不正确");
                }
            }
        }
    }
}
