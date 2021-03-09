using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class MinerClientAddViewModel : ViewModelBase {
        public readonly Guid Id = Guid.NewGuid();
        private string _leftIp = "192.168.0.100";
        private string _rightIp = "192.168.0.200";
        private bool _isIpRange;

        public ICommand Save { get; private set; }

        public MinerClientAddViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                if (!IPAddress.TryParse(this.LeftIp, out _)) {
                    VirtualRoot.Out.ShowError("IP格式不正确", autoHideSeconds: 4);
                    return;
                }

                if (this.IsIpRange) {
                    if (!IPAddress.TryParse(this.RightIp, out _)) {
                        VirtualRoot.Out.ShowError("IP格式不正确", autoHideSeconds: 4);
                        return;
                    }

                    List<string> clientIps = Net.IpUtil.CreateIpRange(this.LeftIp, this.RightIp);

                    if (clientIps.Count > 101) {
                        VirtualRoot.Out.ShowError("最多支持一次添加101个IP", autoHideSeconds: 4);
                        return;
                    }

                    if (clientIps.Count == 0) {
                        VirtualRoot.Out.ShowError("没有IP", autoHideSeconds: 4);
                        return;
                    }
                    MinerStudioRoot.LocalMinerStudioService.AddClientsAsync(clientIps, (response, e) => {
                        if (!response.IsSuccess()) {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e));
                        }
                        else {
                            MinerStudioRoot.MinerClientsWindowVm.QueryMinerClients();
                            VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                        }
                    });
                }
                else {
                    MinerStudioRoot.LocalMinerStudioService.AddClientsAsync(new List<string> { this.LeftIp }, (response, e) => {
                        if (!response.IsSuccess()) {
                            VirtualRoot.Out.ShowError(response.ReadMessage(e));
                        }
                        else {
                            MinerStudioRoot.MinerClientsWindowVm.QueryMinerClients();
                            VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                        }
                    });
                }
            });
            var localIp = VirtualRoot.LocalIpSet.AsEnumerable().FirstOrDefault();
            if (localIp != null) {
                uint left = Net.IpUtil.ConvertToIpNum(localIp.DefaultIPGateway) + 1;
                this._leftIp = Net.IpUtil.ConvertToIpString(left);
                this._rightIp = Net.IpUtil.ConvertToIpString(left + 100);
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
